using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveView : MonoBehaviour
{
    public Button saveButton;
    public TMP_Text saveText;
    public event Action OnSaveButtonClick;

    private void Awake()
    {
        saveButton.onClick.AddListener(() => OnSaveButtonClick?.Invoke());
    }

    public void UpdateSaveText(string text)
    {
        saveText.text = text;
    }

    private void OnDestroy()
    {
        saveButton.onClick.RemoveAllListeners();
    }
}