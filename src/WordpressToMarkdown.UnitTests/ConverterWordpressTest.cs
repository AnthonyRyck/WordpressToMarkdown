using Xunit;
using System;
using System.IO;

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
		public async void TestConvertToObject()
		{
			#region Arrange

			ConverterWordpress converter = new ConverterWordpress();

			#endregion

			#region Act

			var result = await converter.ConvertToObject(_sampleJson);

			#endregion

			#region Assert

			Assert.True(result.Count == 1);

			#endregion
		}


		[Fact]
		public async void Test()
		{
			#region Arrange

			var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SAMPLE_CODE_PLUS_IMG);
			string contentSample = File.ReadAllText(file);

			ConverterWordpress converter = new ConverterWordpress();

			#endregion

			#region Act

			var result = converter.ConvertToMarkdown(contentSample);

			#endregion

			#region Assert

			var stop = true;

			#endregion
		}
	}
}