using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Service
{
    public class RequestService
    {
        public IEnumerator GetRequest(string url, Action<string> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", string.Format("Bearer {0}", Config.Environment.TokenAPI));
                yield return request.SendWebRequest();

                try
                {
                    if (!string.IsNullOrEmpty(request.error))
                    {
                        throw new Exception("Erro ao buscar recursos!");
                    }

                    callback(request.downloadHandler.text);
                }
                catch (Exception ex)
                {
                    callback(null);
                    throw ex;
                }
            }
        }

        public IEnumerator PostRequest(string url, Dictionary<string, string> form, Action<string> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                request.SetRequestHeader("Authorization", string.Format("Bearer {0}", Config.Environment.TokenAPI));
                yield return request.SendWebRequest();

                try
                {
                    if (!string.IsNullOrEmpty(request.error))
                    {
                        throw new Exception("Erro ao buscar recursos!");
                    }

                    callback(request.downloadHandler.text);
                }
                catch (Exception ex)
                {
                    callback(null);
                    throw ex;
                }
            }
        }
    }
}