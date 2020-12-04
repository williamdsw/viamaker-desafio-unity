using System;
using UnityEngine;

[Serializable]
public class Aluno
{
    [SerializeField] private int id;
    [SerializeField] private string nome;

    public int Id { get => id; set => id = value; }
    public string Nome { get => nome; set => nome = value; }
    public Turma Turma { get; set; }

    public Aluno() {}

    public Aluno(int id, string nome, Turma turma)
    {
        Id = id;
        Nome = nome;
        Turma = turma;
    }

    public Aluno(string nome, Turma turma)
    {
        Nome = nome;
        Turma = turma;
    }
}
