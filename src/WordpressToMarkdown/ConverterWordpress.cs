using WordpressToMarkdown.Models;
using HtmlAgilityPack;
using System.Net;

namespace WordpressToMarkdown
{
	public class ConverterWordpress
	{
		/// <summary>
		/// Arrive quand il y a changement de paragraphe
		/// </summary>
		const string CHAGEMENT_PARAGRAPHE = "\n\n\n\n";
		
		/// <summary>
		/// C'est le post en version "HTML"
		/// </summary>
		private HtmlDocument htmlDocPost;




		public Task<string> ConvertToMarkdownAsync(string title, string contentPost, params ITransformWordpress[] transformations)
		{
			return Task.Factory.StartNew(() =>
			{
				contentPost = WebUtility.HtmlDecode(contentPost);
				htmlDocPost = new HtmlDocument();
				htmlDocPost.OptionCheckSyntax = false;
				htmlDocPost.OptionAutoCloseOnEnd = false;
				htmlDocPost.OptionOutputOriginalCase = true;
				htmlDocPost.OptionWriteEmptyNodes = true;
				htmlDocPost.LoadHtml(contentPost);

				contentPost = ChangeHeader(contentPost);
				contentPost = ChangeStyle(contentPost);
				contentPost = ChangeList(contentPost);
				contentPost = ChangeLink(contentPost);
				contentPost = ChangeImage(contentPost);
				contentPost = ChangeVideo(contentPost);
				contentPost = ChangeCodeEnLigne(contentPost);
				contentPost = ChangeCodeBlock(contentPost);
				contentPost = ChangeRetourLigne(contentPost);

				foreach (var userTransform in transformations)
				{
					contentPost = userTransform.Transform(contentPost);
				}

				contentPost.Insert(1, MarkdownSyntax.HEADER_1 + title);
				return contentPost;
			});
		}

		private string ChangeHeader(string content)
		{
			string[] headers = new string[] { "h1", "h2", "h3", "h4", "h5", "h6" };

			var lesHeaders = htmlDocPost.DocumentNode.Descendants().Where(x => headers.Contains(x.Name));
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
		}

		private string ChangeRetourLigne(string content)
		{
			content = content.Replace(CHAGEMENT_PARAGRAPHE, string.Empty)
						.Replace("<br>", MarkdownSyntax.SAUT_LIGNE)
						.Replace("<p>", Environment.NewLine)
						.Replace(@"<\/p>", MarkdownSyntax.SAUT_LIGNE)
						.Replace("</p>", MarkdownSyntax.SAUT_LIGNE);
			return content;
		}

		private string ChangeCodeEnLigne(string content)
		{
			// Codes en ligne
			// Exemple :
			// <code data-enlighter-language="csharp" class="EnlighterJSRAW">LE code</code>
			var codesLine = htmlDocPost.DocumentNode.Descendants().Where(d => d.Name == "code");

			foreach (var code in codesLine)
			{
				content = content.Replace(code.OuterHtml, MarkdownSyntax.CODE_IN_LINE + code.InnerHtml + MarkdownSyntax.CODE_IN_LINE);
			}

			return content;
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
			var lesImages = htmlDocPost.DocumentNode.Descendants().Where(x => x.Name == "div" 
																	&& x.HasAttributes 
																	&& x.Attributes["class"].Value == "wp-block-image");
			foreach (var item in lesImages)
			{
				try
				{
					string textCaption = string.Empty;
					// récup des figcaption
					var caption = item.Descendants().FirstOrDefault(x => x.Name == "figcaption");
					if(caption != null)
						textCaption = caption.InnerText;

					// récup du tag img
					var image = item.Descendants().FirstOrDefault(x => x.Name == "img");
					if (image != null)
					{
						string div = "</div>";

						string divImg = @"<div class=""wp-block-image";
						int indexStart = content.IndexOf(divImg);
						int indexEnd = content.IndexOf(div);

						content = content.Remove(indexStart, indexEnd - indexStart + div.Length);

						if(string.IsNullOrEmpty(textCaption))
						{
							content = content.Insert(indexStart, "METTRE-TAG-IMG-ICI");
						}
						else
						{
							content = content.Insert(indexStart, "METTRE-TAG-IMG-ICI  " 
													+ Environment.NewLine 
													+ MarkdownSyntax.ITALIC + textCaption + MarkdownSyntax.ITALIC 
													+ MarkdownSyntax.SAUT_LIGNE);
						}

						content = content.Replace("METTRE-TAG-IMG-ICI", image.OuterHtml + Environment.NewLine);
					}
				}
				catch (Exception ex)
				{
					// On laisse le contenu.
				}
			}

			return content;
		}

		private string ChangeVideo(string content)
		{
			string URL_VIDEO = "METTRE-URL-VIDEO";
			string figureStart = @"<figure class=""wp-block-video aligncenter"">";
			string figureEnd = "</figure>";

			// Exemple :
			// <figure class="wp-block-video aligncenter"><video controls src="https://spectreconsole.net/assets/images/table.webm"></video><figcaption>Image de spectreconsole.net</figcaption></figure>
			var lesImages = htmlDocPost.DocumentNode.Descendants().Where(x => x.Name == "figure"
																	&& x.HasAttributes
																	&& x.Attributes["class"].Value.Contains("wp-block-video"));
			foreach (var item in lesImages)
			{
				// récup du tag img
				var videoTag = item.Descendants().FirstOrDefault(x => x.Name == "video");
				if (videoTag != null)
				{
					int indexStart = content.IndexOf(figureStart);
					int indexEnd = content.IndexOf(figureEnd);

					content = content.Remove(indexStart, indexEnd - indexStart + figureEnd.Length);
					content = content.Insert(indexStart, URL_VIDEO);

					content = content.Replace(URL_VIDEO, MarkdownSyntax.BOLD + "[LINK TO VIDEO](" + videoTag.Attributes["src"].Value + ")" + MarkdownSyntax.BOLD + Environment.NewLine);
				}
			}
			return content;
		}

		private string ChangeLink(string content)
		{
			// Exemple :
			// <a rel="noreferrer noopener" href="https://www.nuget.org/packages/Spectre.Console/0.43.1-preview.0.43" target="_blank">GitHub : Spectre.Console</a>
			var lesLinks = htmlDocPost.DocumentNode.Descendants().Where(x => x.Name == "a");
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
			var preNode = htmlDocPost.DocumentNode.Descendants().Where(d => d.Name == "pre");

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
				content = content.Replace("METTRE-CODE-ICI", debutCodeBlock + item.InnerHtml + MarkdownSyntax.CODE_END);
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
			content = content.Replace(liFin, "  " + Environment.NewLine);

			return content;
		}

	}
}
