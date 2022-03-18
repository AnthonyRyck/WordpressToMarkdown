using MarkdownWordpress.ModelsValidation;
using Microsoft.AspNetCore.Components;
using WordpressToMarkdown;
using WordpressToMarkdown.Models;

namespace MarkdownWordpress.ViewModels
{
	public class MainViewModel : IMainViewModel
	{
		public MainViewModel(WordpressCollector converterWordpress)
		{
			ResultConvertMd = new List<MarkdownResult>();
			Url = new WordpressSiteValidation();
			_converter = converterWordpress;
		}

		private WordpressCollector _converter;
		private Action State;

		#region Implement IMainViewModel

		/// <summary>
		/// URL d'un site Wordpress
		/// </summary>
		public WordpressSiteValidation Url { get; set; }

		public List<MarkdownResult> ResultConvertMd { get; set; }

		public MarkdownResult ResultMarkdownSelected { get; set; }

		public async void OnValidSubmitUrl()
		{
			try
			{
				ResultConvertMd = await _converter.GetPosts(Url.UrlWordpressSite);
				State.Invoke();
			}
			catch (Exception ex)
			{
				throw;
			}
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

		#endregion
	}
}
