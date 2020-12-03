using System.Collections.Generic;

public interface SchoolRepository
{
    bool Insert(School school);
    bool Update(School school);
    bool DeleteById(int id);
    List<School> FindAll();
    School FindById(int id);
}
