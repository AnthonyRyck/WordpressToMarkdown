﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordpressToMarkdown.Models
{
	internal static class MarkdownSyntax
	{
		/// <summary>
		/// Correspond à un <hx></hx>
		/// </summary>
		public const string HEADER_1 = "# ";
		public const string HEADER_2 = "## ";
		public const string HEADER_3 = "### ";
		public const string HEADER_4 = "#### ";
		public const string HEADER_5 = "##### ";
		public const string HEADER_6 = "###### ";

		/// <summary>
		/// Correspond à un <br>
		/// </summary>
		public const string SAUT_LIGNE = ("  \r\n");

		/// <summary>
		/// Correspond au <strong>
		/// </summary>
		public const string BOLD = "**";

		/// <summary>
		/// Correspond au <em>
		/// </summary>
		public const string ITALIC = "*";

		/// <summary>
		/// Correspond au <strong><em>
		/// </summary>
		public const string ITALIC_BOLD = "***";

		/// <summary>
		/// Pour faire le système de point <ul><li>
		/// </summary>
		public const string START_UNORDORED_LIST = "* ";

		/// <summary>
		/// Correspond à <code> ou <pre>
		/// </summary>
		public const string CODE_START = "```";

		public const string CODE_END = "```\r\n";

		public const string CODE_IN_LINE = "`";

		public static string Image(string name, string url)
		{
			return "[" + name + "](" + url + ")";
		}

		// A voir pour les vidéos
		// Exemple :
		// [![Blinking LEDs] (http://img.youtube.com/vi/XAMVzS13HY0/0.jpg)](http://www.youtube.com/watch?v=XAMVzS13HY0 "Blinking LEDs")
		// [![Alternate Text]({image-url})]({video-url} "Link Title")

	}
}