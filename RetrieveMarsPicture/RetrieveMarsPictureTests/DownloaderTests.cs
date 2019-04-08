using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sincioco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
		/// Test to make sure that we can output an HTML file.
		/// </summary>
		[TestMethod()]
		public void Test004_CreateHTMLOutputFile() {
		
			if (downloader.CreateHTMLOutputFile(new string[]{ "https://apod.nasa.gov/apod/image/1904/AzurePlumesNorway_Sutie_960.jpg" }) == false) {
				Assert.Fail();
			}
		}
	}
}