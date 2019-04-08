using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincioco {

	/// <summary>
	/// The structure of the data that NASA's API returns.
	/// </summary>
	/// 
	public class Photos {
		public Photo[] photos { get; set; }
	}
	public class Photo {
		public string id { get; set; }
		public string sol { get; set; }
		public Camera camera { get; set; }
		public Rover rover { get; set; }
		public string earth_date { get; set; }
		public string img_src { get; set; }
	}

	public class Camera {
		public string id { get; set; }
		public string name { get; set; }
		public int rover_id { get; set; }
		public string full_name { get; set; }
	}

	public class Rover {
		public string id { get; set; }
		public string name { get; set; }
		public string landing_date { get; set; }
		public string launch_date { get; set; }
		public string status { get; set; }
		public int max_sol { get; set; }
		public string max_date { get; set; }
		public int total_photos { get; set; }
		public Cameras[] cameras { get; set; } 

	}

	public class Cameras {
		public string name { get; set; }
		public string full_name { get; set; }
	}

	public class DownloadedFile {
		public string filename { get; set; }
		public string earth_date { get; set; }
	}
}
