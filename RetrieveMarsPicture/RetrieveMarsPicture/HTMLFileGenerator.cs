using System;
using System.Collections.Generic;
using System.IO;

namespace Sincioco {

	/// <summary>
	/// Used to generate an HTML output (like a thumbnail gallery of sorts) for the MARS
	/// images retrieved from NASA
	/// </summary>
	public class HTMLFileGenerator {

		// Templates used in the HTML file generation
		public string HTMLTemplate { get; set; }
		public string HTMLEarthDaySeperatorTemplate { get; set; }
		public string HTMLImageTemplate { get; set; }

		/// <summary>
		/// Default our instance-based HTML templates to the one globally defined
		/// in the constants.
		/// </summary>
		// ------------------------------------------------------------------------------------------
		public HTMLFileGenerator() {
			this.HTMLTemplate = Constant.HTMLOutputTemplate;
			this.HTMLEarthDaySeperatorTemplate = Constant.HTMLEarthDaySeperatorTemplate;
			this.HTMLImageTemplate = Constant.HTMLImageTemplate;
		}

		/// <summary>
		/// Generates the HTML output (thumbnail gallery) of the downloaded MARS images.
		/// </summary>
		/// <param name="downloadedFiles">A list of filenames of the downloaded images.</param>
		/// <param name="outputFilename">The nme of the HTML file to create.</param>
		/// <returns></returns>
		// ------------------------------------------------------------------------------------------
		public bool GenerateHTMLFile(List<DownloadedFile> downloadedFiles, string outputFilename) {

			bool result = false;

			if (downloadedFiles.Count > 0) {

				string HTMLToOutput = string.Empty;
				string workingHTML = string.Empty;
				string lastEarthDay = string.Empty;

				foreach (DownloadedFile file in downloadedFiles) {

					// Check if we encountered a different Earth Day (date)
					if (file.earth_date != lastEarthDay) {

						// If so, inject some sort of header (an h2 tag by default)
						workingHTML += String.Format(this.HTMLEarthDaySeperatorTemplate, file.earth_date);
					}

					// Add the image/thumbnail into the running HTML code being created
					workingHTML += String.Format(this.HTMLImageTemplate, file.filename);

					// Remember the current Earth Date (date)
					lastEarthDay = file.earth_date;
				}

				// Produce the final HTML file to output
				HTMLToOutput = String.Format(this.HTMLTemplate, workingHTML);

				try {

					// Write it to a file.
					File.WriteAllText(outputFilename, HTMLToOutput);
					result = true;
				} catch (Exception) {
					//throw;
					// TODO:  Log exception here but allow the program to continue.
					//        The failure of this operation is not crucial.
				}
			}

			return result;
		}
	}
}
