using System;
using UnityEngine;

[Serializable]
public class EscolaResponse
{
    [SerializeField] private bool sucesso;
    [SerializeField] private Escola retorno;

    public bool Sucesso => sucesso;
    public Escola Retorno => retorno;
}
