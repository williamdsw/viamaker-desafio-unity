using Mono.Data.Sqlite;
using System;
using System.Data;

namespace Service
{
    public class DatabaseService
    {
        public SqliteConnection OpenConnection()
        {
            SqliteConnection connection = new SqliteConnection(Config.Environment.DatabaseUrl);
            connection.Open();
            return connection;
        }

        public void CreateTable(string query)
        {
            if (string.IsNullOrEmpty(query) || string.IsNullOrWhiteSpace(query))
            {
                throw new Exception("Query para criação de tabela não pode ser vazia!");
            }

            using (SqliteConnection connection = this.OpenConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}