using System.Collections.Generic;

public interface TurmaRepository
{
    bool Insert(Turma turma);
    bool Update(Turma turma);
    bool DeleteById(int id);
    bool DeleteAllByEscola(int escolaId);
    List<Turma> FindByEscola(int escolaId);
    Turma FindById(int id);
}