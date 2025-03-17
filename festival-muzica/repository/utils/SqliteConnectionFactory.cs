using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using repository.utils;

namespace repository.utils
{
	public class SqliteConnectionFactory : ConnectionFactory
	{
		public override IDbConnection createConnection(IDictionary<string,string> props)
		{
			
			String connectionString = props["ConnectionString"];
			Console.WriteLine("SQLite ---Se deschide o conexiune la  ... {0}", connectionString);
			return new SQLiteConnection(connectionString);

			// Windows SQLite Connection, fisierul .db ar trebuie sa fie in directorul bin/debug
			//String connectionString = "Data Source=tasks.db;Version=3";
			//return new SQLiteConnection(connectionString);
		}
	}
}
