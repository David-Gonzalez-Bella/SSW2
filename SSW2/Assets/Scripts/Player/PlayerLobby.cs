using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerLobby : NetworkBehaviour
{
    #region Variables
    //Componentes
    private SpriteRenderer spriteRenderer;
    private TMP_Text nicknameText;

    //Variables
    private Color[] recolors = { Color.white, Color.red, Color.green, Color.blue, Color.yellow };

    //Variables sincronizadas
    [HideInInspector] public NetworkVariable<FixedString32Bytes> nickname = new NetworkVariable<FixedString32Bytes>();
    [HideInInspector] public NetworkVariable<byte> colorIndex = new NetworkVariable<byte>();
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        nicknameText = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        //[BUG]: En la version 1.0.0-pre.3, al unirse un nuevo cliente, no se llama en el lado del cliente a los hooks de aquellos jugadores que ya
        //estaban de antes, solo a los del que acaba de unirse. De este modo, aunque los valores de las NetworkVariables estan bien en ambos lados,
        //en el del cliente no se ha producido el cambio correspondiente a su actualización (es decir, la llamada a los hooks), por lo que hay que hacer
        //las llamadas a los hooks de los demás clientes que ya estaban en la sala manualmente

        if (IsHost) return; //En el caso del host se hace bien

        if (IsOwner) return; //Si se trata del cliente cuyos hooks ya se han llamado no hacemos las llamadas manuales (ya se han hecho)

        OnNicknameChanged(nickname.Value, nickname.Value);
        OnColorChanged(colorIndex.Value, colorIndex.Value);
    }

    private void OnEnable()
    {
        //Asignamos los hooks al principio del todo
        nickname.OnValueChanged += OnNicknameChanged;
        colorIndex.OnValueChanged += OnColorChanged;
    }

    private void OnDisable()
    {
        //Desasignar los hooks
        nickname.OnValueChanged -= OnNicknameChanged;
        colorIndex.OnValueChanged -= OnColorChanged;
    }

    public override void OnNetworkSpawn()
    {
        //Al ser instanciado, seteamos (en el servidor) el nombre del jugador el que guardó el servidor en el login
        if (!IsServer) return;

        PlayerData? playerData = ConnectionManager.Singleton.GetPlayerData(OwnerClientId);

        if (playerData.HasValue)
        {
            nickname.Value = new FixedString32Bytes(playerData.Value.nickname);
        }
    }
    #endregion

    #region RPCs
    [ServerRpc]
    public void ChangeColorServerRpc(byte newColorIndex)
    {
        if (newColorIndex > 4) return;

        colorIndex.Value = newColorIndex;
    }
    #endregion

    #region Hooks
    private void OnNicknameChanged(FixedString32Bytes oldNickname, FixedString32Bytes newNickname)
    {
        nicknameText.text = newNickname.ToString();
    }

    private void OnColorChanged(byte oldColorIndex, byte newColorIndex)
    {
        spriteRenderer.color = recolors[newColorIndex];
    }
    #endregion
}
