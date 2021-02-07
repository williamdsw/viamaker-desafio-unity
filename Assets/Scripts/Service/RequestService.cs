using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class RequestService
{
    public IEnumerator GetRequest(string url, Action<string> response)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", Environment.TokenAPI));
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                throw new Exception("Erro ao buscar recursos!");
            }

            response(request.downloadHandler.text);
        }
    }

    public IEnumerator PostRequest(string url, Dictionary<string, string> formFields, Action<string> response)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(url, formFields))
        {
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", Environment.TokenAPI));
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                throw new Exception("Erro ao buscar recursos!");
            }

            response(request.downloadHandler.text);
        }
    }
}