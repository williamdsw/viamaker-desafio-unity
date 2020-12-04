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
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@nome", turma.Nome);
            command.Parameters.AddWithValue("@escola_id", turma.Escola.Id);
            hasInserted = (command.ExecuteNonQuery() == 1);
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

            SqliteCommand command = new SqliteCommand(query.ToString(), connection);
            hasInserted = (command.ExecuteNonQuery() == turmas.Count);
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
            SqliteCommand command = new SqliteCommand(query.ToString(), connection);
            command.Parameters.AddWithValue("@nome", turma.Nome);
            command.Parameters.AddWithValue("@id", turma.Id);
            command.Parameters.AddWithValue("@escola_id", turma.Escola.Id);
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
            string query = " DELETE FROM turma WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            hasDeleted = (command.ExecuteNonQuery() == 1);
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
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@escola_id", escolaId);
            hasDeleted = (command.ExecuteNonQuery() == numberOfClasses);
        }

        return hasDeleted;
    }

    public List<Turma> FindByEscola(int escolaId)
    {
        List<Turma> turmaes = new List<Turma>();

        using (var connection = database.OpenConnection())
        {
            StringBuilder query = new StringBuilder();
            query.Append(" SELECT id, nome FROM turma ");
            query.Append(" INNER JOIN escola ON (turma.escola_id = escola.id) ");
            query.Append(" WHERE chool_id = @escola_id ");
            query.Append(" ORDER BY id ASC ");
            SqliteCommand command = new SqliteCommand(query.ToString(), connection);
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Turma turma = new Turma();
                turma.Id = int.Parse(reader[0].ToString());
                turma.Nome = reader[1].ToString();
                turmaes.Add(turma);
            }
        }

        return turmaes;
    }

    public Turma FindById(int id)
    {
        Turma turma = new Turma();

        using (var connection = database.OpenConnection())
        {
            string query = " SELECT id, nome FROM turma WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            SqliteDataReader reader = command.ExecuteReader();
            bool exists = reader.Read();
            if (!exists)
            {
                throw new Exception (string.Format("Turma não encontrada para classe: {0}", id));
            }
            
            turma.Id = int.Parse(reader[0].ToString());
            turma.Nome = reader[1].ToString();
        }

        return turma;
    }
}
