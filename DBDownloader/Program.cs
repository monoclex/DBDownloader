#define DEBUGGING

using CLAP;
using StringDB;
using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DBDownloader {
	class Program {
		static void Main(string[] args) {
			Parser.Run<DBDownloaderApp>(args);
		}
	}

	class DBDownloaderApp {
		[Verb(IsDefault = true)]
		static void Run(
			[Description("The amount of BigDB entries to download per chunk")]
			[DefaultValue(1000)]
			int chunks,

			[Description("The file to store the 'last-on' data in.")]
			[DefaultValue("last-on.txt")]
			string laston,

			[Description("The database to collect items from")]
			[DefaultValue("PlayerObjects")]
			string dbname,

			[Description("The indexer to be used when downloading each chunk of data from the BigDB")]
			[DefaultValue("name")]
			string dbindexer,

			[Description("The PlayerIO GameID to use when downloading data ( by default, EE's )")]
			[DefaultValue("everybody-edits-su9rn58o40itdbnw69plyw")]
			string gameid,

			[Description("If it should download every world after finishing downloading the players")]
			[DefaultValue(false)]
			bool downloadworlds
			) {
			using (var dw = new DownloadWorker(chunks, laston, dbname, dbindexer, gameid)) {
				dw.Work(); //this will download the players
			}
		}

		[Empty, Help]
		public static void Help(string help) {
			Console.WriteLine(help);
			Console.WriteLine("Example command:\r\nDBDownloader -dbname:PlayerObjects -dbindexer:name -chunks:1000 -laston:last-on.txt -gameid:everybody-edits-su9rn58o40itdbnw69plyw");
		}

		[Error]
		static void ClapError(ExceptionContext c) {
			Console.ForegroundColor = ConsoleColor.Red;

			Console.WriteLine(c.Exception.Message);

			Console.ResetColor();
		}
	}
}
