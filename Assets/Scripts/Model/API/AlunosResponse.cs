using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AlunosResponse
{
    [SerializeField] private bool sucesso;
    [SerializeField] private List<Aluno> retorno;

    public bool Sucesso { get => sucesso; }
    public List<Aluno> Retorno { get => retorno; }
}
