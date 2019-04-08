// ====================================================================================================
//                                           Mars Picture Downloader
// ====================================================================================================
// Programmed By:  Louiery R. Sincioco													  Version:  .01
// Programmed Date:  April 8, 2019
// ====================================================================================================
// Purpose:  Given a file, read its content (a list of dates) and retrieve the corresponding Mars
//           picture using NASA's public API (api.nasa.gov).
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace Sincioco {

	class RetrieveMarsPicture {

		// ------------------------------------------------------------------------------------------
		static void Main(string[] args) {

			Downloader downloader = new Downloader();
			downloader.EnsureRequestFileWithDatesExists();								// C:\Temp\NASARequest.txt

			// Used for the HTML Output (a list of image file names)
			List<string> filesDownloaded = new List<string>();
			string[] dates = File.ReadAllLines(Constant.inputFileWithListOfDates);

			// Iterate through the list of dates
			for (int i = 0; i < dates.Length; i++) {

				DateTime date;

				if (DateTime.TryParse(dates[i], out date) == true) {

					// Download the Image for the specific date and return the filename
					string filename = downloader.RetrieveImageWithDate(date).GetAwaiter().GetResult();

					// Collect the list of file names for the images we downloaded (for the HTML output)
					if (filename != null) filesDownloaded.Add(filename);

				} else {
					Console.WriteLine(dates[i]);
					Console.WriteLine("\tDate conversion failed. Check to make sure the date is valid.");
				}
			}

			if (filesDownloaded.Count > 0) {

				// Create HTML Output file and launch it in the browser.
				downloader.CreateHTMLOutputFile(filesDownloaded.ToArray(), Constant.HTMLOutputFilename);
				System.Diagnostics.Process.Start(Constant.HTMLOutputFilename);
			}

			Console.WriteLine("Press a key to continue");
			Console.ReadKey();

		}
		
	}
}
