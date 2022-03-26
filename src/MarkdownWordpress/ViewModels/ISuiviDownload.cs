using MarkdownWordpress.Models;

namespace MarkdownWordpress.ViewModels
{
	public interface ISuiviDownload
	{
		/// <summary>
		/// Indique le nombre de téléchargement fait.
		/// </summary>
		Download HistoDownload { get; }

		/// <summary>
		/// Ajoute un téléchargement.
		/// </summary>
		/// <returns></returns>
		Task AddDownload();
	}
}
