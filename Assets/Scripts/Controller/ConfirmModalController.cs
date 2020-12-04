using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmModalController : MonoBehaviour
{
    [SerializeField] private GameObject confirmModal;
    [SerializeField] private Button noButton;
    [SerializeField] private Button yesButton;

    private bool result;

    private void Start() 
    {
        
    }

    public void Show(Action<bool> callback)
    {
        if (confirmModal && noButton && yesButton)
        {
            noButton.onClick.AddListener(() => StartCoroutine(SetResult (false, callback)));
            yesButton.onClick.AddListener(() => StartCoroutine(SetResult (true, callback)));
            confirmModal.SetActive(true);
        }
    }

    private IEnumerator SetResult(bool flag, Action<bool> callback)
    {
        yield return flag;
        callback(flag);
        confirmModal.SetActive(false);
    }
}
