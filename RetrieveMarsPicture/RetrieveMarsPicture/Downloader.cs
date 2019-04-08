// ====================================================================================================
//                                           Mars Picture Downloader
// ====================================================================================================
// Programmed By:  Louiery R. Sincioco													  Version:  .01
// Programmed Date:  April 8, 2019
// ====================================================================================================
// Purpose:  Encapsulate all the methods necessary to download a series of MARS images from NASA
//           into a class so it can be unit tested.
// ====================================================================================================

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sincioco {

	/// <summary>
	/// Takes care of downloading an image from MARS using NASA's public API.
	/// </summary>
	public class Downloader {

		// ------------------------------------------------------------------------------------------
		/// <summary>
		/// Ensure that a file named NASARequest.txt exists in C:\Temp
		/// </summary>
		public void EnsureRequestFileWithDatesExists() {

			// Ensure C:\Temp exists
			System.IO.Directory.CreateDirectory(Constant.workingDirectory);

			// Ensure NASARequest.txt exists
			if (File.Exists(Constant.inputFileWithListOfDates) == false) {
				File.WriteAllLines(Constant.inputFileWithListOfDates, Constant.sampleTextFileWithDates);
			}
		}

		// ------------------------------------------------------------------------------------------
		/// <summary>
		/// Given a date, download the Mars image for that date.
		/// </summary>
		/// <param name="date">In the format yyyy-MM-dd</param>
		/// <returns>A filename if successfully downloaded and saved locally</returns>
		public async Task<Photos> RetrievePhotosMetadata(DateTime date, bool flag_UseTestData = false) {

			Photos webAPIResult = null;

			string requestDate = date.ToString("yyyy-MM-dd");
			string RequestUri = String.Format(Constant.basePoint, requestDate);

			Console.WriteLine(requestDate);
			Console.WriteLine("\tWeb Request Sent to:\n\t  " + RequestUri);

			using (HttpClient httpClient = new HttpClient()) {

				string responseContent = string.Empty;

				if (flag_UseTestData == false) {

					try {

						// Send a HTTP Get request to NASA and deserialize the returned JSON result
						HttpResponseMessage response = await httpClient.GetAsync(RequestUri);
						responseContent = await response.Content.ReadAsStringAsync();

					} catch (Exception ex) {

						// Error Handler for when we have no Internet connection or we could not reach the remote server.
						Console.WriteLine("\tERROR: " + ex.Message);

						if (String.IsNullOrEmpty(ex.InnerException.Message) == false) {
							Console.WriteLine("\t  " + ex.InnerException.Message);
						}
					}

				} else {
					// Use for Unit Testing or once we have exceeded our daily API call Limit
					responseContent = File.ReadAllText("Photo_Two.json");
				}

				if (String.IsNullOrEmpty(responseContent) == false) {
					webAPIResult = JsonConvert.DeserializeObject<Photos>(responseContent);
				}

				return webAPIResult;
			}

		}

		// ------------------------------------------------------------------------------------------
		/// <summary>
		/// Given an array of Photos, retrieve all the images
		/// </summary>
		/// <param name="photos">The top-level object that contains a property named 'photos' which is an array of MARS photo.</param>
		/// <returns>a list of filenames that were saved locally</returns>
		public List<DownloadedFile> DownloadMARSPhotos(Photos photos) {

			List<DownloadedFile> downloadedFiles = new List<DownloadedFile>();

			foreach (Photo photo in photos.photos) {

				// Ensure we got an Image URL
				if (String.IsNullOrEmpty(photo.img_src) == false) {

					// Extract just the filename
					string filename = System.IO.Path.GetFileName(photo.img_src);

					//Console.WriteLine("\tImage URL Retrieved:\n\t  " + photo.img_src);
					Console.WriteLine("\tDownloading " + filename);

					// Download the file
					if (this.DownloadFile(photo.img_src, Constant.localPathForSavingImages + filename) == true) {
						Console.WriteLine("\t  Saved to " + Constant.localPathForSavingImages + filename);
						DownloadedFile downloadfile = new DownloadedFile() { filename = filename, earth_date = photo.earth_date };
						downloadedFiles.Add(downloadfile);
					}

				} else {
					Console.WriteLine("\tNo image were returned.\n\t  You may have exceeded your hourly quota from NASA.");
				}
			}

			return downloadedFiles;

		}

		// ------------------------------------------------------------------------------------------
		/// <summary>
		/// Takes an image URI and write it to a local file.
		/// </summary>
		/// <param name="uri">The full URI of the image on the web.</param>
		/// <param name="filename">The full local path to where to save the image to.</param>
		/// <returns></returns>
		public bool DownloadFile(string uri, string filename) {

			bool result = false;

			try {

				using (WebClient client = new WebClient()) {
					client.DownloadFile(new Uri(uri), filename);
					result = true;
				}

			} catch (Exception) {
				throw;
			}


			return result;
		}

		// ------------------------------------------------------------------------------------------
		public bool CreateHTMLOutputFile(List<DownloadedFile> downloadedFiles, string outputFilename) {

			bool result = false;

			if (downloadedFiles.Count > 0) {

				string HTMLToOutput = string.Empty;
				string workingHTML = string.Empty;
				string lastEarthDay = string.Empty;

				foreach (DownloadedFile file in downloadedFiles) {

					if (file.earth_date != lastEarthDay) {
						workingHTML += String.Format(Constant.HTMLEarthDaySeperatorTemplate, file.earth_date);
					}

					workingHTML += String.Format(Constant.HTMLImageTemplate, file.filename);
					lastEarthDay = file.earth_date;
				}

				HTMLToOutput = String.Format(Constant.HTMLOutputTemplate, workingHTML);

				try {
					File.WriteAllText(outputFilename, HTMLToOutput);
					result = true;
				} catch (Exception) {
					throw;
				}
			}

			return result;
		}

	}
}
