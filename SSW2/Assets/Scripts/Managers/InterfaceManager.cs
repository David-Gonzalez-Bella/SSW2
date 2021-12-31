using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] private GameObject HostButton;
    [SerializeField] private GameObject ClientButton;
    [SerializeField] private GameObject LeaveButton;
    public InputField PasswordField;

    public static InterfaceManager Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton == null) Singleton = this;
    }


    public void ShowMainMenuUI()
    {
        HostButton.SetActive(true);
        ClientButton.SetActive(true);
        PasswordField.gameObject.SetActive(true);
        LeaveButton.SetActive(false);
    }

    public void ShowConnectedUI()
    {
        HostButton.SetActive(false);
        ClientButton.SetActive(false);
        PasswordField.gameObject.SetActive(false);
        LeaveButton.SetActive(true);
    }
}
