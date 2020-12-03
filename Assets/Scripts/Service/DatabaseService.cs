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

    public bool CreateTable(string query)
    {
        bool hasCreated = false;

        if (string.IsNullOrEmpty (query) || string.IsNullOrWhiteSpace(query))
        {
            throw new Exception ("Create table query cannot be null or empty!");
        }

        using (SqliteConnection connection = this.OpenConnection())
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = query;
            hasCreated = (command.ExecuteNonQuery() == 1);
        }

        return hasCreated;
    }
}
