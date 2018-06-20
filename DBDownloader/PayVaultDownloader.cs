/*
 * using PlayerIOClient;
using StringDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDownloader {
	class PayVaultDownloader {
		public PayVaultDownloader(string laston, string gameid, Database readPlayers) {
			this._laston = laston;
			this._gameid = gameid;

			this._readPlayers = readPlayers;

			this._fs = File.Open($"PayVault.db", FileMode.OpenOrCreate); ///TODO: custom naming
			this._strdb = Database.FromStream(this._fs);

			//var problem = this._client.BigDB.Load("PlayerObjects", "fb100002099717266");
			//foreach (var i in problem.Properties)
			//	Console.WriteLine(i);

			//foreach (var i in problem.Properties)
			//	Console.WriteLine(problem[i]);

			//Console.ReadLine();
		}

		private Client _client { get; set; }

		private FileStream _fs { get; set; }
		private Database _strdb { get; set; }
		private Database _readPlayers { get; set; }
		private string _laston { get; set; }
		private string _gameid { get; set; }

		private object _lastIndex { get; set; }

		public void Work() {
			foreach(var i in new string[] { _readPlayers.FirstIndex() }) {//_readPlayers) {
				this._client = PlayerIO.Connect(_gameid, "public", i, "", ""); ///TODO: more auth handling across games 'n' stuff
				this._client.PayVault.Refresh();
				string data = Newtonsoft.Json.JsonConvert.SerializeObject( this._client.PayVault.ReadHistory(0, 1231232344) );
				Console.WriteLine(data);
			}
		}
		//dispose of streams and filestreams and the like
		private bool _disposed { get; set; } = false;

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			// Check to see if Dispose has already been called. 
			if (!this._disposed) {
				if (disposing) {
					_fs.Close();
					_fs.Dispose();
				}

				_strdb = null; //we're done w/ u

				// Note disposing has been done.
				this._disposed = true;
			}
		}
	}
}
*/