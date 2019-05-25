using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class AddShip : MonoBehaviour
{
    public AddShipEvent OnAddShip = new AddShipEvent();

#pragma warning disable 0649
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button addButton;
#pragma warning restore

    private void Start()
    {
        addButton.onClick.AddListener(() =>
        {
            OnAddShip.Invoke(nameInput.text);
            nameInput.text = "";
        });
    }

    private void Update()
    {
        addButton.interactable = nameInput.text.Length > 0;
    }
}

[Serializable]
public class AddShipEvent : UnityEvent<string> { }
