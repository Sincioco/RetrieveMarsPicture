// ====================================================================================================
//                                        Mars Picture Downloader Unit Tests
// ====================================================================================================
// Programmed By:  Louiery R. Sincioco													  Version:  .01
// Programmed Date:  April 8, 2019
// ====================================================================================================
// Purpose:  Test the methods of the Downloader.cs class to make sure that: 1.) They work as intended;
//           2.) We have as close to 100% code coverage as possible.
// ====================================================================================================

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sincioco.Tests {
	[TestClass()]
	public class DownloaderTests {

		private Downloader downloader;

		[TestInitialize]
		public void TestInit() {
			downloader = new Downloader();

			// All the test cases depends on C:\Temp and C:\Temp\NASARequest.txt
			downloader.EnsureRequestFileWithDatesExists();
		}

		/// <summary>
		/// Test to make sure that the directory C:\Temp exists and the file NASARequest.txt in it.
		/// </summary>
		[TestMethod()]
		public void Test001_EnsureRequestFileWithDatesExists() {			

			if (Directory.Exists(Constant.workingDirectory) == false) {
				Assert.Fail(Constant.workingDirectory + " does not exist.");
			}


			// Make C:\Temp and C:\Temp\NASARequest.txt exists
			downloader.EnsureRequestFileWithDatesExists();

			// Now delete C:\Temp\NASARequest.txt
			File.Delete(Constant.inputFileWithListOfDates);

			// Call this method a second time again (testing a different branch)
			downloader.EnsureRequestFileWithDatesExists();

			if (File.Exists(Constant.inputFileWithListOfDates) == false) {
				Assert.Fail(Constant.inputFileWithListOfDates + " does not exist.");
			}

			if (File.Exists(Constant.inputFileWithListOfDates) == false) {
				Assert.Fail(Constant.inputFileWithListOfDates + " does not exist.");
			}
			
		}

		[TestMethod()]
		public void Test002_RetrieveImageWithDate() {

			DateTime date = Convert.ToDateTime("2019-04-08");
			string filename = downloader.RetrieveImageWithDate(date).GetAwaiter().GetResult();

			if (String.IsNullOrEmpty(filename) == true) {
				Assert.Fail("Unable to retrieve an image.  Please make sure you have not reached your daily API Limit quota from NASA.");
			}
		}

		/// <summary>
		/// Test to make sure that we can download a file from NASA
		/// </summary>
		[TestMethod()]
		public void Test003_DownloadFile() {

			string localFile = @"C:\Temp\AzurePlumesNorway_Sutie_960.jpg";
			File.Delete(localFile);

			downloader.DownloadFile("https://apod.nasa.gov/apod/image/1904/AzurePlumesNorway_Sutie_960.jpg", localFile);

			if (File.Exists(localFile) == false) {
				Assert.Fail("Failed to download a file.");
			}
		}

		/// <summary>
		/// Test to make sure that we can our exception is working as intended in the DownloadFile() method
		/// </summary>
		[TestMethod()]
		public void Test003_DownloadFile_ExceptionTestCoverage() {

			string invalidUri = @"https://apod.nasa.gov/apod/image/1904/AzurePlumesNorway_Sutie_960.jpg1";
			string invalidFileName = @"C:\Temp\Failure.jpg";

			try {
				downloader.DownloadFile(invalidUri, invalidFileName);
				Assert.Fail("We were expecting a failure state.");
			} catch {
				// Blackhole exception since we purposely triggered it for increased code coverage.
			}
		}

		/// <summary>
		/// Test to make sure that we can output an HTML file.
		/// </summary>
		[TestMethod()]
		public void Test004_CreateHTMLOutputFile() {
		
			if (downloader.CreateHTMLOutputFile(new string[]{
				"https://apod.nasa.gov/apod/image/1904/AzurePlumesNorway_Sutie_960.jpg" }, 
				Constant.HTMLOutputFilename) == false) {

				Assert.Fail();
			}
		}

		/// <summary>
		/// Test to make sure that we can our exception is working as intended in the CreateHTMLOutputFile() method
		/// </summary>
		[TestMethod()]
		public void Test004_CreateHTMLOutputFile_ExceptionTestCoverage() {

			string invalidPath = @"C:\InvalidFolder\Test.jpg";

			try {
				if (downloader.CreateHTMLOutputFile(new string[] {
				"https://apod.nasa.gov/apod/image/1904/AzurePlumesNorway_Sutie_960.jpg" }, invalidPath) == false) {

					Assert.Fail();
				}
			} catch {
				// Blackhole exception since we purposely triggered it for increased code coverage.
			}

		}
	}
}