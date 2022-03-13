using Xunit;
using System;
using System.IO;

namespace WordpressToMarkdown.UnitTests
{
	public class ConverterWordpressTest
	{
		private string _sampleJson;
		private const string NAME_SAMPLE_FILE = "sampleCtrlAltSuppr.json";

		public ConverterWordpressTest()
		{
			var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NAME_SAMPLE_FILE);
			_sampleJson = File.ReadAllText(file);
		}

		[Fact]
		public void Test1()
		{
			
		}
	}
}