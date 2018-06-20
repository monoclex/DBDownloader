using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDownloader {
	public class SerializedDBO {
		public SerializedDBO(DatabaseObject dbo) {
			this.Data = new Dictionary<string, object>();

			if (dbo == null)
				return;

			this.Table = dbo.Table;
			this.Key = dbo.Key;

			foreach (var i in dbo)
				try {
					if (dbo.TryGetValue(i.Key, out var v)) {

						if (v is DatabaseObject)
							this.Data[i.Key] = new SerializedDBO((DatabaseObject)v).Data; //we only need the data portion of it - we don't need the Table KEy e.t.c features of it
						else if (v is DatabaseArray)
							this.Data[i.Key] = new SerializedDBA((DatabaseArray)v).Data;
						else this.Data[i.Key] = v;
					}
				} catch(Exception e) {
					// this is only because "fb100002099717266" has "crap0" that PlayerIOClient can't handle and throws a NullReferenceException on.
					Console.WriteLine($"Error while serializing {i.Key} for {dbo.Key}: {e.Message}");
				}
		}

		public string Table { get; set; } = null;
		public string Key { get; set; } = null;
		public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
	}

	public class SerializedDBA {
		public SerializedDBA(DatabaseArray dba) {
			this.Data = new Dictionary<int, object>();

			if (dba == null)
				return;

			foreach (var j in dba.Indexes) {
					if (dba.TryGetValue(j.ToString(), out var i)) {
						if (i == null)
							Console.WriteLine("null val");

						if (i is DatabaseObject)
							this.Data[j] = new SerializedDBO((DatabaseObject)i).Data;
						else if (i is DatabaseArray)
							this.Data[j] = new SerializedDBA((DatabaseArray)i).Data;
						else this.Data[j] = i;
					}
			}
		}

		public Dictionary<int, object> Data { get; set; } = new Dictionary<int, object>();
	}
}
