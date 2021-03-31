using System;
using UnityEngine;

namespace Model.API
{
    [Serializable]
    public class EscolaResponse
    {
        [SerializeField] private bool sucesso;
        [SerializeField] private Escola retorno;

        public bool Sucesso => sucesso;
        public Escola Retorno => retorno;
    }
}