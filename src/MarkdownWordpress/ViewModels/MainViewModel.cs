using BlazorDownloadFile;
using MarkdownWordpress.ModelsValidation;
using Microsoft.AspNetCore.Components;
using System.Text;
using WordpressToMarkdown;
using WordpressToMarkdown.Models;

namespace MarkdownWordpress.ViewModels
{
	public class MainViewModel : IMainViewModel
	{
		public MainViewModel(WordpressCollector converterWordpress, IBlazorDownloadFileService downloadService)
		{
			ResultConvertMd = new List<MarkdownResult>();
			Url = new WordpressSiteValidation();
			_converter = converterWordpress;
			downloadSvc = downloadService;
		}

		private WordpressCollector _converter;
		private Action State;
		private IBlazorDownloadFileService downloadSvc;

		#region Implement IMainViewModel

		/// <summary>
		/// URL d'un site Wordpress
		/// </summary>
		public WordpressSiteValidation Url { get; set; }

		public List<MarkdownResult> ResultConvertMd { get; set; }

		public MarkdownResult ResultMarkdownSelected { get; set; }

		public bool IsLoading { get; private set; }

		public bool IsError { get; private set; }

		public async void OnValidSubmitUrl()
		{
			IsLoading = true;
			IsError = false;
			try
			{
				ResultConvertMd = await _converter.GetPosts(Url.UrlWordpressSite);
			}
			catch (Exception ex)
			{
				IsError = true;
			}
			IsLoading = false;

			State.Invoke();
		}


		public void OnSelectResultMd(ChangeEventArgs e)
		{
			ResultMarkdownSelected = ResultConvertMd.FirstOrDefault(x => x.Titre == e.Value.ToString());
			State.Invoke();
		}


		public void SetStateHasChanged(Action state)
		{
			State = state;
		}


		public async Task Download()
		{
			// créer un fichier
			byte[] content = Encoding.UTF8.GetBytes(ResultMarkdownSelected.MarkdownContent);
			await downloadSvc.DownloadFile(ResultMarkdownSelected.Titre + ".md", content, "application/octet-stream");
		}


		#endregion
	}
}
