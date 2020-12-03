
public class Constants
{
    public const string DATABASE_URL = "URI=file:{0}/viamaker";

    public static string CreateTableSchool 
    {
        get 
        {
            return " CREATE TABLE IF NOT EXISTS school ( id INTEGER PRIMARY KEY AUTOINCREMENT,  name TEXT NOT NULL ) ";
        }
    }

    public static string CreateTableCourseClass 
    {
        get 
        {
            return " CREATE TABLE IF NOT EXISTS course_class " + 
                   " ( id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, school_id INTEGER NOT NULL, " + 
                   " FOREIGN KEY (school_id) REFERENCES school (id) ) ";
        }
    }

    public static string CreateTableStudent 
    {
        get 
        {
            return " CREATE TABLE IF NOT EXISTS student " + 
                   " ( id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, class_id INTEGER NOT NULL, " + 
                   " FOREIGN KEY (class_id) REFERENCES course_class (id) ) ";
        }
    }

}
