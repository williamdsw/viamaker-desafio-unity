
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public CourseClass CourseClass { get; set; }

    public Student() {}
    public Student(int id, string name, CourseClass courseClass)
    {
        Id = id;
        Name = name;
        CourseClass = courseClass;
    }
}
