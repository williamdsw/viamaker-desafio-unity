
public class CourseClass
{
    public int Id { get; set; }
    public string Name { get; set; }
    public School School { get; set; }

    public CourseClass() {}

    public CourseClass(int id, string name, School school)
    {
        Id = id;
        Name = name;
        School = school;
    }
}
