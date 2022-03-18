using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WordpressToMarkdown.Models;

namespace WordpressToMarkdown
{
	public class WordpressCollector
	{
		private const string URL_POST = "/wp-json/wp/v2/posts?slug=";

		public async Task GetOnePost(string urlPost)
		{
			try
			{
				List<PostWordPress> post = new List<PostWordPress>();
				var url_decompose = urlPost.Split('/', StringSplitOptions.RemoveEmptyEntries);
				string titrePost = url_decompose.Last();
				string website = url_decompose[1];

				string url = "https://" + website + URL_POST + titrePost;

				using (var client = new HttpClient())
				{
					var streamPosts = await client.GetStreamAsync(url);
					post = await JsonSerializer.DeserializeAsync<List<PostWordPress>>(streamPosts);
				}

				if (post.Count == 1)
				{
					ConverterWordpress converter = new ConverterWordpress();
					string contentMD = await converter.ConvertToMarkdownAsync(post[0].content.rendered);
				}
			}
			catch (Exception ex)
			{

			}
		}

		public async Task<List<MarkdownResult>> GetPosts(string url)
		{
			List<PostWordPress> posts = new List<PostWordPress>();
			List<MarkdownResult> resultsMd = new List<MarkdownResult>();
			try
			{
				using (var client = new HttpClient())
				{
					var streamPosts = await client.GetStreamAsync(url);
					posts = await JsonSerializer.DeserializeAsync<List<PostWordPress>>(streamPosts);
				}

				ConverterWordpress converter = new ConverterWordpress();
				foreach (var post in posts)
				{
					string contentMD = await converter.ConvertToMarkdownAsync(post.content.rendered);

					var result = new MarkdownResult();
					result.MarkdownContent = contentMD;
					result.Titre = post.slug;
					result.ContentBrut = post.content.rendered;
					resultsMd.Add(result);
				}
			}
			catch (Exception ex)
			{

				throw;
			}

			return resultsMd;
		}
	}
}
