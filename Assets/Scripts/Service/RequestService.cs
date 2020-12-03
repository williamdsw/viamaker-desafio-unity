using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RequestService : MonoBehaviour
{
    private const string BASE_URL = "http://uniescolas.viamaker.com.br/api";

    public void TesteRequest()
    {
        StartCoroutine(GetRequest(BASE_URL + "/obter/escola"));
    }

    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            string token = "Bearer 8mspL8yN09CgSQ3sgMfwQkfNm2bO64NW2789Wo0EodONKcuKeUtu1taZjG3Wu5XQUi61uxIZiDqxlxuaoZW9LJ5Hj992DNp6H0pk1wA6h4CZdtZkV6fv5xv8mKcFmkQe";
            request.SetRequestHeader("Authorization", token);
            yield return request.SendWebRequest();

            if (!request.isNetworkError)
            {
                string content = request.downloadHandler.text;
                Debug.Log("Content: " + content);
            }
            else
            {
                Debug.Log("Error: " + request.downloadHandler.error);
            }
        }
    }
}
