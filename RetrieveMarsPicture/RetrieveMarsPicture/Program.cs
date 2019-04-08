// ====================================================================================================
//                                           Mars Picture Downloader
// ====================================================================================================
// Programmed By:  Louiery R. Sincioco													  Version:  .01
// Programmed Date:  April 8, 2019 12:00pm
// ====================================================================================================
// Purpose:  Given a file, read its content (a list of dates) and retrieve the corresponding Mars
//           picture using NASA's public API (api.nasa.gov).
// ====================================================================================================

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sincioco {

	class RetrieveMarsPicture {

		static void Main(string[] args) {

			// Ensure C:\Temp exists
			System.IO.Directory.CreateDirectory(Constant.workingDirectory);

			// Ensure NASARequest.txt exists
			if (File.Exists(Constant.requestFile) == false) {
				File.WriteAllLines(Constant.requestFile, Constant.sampleTextFileWithDates);
			}

			// Read the file containing the list of dates and iterate
			// through them, sending a GET request to NASA and downloading
			// the Mars image.

			List<string> listOfFileNames = new List<string>();		// Used for the HTML Output (a list of image file names)
			string[] dates = File.ReadAllLines(Constant.requestFile);

			for (int i = 0; i < dates.Length; i++) {

				DateTime date;

				if (DateTime.TryParse(dates[i], out date) == true) {

					// We have a valid date

					string requestDate = date.ToString("yyyy-MM-dd");
					string filename = null;
					Console.WriteLine(requestDate);

					string fullNASAEndPoint = String.Format(Constant.basePoint, requestDate);

					// Download the Image for the specific date and return the filename
					filename = DownloadMarsImage(fullNASAEndPoint).GetAwaiter().GetResult();

					// Collect the list of file names for the images we downloaded
					if (filename != null) {
						listOfFileNames.Add(filename);
					}

				} else {

					// Error Handler for when an invalid date was supplied
					Console.WriteLine(dates[i]);
					Console.WriteLine("\tDate conversion failed. Check to make sure the date is valid.");
				}
			}

			if (listOfFileNames.Count > 0) {
				CreateHTMLOutputFile(listOfFileNames.ToArray());
				System.Diagnostics.Process.Start(Constant.HTMLOutputFilename);
			}

			Console.WriteLine("Press a key to continue");
			Console.ReadKey();

		}

		static async Task<string> DownloadMarsImage(string RequestUri) {

			Console.WriteLine("\tWeb Request Sent to:\n\t  " + RequestUri);

			using (HttpClient httpClient = new HttpClient()) {

				string result = null;
				Result resultFromNASA = null;

				try {

					// Send a HTTP Get request to NASA and deserialize the returned JSON result
					HttpResponseMessage response = await httpClient.GetAsync(RequestUri);
					var responseContent = await response.Content.ReadAsStringAsync();
					resultFromNASA = JsonConvert.DeserializeObject<Result>(responseContent);

					// Ensure we got an Image URL
					if (String.IsNullOrEmpty(resultFromNASA.url) == false) {

						// Extract just the filename
						string filename = System.IO.Path.GetFileName(resultFromNASA.url);

						Console.WriteLine("\tImage URL Retrieved:\n\t  " + resultFromNASA.url);
						Console.WriteLine("\tDownloading " + filename);

						// Check if the file already exists
						if (File.Exists(Constant.workingDirectory + filename) == false ) {

							// If it doesn't exists, downloaded the file 
							using (WebClient client = new WebClient()) {

								client.DownloadFile(new Uri(resultFromNASA.url), Constant.localPathForSavingImages + filename);

								Console.WriteLine("\t  Saved to " + Constant.localPathForSavingImages + filename);

								// We need this for the HTML Output
								result = filename;
							}

						} else {

							// Otherwise, just use return the previously downloaded file (we need this for the HTML Output)

							// Make sure it's not zero bytes (a failed download)
							if (new System.IO.FileInfo(Constant.workingDirectory + filename).Length > 0) {
								result = filename;
							}
						}
					} else {

						// Error Handler for when no result was returned
						// possibly due to reaching API call limit quota
						Console.WriteLine("\tNo results were returned.\n\t  You may have exceeded your hourly quota from NASA.");
					}

				} catch (Exception ex) {

					// Error Handler for when we have no Internet connection
					// or we could not reach the remote server.

					Console.WriteLine("\tERROR: " + ex.Message);

					if (String.IsNullOrEmpty(ex.InnerException.Message) == false) {
						Console.WriteLine("\t  " + ex.InnerException.Message);
					}
				}

				return result;
			}

		}

		static void CreateHTMLOutputFile(string[] fileList) {

			string HTMLToOutput = string.Empty;
			string workingHTML = string.Empty;

			for (int i = 0; i < fileList.Length; i++) {
				workingHTML += String.Format(Constant.HTMLImageTemplate, fileList[i]);
			}

			HTMLToOutput = String.Format(Constant.HTMLOutputTemplate, workingHTML);

			File.WriteAllText(Constant.HTMLOutputFilename, HTMLToOutput);

		}

	}
}
