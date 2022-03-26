namespace MarkdownWordpress.Models
{
	public class Download
	{
		public int NombreDownloads { get; set; }

		public DateTime LastTime { get; set; }


		public Download()
		{
			NombreDownloads = 0;
			LastTime = DateTime.Now;
		}
	}
}
