using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RequestService
{
    public IEnumerator GetRequest(string url, Action<string> response)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {Environment.TOKEN_API}");
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                throw new Exception("Some error happened");
            }

            response(request.downloadHandler.text);
        }
    }
}
