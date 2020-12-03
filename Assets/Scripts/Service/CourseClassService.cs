using System;
using System.Collections.Generic;
using System.Text;
using Mono.Data.Sqlite;

public class CourseClassService : CourseClassRepository
{
    private DatabaseService database = new DatabaseService();

    public bool Insert(CourseClass courseClass)
    {
        bool hasInserted = false;

        using (var connection = database.OpenConnection())
        {
            string query = " INSERT INTO course_class (name, school_id) VALUES (@name, @school_id) ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@name", courseClass.Name);
            command.Parameters.AddWithValue("@school_id", courseClass.School.Id);
            hasInserted = (command.ExecuteNonQuery() == 1);
        }
      
        return hasInserted;
    }

    public bool Update(CourseClass courseClass)
    {
        this.FindById(courseClass.Id);

        bool hasUpdated = false;

        using (var connection = database.OpenConnection())
        {
            StringBuilder query = new StringBuilder();
            query.Append(" UPDATE course_class SET name = @name ");
            query.Append(" WHERE id = @id and school_id = @school_id ");
            SqliteCommand command = new SqliteCommand(query.ToString(), connection);
            command.Parameters.AddWithValue("@name", courseClass.Name);
            command.Parameters.AddWithValue("@id", courseClass.Id);
            command.Parameters.AddWithValue("@school_id", courseClass.School.Id);
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
            string query = " DELETE FROM course_class WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            hasDeleted = (command.ExecuteNonQuery() == 1);
        }

        return hasDeleted;
    }

    public bool DeleteAllBySchool(int schoolId)
    {
        bool hasDeleted = false;
        int numberOfClasses = this.FindBySchool(schoolId).Count;

        using (var connection = database.OpenConnection())
        {
            string query = " DELETE FROM course_class WHERE school_id = @school_id";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@school_id", schoolId);
            hasDeleted = (command.ExecuteNonQuery() == numberOfClasses);
        }

        return hasDeleted;
    }

    public List<CourseClass> FindBySchool(int schoolId)
    {
        List<CourseClass> courseClasses = new List<CourseClass>();

        using (var connection = database.OpenConnection())
        {
            StringBuilder query = new StringBuilder();
            query.Append(" SELECT id, name FROM course_class ");
            query.Append(" INNER JOIN school ON (course_class.school_id = school.id) ");
            query.Append(" WHERE chool_id = @school_id ");
            query.Append(" ORDER BY id ASC ");
            SqliteCommand command = new SqliteCommand(query.ToString(), connection);
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                CourseClass courseClass = new CourseClass();
                courseClass.Id = int.Parse(reader[0].ToString());
                courseClass.Name = reader[1].ToString();
                courseClasses.Add(courseClass);
            }
        }

        return courseClasses;
    }

    public CourseClass FindById(int id)
    {
        CourseClass courseClass = new CourseClass();

        using (var connection = database.OpenConnection())
        {
            string query = " SELECT id, name FROM course_class WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            SqliteDataReader reader = command.ExecuteReader();
            bool exists = reader.Read();
            if (!exists)
            {
                throw new Exception (string.Format("Course Class not found by id {0}", id));
            }
            
            courseClass.Id = int.Parse(reader[0].ToString());
            courseClass.Name = reader[1].ToString();
        }

        return courseClass;
    }
}
