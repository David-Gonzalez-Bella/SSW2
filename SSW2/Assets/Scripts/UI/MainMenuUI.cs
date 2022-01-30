using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class MainMenuUI : MonoBehaviour
{
    public TMP_InputField nicknameField;
    public TMP_InputField passwordField;

    public Button hostButton;
    public Button clientButton;

    public static MainMenuUI Singleton { get; private set; }

    private void Awake()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
    }

    private void Start()
    {
        hostButton.onClick.AddListener(Host);
        clientButton.onClick.AddListener(Client);
    }

    public void Host()
    {
        //Cuando un cliente se concecte a este host se le concede acceso validando su contraseña
        ConnectionManager.Singleton.InitializePlayerDataList();
        NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        var payload = JsonUtility.ToJson(new ConnectionPayload()
        {
            nickname = nicknameField.text,
            password = passwordField.text
        });

        byte[] connectionData = Encoding.ASCII.GetBytes(payload);

        //Cuando un cliente se conecta, se setea la información que enviaremos al servidor
        NetworkManager.Singleton.NetworkConfig.ConnectionData = connectionData;
        NetworkManager.Singleton.StartClient();
    }
}
