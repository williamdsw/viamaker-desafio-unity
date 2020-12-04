using System;
using System.Collections.Generic;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;

public class TurmaService : TurmaRepository
{
    private DatabaseService database = new DatabaseService();

    public bool Insert(Turma turma)
    {
        bool hasInserted = false;

        using (var connection = database.OpenConnection())
        {
            string query = " INSERT INTO turma (nome, escola_id) VALUES (@nome, @escola_id) ";
            using (SqliteCommand command = new SqliteCommand(query, connection))
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

        using (var connection = database.OpenConnection())
        {
            StringBuilder query = new StringBuilder();
            foreach (var turma in turmas)
            {
                string format = " INSERT INTO turma (nome, escola_id) VALUES (\'{0}\', {1}); ";
                query.AppendFormat(format, turma.Nome, turma.Escola.Id);
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

        using (var connection = database.OpenConnection())
        {
            StringBuilder query = new StringBuilder();
            query.Append(" UPDATE turma SET nome = @nome ");
            query.Append(" WHERE id = @id AND escola_id = @escola_id ");

            using (SqliteCommand command = new SqliteCommand(query.ToString(), connection))
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

        using (var connection = database.OpenConnection())
        {
            string query = " DELETE FROM turma WHERE id = @id ";
            using (SqliteCommand command = new SqliteCommand(query, connection))
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

        using (var connection = database.OpenConnection())
        {
            string query = " DELETE FROM turma WHERE escola_id = @escola_id";
            using (SqliteCommand command = new SqliteCommand(query, connection))
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

        using (var connection = database.OpenConnection())
        {
            StringBuilder query = new StringBuilder();
            query.Append(" SELECT turma.id, turma.nome FROM turma AS turma ");
            query.Append(" INNER JOIN escola AS escola ON (turma.escola_id = escola.id) ");
            query.Append(" WHERE escola.id = @id ");
            query.Append(" ORDER BY turma.id ASC ");

            using (SqliteCommand command = new SqliteCommand(query.ToString(), connection))
            {
                command.Parameters.AddWithValue("@id", escolaId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Turma turma = new Turma();
                        turma.Id = int.Parse(reader[0].ToString());
                        turma.Nome = reader[1].ToString();
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

        using (var connection = database.OpenConnection())
        {
            string query = " SELECT id, nome FROM turma WHERE id = @id ";
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new Exception (string.Format("Turma não encontrada para classe: {0}", id));
                    }
                    
                    turma.Id = int.Parse(reader[0].ToString());
                    turma.Nome = reader[1].ToString();
                }
            }
        }

        return turma;
    }
}
