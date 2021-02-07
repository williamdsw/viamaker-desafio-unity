using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Escola
{
    [SerializeField] private int id;
    [SerializeField] private string nome;
    [SerializeField] private List<Turma> turmas = new List<Turma>();

    public int Id { get => id; set => id = value; }
    public string Nome { get => nome; set => nome = value; }
    public List<Turma> Turmas => turmas;

    public Escola() {}

    public Escola(int id, string nome)
    {
        Id = id;
        Nome = nome;
    }
}