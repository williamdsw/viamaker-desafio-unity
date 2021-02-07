using System;
using System.Collections.Generic;
using System.Text;
using Mono.Data.Sqlite;

public class TurmaService : TurmaRepository
{
    private DatabaseService database = new DatabaseService();

    public bool Insert(Turma turma)
    {
        bool hasInserted = false;

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Turma.Insert, connection))
            {
                command.Parameters.AddWithValue("@nome", turma.Nome);
                command.Parameters.AddWithValue("@escola_id", turma.Escola.Id);
                hasInserted = (command.ExecuteNonQuery() == 1);
            }
        }
      
        return hasInserted;
    }

    public bool InsertMultiples(List<Turma> turmas)
    {
        bool hasInserted = false;

        using (SqliteConnection connection = database.OpenConnection())
        {
            StringBuilder query = new StringBuilder();
            foreach (Turma turma in turmas)
            {
                query.AppendFormat(Queries.Turma.InsertMultiples, turma.Nome, turma.Escola.Id);
            }

            using (SqliteCommand command = new SqliteCommand(query.ToString(), connection))
            {
                hasInserted = (command.ExecuteNonQuery() == turmas.Count);
            }
        }
      
        return hasInserted;
    }

    public bool Update(Turma turma)
    {
        this.FindById(turma.Id);

        bool hasUpdated = false;

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Turma.Update, connection))
            {
                command.Parameters.AddWithValue("@nome", turma.Nome);
                command.Parameters.AddWithValue("@id", turma.Id);
                command.Parameters.AddWithValue("@escola_id", turma.Escola.Id);
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
            using (SqliteCommand command = new SqliteCommand(Queries.Turma.DeleteById, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                hasDeleted = (command.ExecuteNonQuery() == 1);
            }
        }

        return hasDeleted;
    }

    public bool DeleteAllByEscola(int escolaId)
    {
        bool hasDeleted = false;
        int numberOfClasses = this.FindByEscola(escolaId).Count;

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Turma.DeleteAllByEscola, connection))
            {
                command.Parameters.AddWithValue("@escola_id", escolaId);
                hasDeleted = (command.ExecuteNonQuery() == numberOfClasses);
            }
        }

        return hasDeleted;
    }

    public List<Turma> FindByEscola(int escolaId)
    {
        List<Turma> turmas = new List<Turma>();

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Turma.FindByEscola, connection))
            {
                command.Parameters.AddWithValue("@id", escolaId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Turma turma = new Turma();
                        turma.Id = int.Parse(reader["id"].ToString());
                        turma.Nome = reader["nome"].ToString();
                        turmas.Add(turma);
                    }
                }
            }
        }

        return turmas;
    }

    public Turma FindById(int id)
    {
        Turma turma = new Turma();

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Turma.FindById, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new Exception (string.Format("Turma não encontrada para classe: {0}", id));
                    }
                    
                    turma.Id = int.Parse(reader["id"].ToString());
                    turma.Nome = reader["nome"].ToString();
                }
            }
        }

        return turma;
    }
}