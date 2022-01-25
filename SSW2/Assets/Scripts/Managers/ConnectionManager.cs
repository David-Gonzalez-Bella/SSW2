using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Text;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Singleton { get; private set; }
    private Dictionary<ulong, PlayerData> clientData;
 
    private void Awake()
    {
        if (Singleton == null) Singleton = this;

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
    }

    public void Host()
    {
        clientData = new Dictionary<ulong, PlayerData>();
        clientData[NetworkManager.Singleton.LocalClientId] = new PlayerData(MainMenuUI.Singleton.nicknameField.text);

        //Cuando un cliente se concecte a este host se le concede acceso validando su contraseña
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        var payload = JsonUtility.ToJson(new ConnectionPayload()
        {
            nickname = MainMenuUI.Singleton.nicknameField.text,
            password = MainMenuUI.Singleton.passwordField.text
        });

        byte[] connectionData = Encoding.ASCII.GetBytes(payload);

        //Cuando un cliente se conecta, se setea la información que enviaremos al servidor
        NetworkManager.Singleton.NetworkConfig.ConnectionData = connectionData;
        NetworkManager.Singleton.StartClient();
    }

    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        NetworkManager.Singleton.Shutdown(); //StopHost / StopClient
        NetworkManager.Singleton.SceneManager.LoadScene(TypedClasses.Scene_MainMenu, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    // Se ejecuta en el cliente y en el servidor al conectarse un cliente
    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            //Cuando un cliente (client o host) se conecta, va al lobby
            NetworkManager.Singleton.SceneManager.LoadScene(TypedClasses.Scene_Lobby, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    // Se ejecuta en el cliente y en el servidor cuando este desconecta a un cliente (no cuando el cliente se desconecta de motu propio)
    private void HandleClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            //Cuando un cliente es desconectado por el host, se actualiza el interfaz
            NetworkManager.Singleton.SceneManager.LoadScene(TypedClasses.Scene_MainMenu, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    public PlayerData? GetPlayerData(ulong clientId)
    {
        if (clientData.TryGetValue(clientId, out PlayerData playerData))
        {
            return playerData;
        }
        return null;
    }

    public void SetColorIndex(int colorIndex)
    {
        NetworkClient localClient = NetworkManager.Singleton.IsHost ?
                      NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId] :
                      NetworkManager.Singleton.LocalClient;

        //Obtenemos el jugador local, el de la sesión que corresponde
        if (localClient == null) return;

        //Obtenemos la componente "Player" del jugador local
        if (!localClient.PlayerObject.TryGetComponent<PlayerLobby>(out PlayerLobby localPlayer)) return;

        //El jugador local llama una RPC para informar al servidor del cambio de color
        localPlayer.ChangeColorServerRpc((byte)colorIndex);
    }

    // Se ejecuta en el servidor cuando un cliente se conecta
    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate result)
    {
        //Comprobar que el nombre no llega vacio y que la contraseña que llega coincide con la que ve (introdujo) el host
        string payload = Encoding.ASCII.GetString(connectionData);

        var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

        bool connectionApproved = !string.IsNullOrEmpty(connectionPayload?.nickname.Trim()) &&
                                  connectionPayload?.password == MainMenuUI.Singleton.passwordField.text;

        Vector3 spawnPos = NetworkManager.Singleton.LocalClientId == clientId ? new Vector3(-0.5f, 0f, 0f) : new Vector3(0.5f, 0f, 0f);

        if (connectionApproved)
        {
            clientData[clientId] = new PlayerData(connectionPayload?.nickname);
        }

        result(true, null, connectionApproved, spawnPos, Quaternion.identity); //Resultado de la evaluación
    }
}
