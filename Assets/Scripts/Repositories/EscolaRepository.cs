using Model;
using System.Collections.Generic;

namespace Repository
{
    public interface EscolaRepository
    {
        bool Insert(Escola escola);
        bool Update(Escola escola);
        bool DeleteById(int id);
        List<Escola> FindAll();
        Escola FindById(int id);
    }
}