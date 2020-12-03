using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class SchoolService : SchoolRepository
{
    private DatabaseService database = new DatabaseService();

    public bool Insert(School school)
    {
        bool hasInserted = false;

        using (var connection = database.OpenConnection())
        {
            string query = " INSERT INTO school (name) VALUES (@name) ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@name", school.Name);
            hasInserted = (command.ExecuteNonQuery() == 1);
        }
      
        return hasInserted;
    }

    public bool Update(School school)
    {
        this.FindById(school.Id);

        bool hasUpdated = false;

        using (var connection = database.OpenConnection())
        {
            string query = " UPDATE school SET name = @name WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@name", school.Name);
            command.Parameters.AddWithValue("@id", school.Id);
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
            string query = " DELETE FROM school WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            hasDeleted = (command.ExecuteNonQuery() == 1);
        }

        return hasDeleted;
    }

    public List<School> FindAll()
    {
        List<School> schools = new List<School>();

        using (var connection = database.OpenConnection())
        {
            string query = " SELECT id, name FROM school ORDER BY name ASC ";
            SqliteCommand command = new SqliteCommand(query, connection);
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                School school = new School();
                school.Id = int.Parse(reader[0].ToString());
                school.Name = reader[1].ToString();
                schools.Add(school);
            }
        }

        return schools;
    }

    public School FindById(int id)
    {
        School school = new School();

        using (var connection = database.OpenConnection())
        {
            string query = " SELECT id, name FROM school WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            SqliteDataReader reader = command.ExecuteReader();
            bool exists = reader.Read();
            if (!exists)
            {
                throw new Exception (string.Format("School not found by id {0}", id));
            }
            
            school.Id = int.Parse(reader[0].ToString());
            school.Name = reader[1].ToString();
        }

        return school;
    }
}
