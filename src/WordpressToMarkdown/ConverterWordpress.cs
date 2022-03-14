using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WordpressToMarkdown.Models;
using HtmlAgilityPack;
using System.Net;

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

		private HtmlDocument doc;

		public string ConvertToMarkdown(string contentPost)
		{
			contentPost = WebUtility.HtmlDecode(contentPost);
			doc = new HtmlDocument();
			doc.LoadHtml(contentPost);

			contentPost = ChangeHeader(contentPost);
			contentPost = ChangeStyle(contentPost);
			contentPost = ChangeList(contentPost);
			contentPost = ChangeLink(contentPost);
			contentPost = ChangeImage(contentPost);
			contentPost = ChangeCode(contentPost);
			contentPost = ChangeCodeBlock(contentPost);
			contentPost = ChangeRetourLigne(contentPost);

			return contentPost;
		}

		private string ChangeHeader(string content)
		{
			string[] headers = new string[] { "h1", "h2", "h3", "h4", "h5", "h6" };

			var lesHeaders = doc.DocumentNode.Descendants().Where(x => headers.Contains(x.Name));
			foreach (HtmlNode? header in lesHeaders)
			{
				string headerSelected = string.Empty;
				switch (header.Name)
				{
					case "h1":
						headerSelected = MarkdownSyntax.HEADER_1;
						break;
					case "h2":
						headerSelected = MarkdownSyntax.HEADER_2;
						break;
					case "h3":
						headerSelected = MarkdownSyntax.HEADER_3;
						break;
					case "h4":
						headerSelected = MarkdownSyntax.HEADER_4;
						break;
					case "h5":
						headerSelected = MarkdownSyntax.HEADER_5;
						break;
					case "h6":
						headerSelected = MarkdownSyntax.HEADER_6;
						break;
					default:
						break;
					
				}

				content = content.Replace(header.OuterHtml, headerSelected + header.InnerHtml + Environment.NewLine);
			}

			return content;
			//return content.Replace("<h1>", MarkdownSyntax.HEADER_1)
			//		.Replace(@"<\/h1>", Environment.NewLine)
			//		.Replace("<h2>", MarkdownSyntax.HEADER_2)
			//		.Replace(@"<\/h2>", Environment.NewLine)
			//		.Replace("<h3>", MarkdownSyntax.HEADER_3)
			//		.Replace(@"<\/h3>", Environment.NewLine)
			//		.Replace("<h4>", MarkdownSyntax.HEADER_4)
			//		.Replace(@"<\/h4>", Environment.NewLine)
			//		.Replace("<h5>", MarkdownSyntax.HEADER_5)
			//		.Replace(@"<\/h5>", Environment.NewLine)
			//		.Replace("<h6>", MarkdownSyntax.HEADER_6)
			//		.Replace(@"<\/h6>", Environment.NewLine);
		}

		private string ChangeRetourLigne(string content)
		{
			return content.Replace(RETOUR_LIGNE, Environment.NewLine)
						.Replace("<br>", MarkdownSyntax.SAUT_LIGNE)
						.Replace("<p>", Environment.NewLine)
						.Replace(@"<\/p>", MarkdownSyntax.SAUT_LIGNE)
						.Replace("</p>", MarkdownSyntax.SAUT_LIGNE);
		}

		private string ChangeCode(string content)
		{
			// Codes en ligne
			// Exemple :
			// <code data-enlighter-language="csharp" class="EnlighterJSRAW">LE code</code>
			var codesLine = doc.DocumentNode.Descendants().Where(d => d.Name == "code");

			foreach (var code in codesLine)
			{
				//string preClass = @"<code data-enlighter-language=";
				//string finPred = "</pre>";

				//int indexStart = content.IndexOf(preClass);
				//int indexEnd = content.IndexOf(finPred);

				//content = content.Remove(indexStart, indexEnd - indexStart + finPred.Length);
				//content = content.Insert(indexStart, "METTRE-CODE-ICI");

				//string debutCodeBlock = MarkdownSyntax.CODE_START + langage + Environment.NewLine;
				//content = content.Replace("METTRE-CODE-ICI", debutCodeBlock + item.InnerText + MarkdownSyntax.CODE_END);

				content = content.Replace(code.OuterHtml, MarkdownSyntax.CODE_IN_LINE + code.InnerHtml + MarkdownSyntax.CODE_IN_LINE);
			}

			return content;
			//return content.Replace("<code>", MarkdownSyntax.CODE_IN_LINE)
			//			.Replace(@"<\/code>", MarkdownSyntax.CODE_IN_LINE)
			//			.Replace("</code>", MarkdownSyntax.CODE_IN_LINE);
		}

		private string ChangeStyle(string content)
		{
			return content.Replace("<em>", MarkdownSyntax.ITALIC)
						.Replace(@"<\/em>", MarkdownSyntax.ITALIC)
						.Replace("</em>", MarkdownSyntax.ITALIC)
						.Replace("<strong>", MarkdownSyntax.BOLD)
						.Replace(@"<\/strong>", MarkdownSyntax.BOLD)
						.Replace("</strong>", MarkdownSyntax.BOLD);
		}

		private string ChangeImage(string content)
		{
			// Exemple :
			// <div class="wp-block-image"><figure class="aligncenter size-large"><img src="https://spectreconsole.net/assets/images/example.png" alt=""/><figcaption>Image de spectreconsole.net</figcaption></figure></div>
			var lesImages = doc.DocumentNode.Descendants().Where(x => x.Name == "div" 
																	&& x.HasAttributes 
																	&& x.Attributes["class"].Value == "wp-block-image");
			foreach (var item in lesImages)
			{
				// récup du tag img
				var image = item.Descendants().FirstOrDefault(x => x.Name == "img");
				if(image != null)
				{
					string div = "</div>";

					string divImg = @"<div class=""wp-block-image"">";
					int indexStart = content.IndexOf(divImg);
					int indexEnd = content.IndexOf(div);

					content = content.Remove(indexStart, indexEnd - indexStart + div.Length);
					content = content.Insert(indexStart, "METTRE-TAG-IMG-ICI");

					content = content.Replace("METTRE-TAG-IMG-ICI", image.OuterHtml);
				}
			}
			return content;
		}

		private string ChangeLink(string content)
		{
			// Exemple :
			// <a rel="noreferrer noopener" href="https://www.nuget.org/packages/Spectre.Console/0.43.1-preview.0.43" target="_blank">GitHub : Spectre.Console</a>
			var lesLinks = doc.DocumentNode.Descendants().Where(x => x.Name == "a");
			foreach (var item in lesLinks)
			{
				content = content.Replace(item.OuterHtml, "[" + item.InnerHtml + "](" + item.Attributes["href"].Value + ")");
			}

			return content;
		}

		private string ChangeCodeBlock(string content)
		{
			// ### Exemple AVEC language sélectionné:
			// <pre class="EnlighterJSRAW" data-enlighter-language="csharp" data-enlighter-theme="" data-enlighter-highlight="" data-enlighter-linenumbers="" data-enlighter-lineoffset="" data-enlighter-title="" data-enlighter-group="">using Spectre.Console;
			var preNode = doc.DocumentNode.Descendants().Where(d => d.Name == "pre");

			foreach (var item in preNode)
			{
				string langage = item.Attributes["data-enlighter-language"].Value;

				string preClass = @"<pre class=""EnlighterJSRAW""";
				string finPred = "</pre>";

				int indexStart = content.IndexOf(preClass);
				int indexEnd = content.IndexOf(finPred);

				content = content.Remove(indexStart, indexEnd - indexStart + finPred.Length);
				content = content.Insert(indexStart, "METTRE-CODE-ICI");

				string debutCodeBlock = MarkdownSyntax.CODE_START + langage + Environment.NewLine;
				content = content.Replace("METTRE-CODE-ICI", debutCodeBlock + item.InnerText + MarkdownSyntax.CODE_END);
			}

			return content;
		}


		private string ChangeList(string content)
		{
			string ul = "<ul>";
			string ulFin = "</ul>";
			string li = "<li>";
			string liFin = "</li>";


			content = content.Replace(ul, string.Empty);
			content = content.Replace(ulFin, string.Empty);
			content = content.Replace(li, "* ");
			content = content.Replace(liFin, "  /r/n");

			return content;
		}

	}
}
