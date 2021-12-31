using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Text;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    private void OnDestroy()
    {
        if(NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
    }

    public void Host() 
    {
        //Cuando un cliente se concecte a este host se le concede acceso validando su contraseña
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck; 
        NetworkManager.Singleton.StartHost();
    }

    public void Client() 
    {
        //Cuando un cliente se conecta, se setea la contraseña a comprar 
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(InterfaceManager.Singleton.PasswordField.text);
        NetworkManager.Singleton.StartClient();
    }

    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        NetworkManager.Singleton.Shutdown(); //StopHost / StopClient
        InterfaceManager.Singleton.ShowMainMenuUI();
    }


    // Se ejecuta en el servidor cuando un cliente se conecta
    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate result)
    {
        string password = Encoding.ASCII.GetString(connectionData);

        bool connectionApproved = password == InterfaceManager.Singleton.PasswordField.text; //Comprobar si la contraseña que llega coincide con la que ve (introdujo) el host

        Vector3 spawnPos = NetworkManager.Singleton.LocalClientId == clientId ? new Vector3(-0.5f, 0f, 0f) : new Vector3(0.5f, 0f, 0f);

        result(true, null, connectionApproved, spawnPos, Quaternion.identity); //Resultado de la evaluación
    }

    // Se ejecuta en el cliente y en el servidor al conectarse un cliente
    private void HandleClientConnected(ulong clientId)
    {
        
        if(clientId == NetworkManager.Singleton.LocalClientId)
        {
            //Cuando un cliente (client o host) se conecta, se actualiza el interfaz
            InterfaceManager.Singleton.ShowConnectedUI();
        }
    }

    // Se ejecuta en el cliente y en el servidor cuando este desconecta a un cliente (no cuando el cliente se desconecta de motu propio)
    private void HandleClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            //Cuando un cliente es desconectado por el host, se actualiza el interfaz (redundante)
            InterfaceManager.Singleton.ShowMainMenuUI();
        }
    }
}
