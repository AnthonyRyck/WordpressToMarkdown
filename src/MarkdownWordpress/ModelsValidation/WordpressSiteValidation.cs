using System.ComponentModel.DataAnnotations;


namespace MarkdownWordpress.ModelsValidation
{
	public class WordpressSiteValidation
	{
		[Required(ErrorMessage = "Il faut un site wordpress")]
		public string UrlWordpressSite { get; set; }
	}
}
