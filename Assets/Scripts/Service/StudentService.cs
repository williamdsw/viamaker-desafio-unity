using System;
using System.Collections.Generic;
using System.Text;
using Mono.Data.Sqlite;

public class StudentService : StudentRepository
{
    private DatabaseService database = new DatabaseService();

    public bool Insert(Student student)
    {
        bool hasInserted = false;

        using (var connection = database.OpenConnection())
        {
            string query = " INSERT INTO student (name, class_id) VALUES (@name, @class_id) ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@name", student.Name);
            command.Parameters.AddWithValue("@class_id", student.CourseClass.Id);
            hasInserted = (command.ExecuteNonQuery() == 1);
        }
      
        return hasInserted;
    }

    public bool Update(Student student)
    {
        this.FindById(student.Id);

        bool hasUpdated = false;

        using (var connection = database.OpenConnection())
        {
            StringBuilder query = new StringBuilder();
            query.Append(" UPDATE student SET name = @name ");
            query.Append(" WHERE id = @id and class_id = @class_id ");
            SqliteCommand command = new SqliteCommand(query.ToString(), connection);
            command.Parameters.AddWithValue("@name", student.Name);
            command.Parameters.AddWithValue("@id", student.Id);
            command.Parameters.AddWithValue("@class_id", student.CourseClass.Id);
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
            string query = " DELETE FROM student WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            hasDeleted = (command.ExecuteNonQuery() == 1);
        }

        return hasDeleted;
    }

    public bool DeleteAllByClass(int classId)
    {
        bool hasDeleted = false;
        int numberOfStudents = this.FindByClass(classId).Count;

        using (var connection = database.OpenConnection())
        {
            string query = " DELETE FROM student WHERE class_id = @class_id";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@class_id", classId);
            hasDeleted = (command.ExecuteNonQuery() == numberOfStudents);
        }

        return hasDeleted;
    }

    public List<Student> FindByClass(int classId)
    {
        List<Student> students = new List<Student>();

        using (var connection = database.OpenConnection())
        {
            StringBuilder query = new StringBuilder();
            query.Append(" SELECT id, name FROM student ");
            query.Append(" INNER JOIN course_class ON (student.class_id = course_class.id) ");
            query.Append(" WHERE class_id = @class_id ");
            query.Append(" ORDER BY name ASC ");
            SqliteCommand command = new SqliteCommand(query.ToString(), connection);
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Student student = new Student();
                student.Id = int.Parse(reader[0].ToString());
                student.Name = reader[1].ToString();
                students.Add(student);
            }
        }

        return students;
    }

    public Student FindById(int id)
    {
        Student student = new Student();

        using (var connection = database.OpenConnection())
        {
            string query = " SELECT id, name FROM student WHERE id = @id ";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            SqliteDataReader reader = command.ExecuteReader();
            bool exists = reader.Read();
            if (!exists)
            {
                throw new Exception (string.Format("Student not found by id {0}", id));
            }
            
            student.Id = int.Parse(reader[0].ToString());
            student.Name = reader[1].ToString();
        }

        return student;
    }
}
