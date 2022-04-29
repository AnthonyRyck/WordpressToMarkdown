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
		private const string METTRE_IMG_ICI = "METTRE-TAG-IMG-ICI";

		private static readonly string[] headers = new string[] { "h1", "h2", "h3", "h4", "h5", "h6" };

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
				contentPost = ChangeQuote(contentPost);
				contentPost = ChangeCodeEnLigne(contentPost);
				contentPost = ChangeCodeBlock(contentPost);
				contentPost = ChangeTable(contentPost);
				contentPost = ChangeRetourLigne(contentPost);

				foreach (var userTransform in transformations)
				{
					contentPost = userTransform.Transform(contentPost);
				}

				contentPost = contentPost.Insert(1, MarkdownSyntax.HEADER_1 + WebUtility.HtmlDecode(title) + Environment.NewLine);
				
				return contentPost;
			});
		}

		private string ChangeHeader(string content)
		{
			

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

				content = content.Replace(header.OuterHtml, Environment.NewLine + headerSelected + header.InnerHtml + Environment.NewLine);
			}

			return content;
		}

		private string ChangeRetourLigne(string content)
		{
			content = content.Replace(CHAGEMENT_PARAGRAPHE, string.Empty)
						.Replace("<br>", MarkdownSyntax.SAUT_LIGNE)
						.Replace("<br />", MarkdownSyntax.SAUT_LIGNE)
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
																	&& x.Attributes["class"].Value.Contains("wp-block-image"));
			foreach (var item in lesImages)
			{
				try
				{
					string textCaption = string.Empty;
					// récup des figcaption
					var caption = item.Descendants().FirstOrDefault(x => x.Name == "figcaption");
					if (caption != null)
						textCaption = caption.InnerText;

					// récup du tag img
					var tagImage = item.Descendants().FirstOrDefault(x => x.Name == "img");
					if (tagImage != null)
					{
						string src = tagImage.Attributes["src"].Value;


						string div = "</div>";

						string divImg = @"<div class=""wp-block-image";
						int indexStart = content.IndexOf(divImg);
						int indexEnd = content.IndexOf(div);

						content = content.Remove(indexStart, indexEnd - indexStart + div.Length);
						content = content.Insert(indexStart, METTRE_IMG_ICI);

						if (string.IsNullOrEmpty(textCaption))
						{
							string imageText = Environment.NewLine + MarkdownSyntax.IMAGE_START + src + MarkdownSyntax.IMAGE_END + Environment.NewLine;
							content = content.Replace(METTRE_IMG_ICI, imageText);
						}
						else
						{
							string imgText = Environment.NewLine + MarkdownSyntax.IMAGE_START + src + MarkdownSyntax.IMAGE_END + Environment.NewLine
											+ textCaption + "  " + Environment.NewLine;
							content = content.Replace(METTRE_IMG_ICI, imgText);
						}
					}
				}
				catch (Exception)
				{
					// On laisse le contenu.
				}
			}
			
			// Par Figure
			// <figure class="wp-block-image size-large"><img src="https://raw.githubusercontent.com/AnthonyRyck/ctrl-alt-suppr/main/ImgBlog/GithubAction/CreerPackageNuget/GithubActions_01_Menu.png" alt=""/></figure>
			var imgFigure = htmlDocPost.DocumentNode.Descendants().Where(x => x.Name == "figure"
																	&& x.HasAttributes
																	&& x.Attributes["class"].Value.Contains("wp-block-image"));
			foreach (var item in imgFigure)
			{
				try
				{
					string textCaption = string.Empty;
					// récup des figcaption
					var caption = item.Descendants().FirstOrDefault(x => x.Name == "figcaption");
					if (caption != null)
						textCaption = caption.InnerText;


					// récup du tag img
					var tagImage = item.Descendants().FirstOrDefault(x => x.Name == "img");
					if (tagImage != null)
					{
						string src = tagImage.Attributes["src"].Value;

						if (string.IsNullOrEmpty(textCaption))
							content = content.Replace(item.OuterHtml, Environment.NewLine + MarkdownSyntax.IMAGE_START + src + MarkdownSyntax.IMAGE_END + Environment.NewLine);
						else
							content = content.Replace(item.OuterHtml, Environment.NewLine + MarkdownSyntax.IMAGE_START + src + MarkdownSyntax.IMAGE_END + Environment.NewLine
																		+ textCaption + "  " + Environment.NewLine);
					}
				}
				catch (Exception)
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

					string markDownContent = Environment.NewLine + "Click to open video in new tab  "
											+ $"[![](https://raw.githubusercontent.com/AnthonyRyck/WordpressToMarkdown/main/src/VideoIcon.png)]({videoTag.Attributes["src"].Value})"
											+ Environment.NewLine;

					content = content.Replace(URL_VIDEO, markDownContent);
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
				content = content.Replace("METTRE-CODE-ICI", Environment.NewLine + debutCodeBlock + item.InnerHtml + MarkdownSyntax.CODE_END);
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

		private string ChangeQuote(string content)
		{
			// Exemple :
			// <blockquote class="wp-block-quote">
			var lesQuotes = htmlDocPost.DocumentNode.Descendants().Where(x => x.Name == "blockquote");

			string quoteStart = "<blockquote";
			string quoteEnd = "</blockquote>";

			foreach (var quote in lesQuotes)
			{
				int indexStart = content.IndexOf(quoteStart);
				int indexEnd = content.IndexOf(quoteEnd);
				content = content.Remove(indexStart, indexEnd - indexStart + quoteEnd.Length);

				string sourceCite = string.Empty;
				// récup si source citation
				var cite = quote.Descendants().FirstOrDefault(x => x.Name == "cite");
				if (cite != null)
					sourceCite = cite.InnerText;

				if (string.IsNullOrEmpty(sourceCite))
				{
					content = content.Insert(indexStart, "METTRE-QUOTE-ICI");
				}
				else
				{
					content = content.Insert(indexStart, "METTRE-QUOTE-ICI  "
											+ Environment.NewLine
											+ MarkdownSyntax.ITALIC + MarkdownSyntax.QUOTE_SOURCE + sourceCite + MarkdownSyntax.ITALIC
											+ MarkdownSyntax.SAUT_LIGNE);
				}
				content = content.Replace("METTRE-QUOTE-ICI", MarkdownSyntax.QUOTE + quote.InnerText + Environment.NewLine);
			}

			return content;
		}


		private string ChangeTable(string content)
		{
			// Par Figure : Exemple
			// <figure class="wp-block-table"><table><tbody><tr><td>**GitGuardian has detected the following NuGet API Key exposed within your GitHub account.**</td></tr><tr><td>**Details**  
			// – Secret type: NuGet API Key
			// – Repository: AnthonyRyck / RichLogger
			// – Pushed date: April 13th 2021, 10:10:13 UTC </td></tr></tbody></table></figure >
			var tableFigure = htmlDocPost.DocumentNode.Descendants().Where(x => x.Name == "figure"
																	&& x.HasAttributes
																	&& x.Attributes["class"].Value.Contains("wp-block-table"));
			foreach (var item in tableFigure)
			{
				try
				{
					// récup du tag table
					var table = item.Descendants().FirstOrDefault(x => x.Name == "table");
					// <thead><tr><th>Virtual environment</th><th>YAML workflow label</th></tr></thead><tbody><tr><td>Windows Server 2019</td><td>`windows-latest` or `windows-2019`</td></tr><tr><td>Ubuntu 20.04</td><td>`ubuntu-latest` or `ubuntu-20.04`</td></tr><tr><td>Ubuntu 18.04</td><td>`ubuntu-18.04`</td></tr><tr><td>Ubuntu 16.04</td><td>`ubuntu-16.04`</td></tr><tr><td>macOS Big Sur 11.0</td><td>`macos-11.0`</td></tr><tr><td>macOS Catalina 10.15</td><td>`macos-latest` or `macos-10.15`</td></tr></tbody>
					if (table != null)
					{
						string figure = "</figure>";
						string figureImg = @"<figure class=""wp-block-table";

						int indexStart = content.IndexOf(figureImg);
						int indexEnd = content.IndexOf(figure);

						content = content.Remove(indexStart, indexEnd - indexStart + figure.Length);
						content = content.Insert(indexStart, "METTRE_TABLEAU");

						var intColonne = table.Descendants("th").Count();
						string separationHeaderBody = string.Empty;
						for (int i = 0; i < intColonne; i++)
						{
							if((i+1) == intColonne)
								separationHeaderBody += MarkdownSyntax.SEPARATEUR_FIN_TABLE;
							else
								separationHeaderBody += MarkdownSyntax.SEPARATEUR_TABLE;
						}

						string temp = table.InnerHtml.Replace(@"<thead><tr><th>", string.Empty)
											.Replace("</th><th>", " | ")
											.Replace("</th></tr></thead>", "  " + Environment.NewLine)
											.Replace("<tbody>", separationHeaderBody)
											.Replace("<tr><td>", MarkdownSyntax.LIGNE_DEBUT_DATA_TABLE)
											.Replace("</td><td>", MarkdownSyntax.LIGNE_INTERDATA_TABLE)
											.Replace("</td></tr>", MarkdownSyntax.LIGNE_FIN_DATA_TABLE)
											.Replace("</tbody></table>", MarkdownSyntax.SAUT_LIGNE);

						content = content.Replace("METTRE_TABLEAU", temp);
					}
				}
				catch (Exception)
				{
					// On laisse le contenu.
				}
			}
			return content;
		}

	}
}
