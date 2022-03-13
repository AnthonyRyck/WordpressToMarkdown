using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordpressToMarkdown.UnitTests.Tools
{
	internal static class ResultExtension
	{
		public static async Task ToFileOutput(this string result, string pathFile)
		{
			await File.WriteAllTextAsync(pathFile, result);
		}
	}
}
