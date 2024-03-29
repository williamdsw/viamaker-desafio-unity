﻿using Model;
using System.Collections.Generic;

namespace Repository
{
    public interface TurmaRepository
    {
        bool Insert(Turma turma);
        bool InsertMultiples(List<Turma> turmas);
        bool Update(Turma turma);
        bool DeleteById(int id);
        bool DeleteAllByEscola(int escolaId);
        List<Turma> FindByEscola(int escolaId);
        Turma FindById(int id);
    }
}