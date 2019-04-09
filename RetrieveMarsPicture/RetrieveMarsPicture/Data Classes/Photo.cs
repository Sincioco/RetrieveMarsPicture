namespace Sincioco {

	public class Photo {
		public string id { get; set; }
		public string sol { get; set; }
		public Camera camera { get; set; }
		public Rover rover { get; set; }
		public string earth_date { get; set; }
		public string img_src { get; set; }
	}
}
