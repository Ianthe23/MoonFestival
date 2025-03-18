using System;
using System.IO;
using System.Data.SQLite;

namespace festival_muzica
{
    public class CreateDatabase
    {
        private static void DisplayTable(SQLiteConnection connection, string tableName)
        {
            Console.WriteLine($"\nContents of {tableName} table:");
            using (var command = new SQLiteCommand($"SELECT * FROM {tableName}", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    // Print column names
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write($"{reader.GetName(i)} | ");
                    }
                    Console.WriteLine("\n" + new string('-', 50));

                    // Print data
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader[i]} | ");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        public static void CreateTables()
        {
            string dbPath = "C:\\Users\\Ivona\\Facultate\\Baze-date\\MPP\\festival-csharp.db";
            string scriptPath = "sql/create_tables.sql";

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

                // Display contents of each table
                DisplayTable(connection, "Employee");
                DisplayTable(connection, "Show");
                DisplayTable(connection, "Client");
                DisplayTable(connection, "Ticket");
            }
        }
    }
} 