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
	}
}
