using MarkdownWordpress.Models;
using System.Text.Json;

namespace MarkdownWordpress.ViewModels
{
	public class SuiviDownload : ISuiviDownload
	{
		public Download HistoDownload { get; private set; }

		private string path;
		

		public SuiviDownload()
		{
			HistoDownload = new Download();
			path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "suivi", "suivi.json");
			LoadSaveFile().GetAwaiter();
		}
		
		
		
		private async Task LoadSaveFile()
		{
			string pathRep = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "suivi");
			
			if (!Directory.Exists(pathRep))
			{
				Directory.CreateDirectory(pathRep);
			}

			if (File.Exists(path))
			{
				string json = await File.ReadAllTextAsync(path);
				HistoDownload = JsonSerializer.Deserialize<Download>(json);
			}
			else
			{
				HistoDownload = new Download();
			}			
		}


		public async Task AddDownload()
		{
			HistoDownload.NombreDownloads++;
			HistoDownload.LastTime = DateTime.Now;

			string json = JsonSerializer.Serialize(HistoDownload);
			await File.WriteAllTextAsync(path, json);
		}

	}
}
