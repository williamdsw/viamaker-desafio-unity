using System;
using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;

public class DatabaseService
{
    public SqliteConnection OpenConnection()
    {
        SqliteConnection connection = new SqliteConnection(Environment.databaseUrl);
        connection.Open();
        return connection;
    }

    public void CreateTable(string query)
    {
        if (string.IsNullOrEmpty (query) || string.IsNullOrWhiteSpace(query))
        {
            throw new Exception ("Query para criação de tabela não pode ser vazia!");
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
