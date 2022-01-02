using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] private GameObject networkPanel;
    [SerializeField] private GameObject colorPanel;
    [SerializeField] private GameObject leaveButton;
    public InputField passwordField;

    public static InterfaceManager Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton == null) Singleton = this;
    }

    public void ShowMainMenuUI()
    {
        networkPanel.SetActive(true);
        passwordField.gameObject.SetActive(true);
        colorPanel.SetActive(false);
        leaveButton.SetActive(false);
    }

    public void ShowConnectedUI()
    {
        networkPanel.SetActive(false);
        passwordField.gameObject.SetActive(false);
        colorPanel.SetActive(true);
        leaveButton.SetActive(true);
    }
}
