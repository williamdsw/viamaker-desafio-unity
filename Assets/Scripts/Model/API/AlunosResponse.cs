using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AlunosResponse
{
    [SerializeField] private bool sucesso;
    [SerializeField] private List<Aluno> retorno;

    public bool Sucesso => sucesso;
    public List<Aluno> Retorno => retorno;
}
