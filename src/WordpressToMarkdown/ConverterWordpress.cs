using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WordpressToMarkdown.Models;

namespace WordpressToMarkdown
{
	public class ConverterWordpress
	{
		const string RETOUR_LIGNE = @"\n\n\n\n";

		
		public async Task<List<PostWordPress>> ConvertToObject(Stream content)
		{
			return await JsonSerializer.DeserializeAsync<List<PostWordPress>>(content);
		}
		
		public async Task<List<PostWordPress>> GetPosts(string url)
		{
			List<PostWordPress> posts = new List<PostWordPress>();

			using (var client = new HttpClient())
			{
				var streamPosts = await client.GetStreamAsync(url);
				posts = await JsonSerializer.DeserializeAsync<List<PostWordPress>>(streamPosts);
			}

			return posts;
		}


		public string ConvertToMarkdown(string contentPost)
		{
			contentPost = ChangeRetourLigne(contentPost);			
			contentPost = ChangeHeader(contentPost);
			contentPost = ChangeCode(contentPost);
			contentPost = ChangeStyle(contentPost);
			return contentPost;
		}

		private string ChangeHeader(string content)
		{
			return content.Replace("<h1>", MarkdownSyntax.HEADER_1)
					.Replace(@"<\/h1>", Environment.NewLine)
					.Replace("<h2>", MarkdownSyntax.HEADER_2)
					.Replace(@"<\/h2>", Environment.NewLine)
					.Replace("<h3>", MarkdownSyntax.HEADER_3)
					.Replace(@"<\/h3>", Environment.NewLine)
					.Replace("<h4>", MarkdownSyntax.HEADER_4)
					.Replace(@"<\/h4>", Environment.NewLine)
					.Replace("<h5>", MarkdownSyntax.HEADER_5)
					.Replace(@"<\/h5>", Environment.NewLine)
					.Replace("<h6>", MarkdownSyntax.HEADER_6)
					.Replace(@"<\/h6>", Environment.NewLine);
		}

		private string ChangeRetourLigne(string content)
		{
			return content.Replace(RETOUR_LIGNE, Environment.NewLine)
						.Replace("<br>", MarkdownSyntax.SAUT_LIGNE)
						.Replace("<p>", Environment.NewLine)
						.Replace(@"<\/p>", MarkdownSyntax.SAUT_LIGNE);
		}

		private string ChangeCode(string content)
		{
			return content.Replace("<code>", MarkdownSyntax.CODE_IN_LINE)
						.Replace(@"<\/code>", MarkdownSyntax.CODE_IN_LINE);
		}

		private string ChangeStyle(string content)
		{
			return content.Replace("<em>", MarkdownSyntax.ITALIC)
						.Replace(@"<\/em>", MarkdownSyntax.ITALIC)
						.Replace("<strong>", MarkdownSyntax.BOLD)
						.Replace(@"<\/strong>", MarkdownSyntax.BOLD);
		}

		private string ChangeImage(string content)
		{
			return content;
		}
	}
}
