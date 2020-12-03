using System;
using UnityEngine;

[Serializable]
public class Turma
{
    [SerializeField] private int id;
    [SerializeField] private string nome;

    public int Id { get => id; set => id = value; }
    public string Nome { get => nome; set => nome = value; }
    public Escola Escola { get; set; }

    public Turma() {}

    public Turma(int id, string nome, Escola escola)
    {
        Id = id;
        Nome = nome;
        Escola = escola;
    }
}
