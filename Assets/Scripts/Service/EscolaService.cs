using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class EscolaService : EscolaRepository
{
    private DatabaseService database = new DatabaseService();

    public bool Insert(Escola escola)
    {
        bool hasInserted = false;

        using (var connection = database.OpenConnection())
        {
            string query = " INSERT INTO escola (nome) VALUES (@nome) ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@nome", escola.Nome);
            hasInserted = (command.ExecuteNonQuery() == 1);
        }
      
        return hasInserted;
    }

    public bool Update(Escola escola)
    {
        this.FindById(escola.Id);

        bool hasUpdated = false;

        using (var connection = database.OpenConnection())
        {
            string query = " UPDATE escola SET nome = @nome WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@nome", escola.Nome);
            command.Parameters.AddWithValue("@id", escola.Id);
            hasUpdated = (command.ExecuteNonQuery() == 1);
        }

        return hasUpdated;
    }

    public bool DeleteById(int id)
    {
        this.FindById(id);

        bool hasDeleted = false;

        using (var connection = database.OpenConnection())
        {
            string query = " DELETE FROM escola WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            hasDeleted = (command.ExecuteNonQuery() == 1);
        }

        return hasDeleted;
    }

    public List<Escola> FindAll()
    {
        List<Escola> escolas = new List<Escola>();

        using (var connection = database.OpenConnection())
        {
            string query = " SELECT id, nome FROM escola ORDER BY nome ASC ";
            SqliteCommand command = new SqliteCommand(query, connection);
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Escola escola = new Escola();
                escola.Id = int.Parse(reader[0].ToString());
                escola.Nome = reader[1].ToString();
                escolas.Add(escola);
            }
        }

        return escolas;
    }

    public Escola FindById(int id)
    {
        Escola escola = new Escola();

        using (var connection = database.OpenConnection())
        {
            string query = " SELECT id, nome FROM escola WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            SqliteDataReader reader = command.ExecuteReader();
            bool exists = reader.Read();
            if (!exists)
            {
                throw new Exception (string.Format("Escola não encontrada pelo id: {0}", id));
            }
            
            escola.Id = int.Parse(reader[0].ToString());
            escola.Nome = reader[1].ToString();
        }

        return escola;
    }
}
