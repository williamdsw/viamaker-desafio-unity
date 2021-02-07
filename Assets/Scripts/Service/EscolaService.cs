using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class EscolaService : EscolaRepository
{
    private DatabaseService database = new DatabaseService();

    public bool Insert(Escola escola)
    {
        bool hasInserted = false;

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Escola.Insert, connection))
            {
                command.Parameters.AddWithValue("@nome", escola.Nome);
                hasInserted = (command.ExecuteNonQuery() == 1);
            }
        }

        return hasInserted;
    }

    public bool Update(Escola escola)
    {
        this.FindById(escola.Id);

        bool hasUpdated = false;

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Escola.Update, connection))
            {
                command.Parameters.AddWithValue("@nome", escola.Nome);
                command.Parameters.AddWithValue("@id", escola.Id);
                hasUpdated = (command.ExecuteNonQuery() == 1);
            }
        }

        return hasUpdated;
    }

    public bool DeleteById(int id)
    {
        this.FindById(id);

        bool hasDeleted = false;

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Escola.DeleteById, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                hasDeleted = (command.ExecuteNonQuery() == 1);
            }
        }

        return hasDeleted;
    }

    public List<Escola> FindAll()
    {
        List<Escola> escolas = new List<Escola>();

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Escola.FindAll, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Escola escola = new Escola();
                        escola.Id = int.Parse(reader["id"].ToString());
                        escola.Nome = reader["nome"].ToString();
                        escolas.Add(escola);
                    }
                }
            }
        }

        return escolas;
    }

    public Escola FindById(int id)
    {
        Escola escola = new Escola();

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Escola.FindById, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new Exception(string.Format("Escola não encontrada pelo id: {0}", id));
                    }

                    escola.Id = int.Parse(reader["id"].ToString());
                    escola.Nome = reader["nome"].ToString();
                }
            }
        }

        return escola;
    }
}