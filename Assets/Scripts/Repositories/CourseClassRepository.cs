using System.Collections.Generic;

public interface CourseClassRepository
{
    bool Insert(CourseClass courseClass);
    bool Update(CourseClass courseClass);
    bool DeleteById(int id);
    bool DeleteAllBySchool(int schoolId);
    List<CourseClass> FindBySchool(int schoolId);
    CourseClass FindById(int id);
}