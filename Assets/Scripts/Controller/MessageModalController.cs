using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageModalController : MonoBehaviour
{
    // || Inspector References

    [Header("UI Elements")]
    [SerializeField] private GameObject messageModal;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI body;
    [SerializeField] private Button closeButton;

    // || Cached References

    private Transform container;

    private void Start()
    {
        if (messageModal)
        {
            container = messageModal.transform.Find("Overlay").Find("Container");

            if (closeButton)
            {
                closeButton.onClick.AddListener(() => messageModal.SetActive(false));
            }
        }
    }

    public void Show(string title, string message, Color color)
    {
        if (messageModal && header && body && container)
        {
            header.text = title;
            body.text = message;
            Image image = container.GetComponent<Image>();
            image.color = color;
            messageModal.SetActive(true);
        }
    }
}