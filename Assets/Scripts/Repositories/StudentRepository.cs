using System.Collections.Generic;

public interface StudentRepository
{
    bool Insert(Student student);
    bool Update(Student student);
    bool DeleteById(int id);
    bool DeleteAllByClass(int classId);
    List<Student> FindByClass(int classId);
    Student FindById(int id);
}
