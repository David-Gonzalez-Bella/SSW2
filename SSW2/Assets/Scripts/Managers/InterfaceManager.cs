using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] private GameObject networkPanel;
    [SerializeField] private GameObject colorPanel;
    [SerializeField] private GameObject leaveButton;

    public TMP_InputField nicknameField;
    public TMP_InputField passwordField;

    public static InterfaceManager Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton == null) Singleton = this;
    }

    public void ShowMainMenuUI()
    {
        nicknameField.gameObject.SetActive(true);
        passwordField.gameObject.SetActive(true);
        networkPanel.SetActive(true);
        colorPanel.SetActive(false);
        leaveButton.SetActive(false);
    }

    public void ShowConnectedUI()
    {
        nicknameField.gameObject.SetActive(false);
        passwordField.gameObject.SetActive(false);
        networkPanel.SetActive(false);
        colorPanel.SetActive(true);
        leaveButton.SetActive(true);
    }
}
