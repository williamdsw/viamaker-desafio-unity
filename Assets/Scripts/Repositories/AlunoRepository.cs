using Model;
using System.Collections.Generic;

namespace Repository
{
    public interface AlunoRepository
    {
        bool Insert(Aluno aluno);
        bool InsertMultiples(List<Aluno> alunos);
        bool Update(Aluno aluno);
        bool DeleteById(int id);
        bool DeleteAllByTurma(int turmaId);
        List<Aluno> FindByTurma(int turmaId);
        Aluno FindById(int id);
    }

}