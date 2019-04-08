using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincioco {

	/// <summary>
	/// The various constants used throughout this program.
	/// </summary>
	public static class Constant {

		// The working directory
		public const string workingDirectory = @"C:\Temp\";

		// The path where we will save the download MARS images (assume working directory)
		public const string localPathForSavingImages = workingDirectory;

		// The name of the text file that contains the list of dates
		public const string inputFileWithListOfDates = workingDirectory + "NASARequest.txt";

		// A list of dates for NASARequest.txt in case we have to create it (we use the provided dates from the test)
		public static readonly string[] sampleTextFileWithDates = { "02/27/17", "June 2, 2018", "Jul-13-2016", "April 31, 2018" };

		// The NASA public end point so we can fetch the MARS images as per the list of dates
		public const string basePoint = @"https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY&date={0}";

		// The HTML output we will generate at the end of the program execution
		public const string HTMLOutputFilename = workingDirectory + "Results.html";

		// A basic HTML template
		public const string HTMLOutputTemplate = @"
			<html>
			<body>
			<h1>MARS Images Downloaded</h1>
			{0}
			</body>
			</html>
		";

		// A basic HTML image tag (notice we hard-coded a width of 50% on purpose so we can see multiple images on one screen) 
		public const string HTMLImageTemplate = @"
			<img src=""{0}"" style=""width:500"">
		";

	}
}
