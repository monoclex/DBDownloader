using PlayerIOClient;
using StringDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDownloader {
	public class DownloadWorker : IDisposable {
		public DownloadWorker(int chunks, string laston, string dbname, string dbindex, string gameid) {
			this._chunks = chunks;
			this._laston = laston;
			this._dbname = dbname;
			this._dbindex = dbindex;
			this._gameid = gameid;
			
			this._strdb = Database.FromFile($"{dbname}.db"); ///TODO: custom naming

			this._client = PlayerIO.Connect(gameid, "public", "user", "", ""); ///TODO: more auth handling across games 'n' stuff

			//var problem = new SerializedDBO(this._client.BigDB.Load("PlayerObjects", "fb100002099717266"));
		}

		private Client _client { get; set; }
		
		private Database _strdb { get; set; }
		private int _chunks { get; set; }
		private string _laston { get; set; }
		private string _dbname { get; set; }
		private string _dbindex { get; set; }
		private string _gameid { get; set; }

		private object _lastIndex { get; set; }

		public void Work() {
			LoadItemOn(); //load the item

			if (this._lastIndex == null) //if we're the first one, we wanna save the first chunk
				SaveArray(GetChunk(this._lastIndex));

			DatabaseObject[] dbo; //from then on we'll get the truncated version
			while((dbo = GetTruncatedChunk(this._lastIndex)).Length > 1) { //as long as there's more then 1 item
				WorkOn(dbo);
			}
		}

		public void WorkOn(DatabaseObject[] items) {
			SaveArray(items); //save the data
			
			File.WriteAllText(this._laston, Newtonsoft.Json.JsonConvert.SerializeObject(this._lastIndex)); //every SaveArray we set the last index
		}

		public object LoadItemOn() {
			this._lastIndex = null;

			if (File.Exists(this._laston))
				this._lastIndex = Newtonsoft.Json.JsonConvert.DeserializeObject<object>( File.ReadAllText(this._laston) );

			return this._lastIndex;
		}

		public void SaveArray(DatabaseObject[] dbo) {
			this._lastIndex = dbo[dbo.Length - 1][this._dbindex]; //store the next last index
			Console.Write($"{this._lastIndex.ToString()} "); //progress showing to make us feel better

			//iterate over each one and save it

			var arrData = new List<KeyValuePair<string, string>>(dbo.Length);

			for (uint i = 0; i < dbo.Length; i++)
				if(dbo[i] != null)
					arrData.Add(
						new KeyValuePair<string, string>(
							dbo[i].Key,
								Newtonsoft.Json.JsonConvert.SerializeObject(
									new SerializedDBO(dbo[i])
								)
							)
						);

			_strdb.InsertRange(arrData.ToArray());
		}

		//NOTE WHEN DOWNLOADING MULTIPLE CHUNKS:
		//if start at 'A' and download until 'T', then use 'T' as the start, you're going to get 'T' again.
		//so when you get the next chunk, terminate the first result you get.
		internal DatabaseObject[] GetChunk(object start) {
			try {
				return _client.BigDB.LoadRange(this._dbname, this._dbindex, null, start, null, _chunks);
			} catch(Exception e) { //timed out or something - let's wait a little and re-call the function
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine(e.Message);
				Console.ResetColor();
				System.Threading.Thread.Sleep(10000);
				return GetChunk(start);
			}
		}

		internal DatabaseObject[] TruncateFirst(DatabaseObject[] arr) => arr.Skip(1).ToArray();
		internal DatabaseObject[] GetTruncatedChunk(object start) => TruncateFirst(GetChunk(start));

		//dispose of streams and filestreams and the like
		private bool _disposed { get; set; } = false;

		public void Dispose() {
			Dispose(true);
		}
		
		protected virtual void Dispose(bool disposing) {
			// Check to see if Dispose has already been called. 
			if (!this._disposed) {
				if (disposing) {
					_strdb.Dispose();
				}
				
				_strdb = null; //we're done w/ u

				// Note disposing has been done.
				this._disposed = true;
			}
		}
	}
}
