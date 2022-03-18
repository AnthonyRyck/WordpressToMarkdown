using Xunit;
using System;
using System.IO;
using System.Collections.Generic;
using WordpressToMarkdown.Models;
using System.Text.Json;

namespace WordpressToMarkdown.UnitTests
{
	public class ConverterWordpressTest
	{
		private Stream _sampleJson;
		private const string NAME_SAMPLE_FILE = "sampleCtrlAltSuppr.json";
		private const string SAMPLE_CODE_PLUS_IMG = "SampleCodePlusImg.txt";

		public ConverterWordpressTest()
		{
			var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NAME_SAMPLE_FILE);
			_sampleJson = File.OpenRead(file);
		}


		[Fact]
		public async void Test()
		{
			#region Arrange

			var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ContentToTest.txt");
			var contentSample = File.ReadAllText(file);
			
			ConverterWordpress converter = new ConverterWordpress();

			#endregion

			#region Act
			
			var result = converter.ConvertToMarkdownAsync(contentSample);

			#endregion

			#region Assert

			var stop = true;

			#endregion
		}


		[Fact]
		public async void TestCodeConversion()
		{
			#region Arrange

			string urlPost = "https://www.ctrl-alt-suppr.dev/2022/03/08/passer-du-c-au-typescript/";

			string urlAllPosts = "https://www.ctrl-alt-suppr.dev/wp-json/wp/v2/posts";

			#endregion

			#region Act

			WordpressCollector collector = new WordpressCollector();
			//await collector.GetOnePost(urlPost);
			await collector.GetPosts(urlAllPosts);

			#endregion

			#region Assert



			#endregion

		}

	}
}