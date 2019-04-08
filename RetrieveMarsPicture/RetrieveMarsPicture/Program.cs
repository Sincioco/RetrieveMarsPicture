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

		// Parameters that could be passed into the args in Main (in future versions)
		static string workingDirectory = @"C:\Temp\";
		static string localPathForSavingImages = workingDirectory;
		static string requestFile = workingDirectory + "NasaRequest.txt";
		static string basePoint = @"https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY&date={0}";

		static void Main(string[] args) {

			if (File.Exists(requestFile) == true) {

				// Read the file containing the list of dates and iterate
				// through them, sending a GET request to NASA and downloading
				// the Mars image.

				ResultfromNASA resultFromNASA;

				string[] dates = File.ReadAllLines(requestFile);

				for (int i = 0; i < dates.Length; i++) {

					DateTime date;

					if (DateTime.TryParse(dates[i], out date) == true) {

						// We have a valid date

						string requestDate = date.ToString("yyyy-MM-dd");
						Console.WriteLine(requestDate);

						string fullNASAEndPoint = String.Format(basePoint, requestDate);

						// Download the Image for the specific date
						resultFromNASA = DonwloadMarsImage(fullNASAEndPoint).GetAwaiter().GetResult();

					} else {

						// Error Handler for when an invalid date was supplied
						Console.WriteLine(dates[i]);
						Console.WriteLine("\tDate conversion failed. Check to make sure the date is valid.");
					}
				}

			} else {

				// Error Handler for when we cannot find the expected file
				Console.WriteLine(requestFile + " could not be found.  Please ensure that it exists and it contains a list of dates.");

			}

			Console.WriteLine("Press a key to continue");
			Console.ReadKey();

		}

		static async Task<ResultfromNASA> DonwloadMarsImage(string RequestUri) {

			Console.WriteLine("\tWeb Request Sent to:\n\t  " + RequestUri);

			using (HttpClient httpClient = new HttpClient()) {

				ResultfromNASA result = null;

				try {

					// Send a HTTP Get request to NASA and deserialize the returned JSON result
					HttpResponseMessage response = await httpClient.GetAsync(RequestUri);
					var responseContent = await response.Content.ReadAsStringAsync();
					result = JsonConvert.DeserializeObject<ResultfromNASA>(responseContent);

					// Ensure we got a valid Image URL
					if (String.IsNullOrEmpty(result.url) == false) {

						string filename = System.IO.Path.GetFileName(result.url);

						Console.WriteLine("\tImage URL Retrieved:\n\t  " + result.url);
						Console.WriteLine("\tDownloading " + filename);

						// Download the file
						using (WebClient client = new WebClient()) {
							client.DownloadFile(new Uri(result.url), localPathForSavingImages + filename);
							Console.WriteLine("\t  Saved to " + localPathForSavingImages + filename);
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
	}
}
