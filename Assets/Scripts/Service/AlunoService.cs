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
            string query = " INSERT INTO aluno (nome, turma_id) VALUES (@nome, @turma_id) ";
            using (SqliteCommand command = new SqliteCommand(query, connection))
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
            foreach (var aluno in alunos)
            {
                string format = " INSERT INTO aluno (nome, turma_id) VALUES (\'{0}\', {1}); ";
                query.AppendFormat(format, aluno.Nome, aluno.Turma.Id);
            }

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
            StringBuilder query = new StringBuilder();
            query.Append(" UPDATE aluno SET nome = @nome ");
            query.Append(" WHERE id = @id AND turma_id = @turma_id ");

            using (SqliteCommand command = new SqliteCommand(query.ToString(), connection))
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
            string query = " DELETE FROM aluno WHERE id = @id ";
            using (SqliteCommand command = new SqliteCommand(query, connection))
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
            string query = " DELETE FROM aluno WHERE turma_id = @turma_id";
            using (SqliteCommand command = new SqliteCommand(query, connection))
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
            StringBuilder query = new StringBuilder();
            query.Append(" SELECT aluno.id, aluno.nome FROM aluno AS aluno ");
            query.Append(" INNER JOIN turma AS turma ON (aluno.turma_id = turma.id) ");
            query.Append(" WHERE turma.id = @turma_id ");
            query.Append(" ORDER BY aluno.nome ASC ");

            using (SqliteCommand command = new SqliteCommand(query.ToString(), connection))
            {
                command.Parameters.AddWithValue("@turma_id", turmaId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Aluno aluno = new Aluno();
                        aluno.Id = int.Parse(reader[0].ToString());
                        aluno.Nome = reader[1].ToString();
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
            string query = " SELECT id, nome FROM aluno WHERE id = @id ";
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new Exception (string.Format("Aluno não encontrado pelo id: {0}", id));
                    }
                    
                    aluno.Id = int.Parse(reader[0].ToString());
                    aluno.Nome = reader[1].ToString();
                }
            }
        }

        return aluno;
    }
}
