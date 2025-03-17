using System;
using System.IO;
using System.Data.SQLite;

namespace festival_muzica
{
    public class CreateDatabase
    {
        public static void CreateTables()
        {
            string dbPath = "C:\\Users\\Ivona\\Facultate\\Baze-date\\MPP\\festival-csharp.db";
            string scriptPath = "create_tables.sql";

            // Create database if it doesn't exist
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            // Read and execute the SQL script
            string script = File.ReadAllText(scriptPath);
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                using (var command = new SQLiteCommand(script, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Tables created successfully!");
                }
            }
        }
    }
} 