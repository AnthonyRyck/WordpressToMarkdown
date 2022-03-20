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
			
			var result = await converter.ConvertToMarkdownAsync("TEST", contentSample);

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


		[Fact]
		public async void ConversionTableauToMarkdown()
		{
			#region Arrange

			string tableau = @"<div><figure class=""wp-block-table""><table><thead><tr><th>Virtual environment</th><th>YAML workflow label</th></tr></thead><tbody><tr><td>Windows Server 2019</td><td>`windows-latest` or `windows-2019`</td></tr><tr><td>Ubuntu 20.04</td><td>`ubuntu-latest` or `ubuntu-20.04`</td></tr><tr><td>Ubuntu 18.04</td><td>`ubuntu-18.04`</td></tr><tr><td>Ubuntu 16.04</td><td>`ubuntu-16.04`</td></tr><tr><td>macOS Big Sur 11.0</td><td>`macos-11.0`</td></tr><tr><td>macOS Catalina 10.15</td><td>`macos-latest` or `macos-10.15`</td></tr></tbody></table></figure></div>";

			#endregion

			#region Act

			ConverterWordpress converter = new ConverterWordpress();
			var result = await converter.ConvertToMarkdownAsync("TEST", tableau);

			#endregion

			#region Assert

			Assert.True(true);

			#endregion

		}


	}
}