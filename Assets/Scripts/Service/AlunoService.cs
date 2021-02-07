using System;
using System.Collections.Generic;
using System.Text;
using Mono.Data.Sqlite;

public class AlunoService : AlunoRepository
{
    private DatabaseService database = new DatabaseService();

    public bool Insert(Aluno aluno)
    {
        bool hasInserted = false;

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Aluno.Insert, connection))
            {
                command.Parameters.AddWithValue("@nome", aluno.Nome);
                command.Parameters.AddWithValue("@turma_id", aluno.Turma.Id);
                hasInserted = (command.ExecuteNonQuery() == 1);
            }
        }
      
        return hasInserted;
    }

    public bool InsertMultiples(List<Aluno> alunos)
    {
        bool hasInserted = false;

        using (SqliteConnection connection = database.OpenConnection())
        {
            StringBuilder query = new StringBuilder();
            alunos.ForEach(aluno => query.AppendFormat(Queries.Aluno.InsertMultiples, aluno.Nome, aluno.Turma.Id));
            using (SqliteCommand command = new SqliteCommand(query.ToString(), connection))
            {
                hasInserted = (command.ExecuteNonQuery() == alunos.Count);
            }
        }
      
        return hasInserted;
    }

    public bool Update(Aluno aluno)
    {
        this.FindById(aluno.Id);

        bool hasUpdated = false;

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Aluno.Update, connection))
            {
                command.Parameters.AddWithValue("@nome", aluno.Nome);
                command.Parameters.AddWithValue("@id", aluno.Id);
                command.Parameters.AddWithValue("@turma_id", aluno.Turma.Id);
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
            using (SqliteCommand command = new SqliteCommand(Queries.Aluno.Delete, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                hasDeleted = (command.ExecuteNonQuery() == 1);
            }
        }

        return hasDeleted;
    }

    public bool DeleteAllByTurma(int turmaId)
    {
        bool hasDeleted = false;
        int count = this.FindByTurma(turmaId).Count;

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Aluno.DeleteAllByTurma, connection))
            {
                command.Parameters.AddWithValue("@turma_id", turmaId);
                hasDeleted = (command.ExecuteNonQuery() == count);
            }
        }

        return hasDeleted;
    }

    public List<Aluno> FindByTurma(int turmaId)
    {
        List<Aluno> alunos = new List<Aluno>();

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Aluno.FindByTurma, connection))
            {
                command.Parameters.AddWithValue("@turma_id", turmaId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Aluno aluno = new Aluno();
                        aluno.Id = int.Parse(reader["id"].ToString());
                        aluno.Nome = reader["nome"].ToString();
                        alunos.Add(aluno);
                    }
                }
            }
        }

        return alunos;
    }

    public Aluno FindById(int id)
    {
        Aluno aluno = new Aluno();

        using (SqliteConnection connection = database.OpenConnection())
        {
            using (SqliteCommand command = new SqliteCommand(Queries.Aluno.FindById, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new Exception (string.Format("Aluno não encontrado pelo id: {0}", id));
                    }
                    
                    aluno.Id = int.Parse(reader["id"].ToString());
                    aluno.Nome = reader["nome"].ToString();
                }
            }
        }

        return aluno;
    }
}