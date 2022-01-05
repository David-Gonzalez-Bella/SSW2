using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerLobby : NetworkBehaviour
{
    //Componentes
    private SpriteRenderer spriteRenderer;
    private TMP_Text nicknameText;

    //Referencias
    [SerializeField] private Color[] recolors;

    //Variables sincronizadas
    [HideInInspector] public NetworkVariable<FixedString64Bytes> nickname = new NetworkVariable<FixedString64Bytes>();
    [HideInInspector] public NetworkVariable<byte> colorIndex = new NetworkVariable<byte>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        nicknameText = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        //Suscribimos la variable sincronizada al evento de cambio de color
        nickname.OnValueChanged += OnNicknameChanged;
        colorIndex.OnValueChanged += OnColorChanged;
    }

    private void OnDisable()
    {
        //Dessuscribimos la variable sincronizada al evento de cambio de color
        nickname.OnValueChanged -= OnNicknameChanged;
        colorIndex.OnValueChanged -= OnColorChanged;
    }

    public override void OnNetworkSpawn()
    {
        //Al ser instanciado, seteamos (en el servidor) el nombre del jugador el que guardó el servidor en el login
        if (!IsServer) return;

        SetClientNickname();
    }

    private void SetClientNickname()
    {
        PlayerData? playerData = ConnectionManager.Singleton.GetPlayerData(OwnerClientId);

        if (playerData.HasValue)
        {
            nickname.Value = new FixedString64Bytes(playerData.Value.nickname);
        }
    }

    [ServerRpc]
    public void ChangeNameServerRpc()
    {
        SetClientNickname();
    }

    [ServerRpc]
    public void ChangeColorServerRpc(byte newColorIndex)
    {
        if (newColorIndex > 4) return;

        colorIndex.Value = newColorIndex;
    }

    private void OnNicknameChanged(FixedString64Bytes oldNickname, FixedString64Bytes newNickname)
    {
        nicknameText.text = newNickname.ToString();
    }

    private void OnColorChanged(byte oldColorIndex, byte newColorIndex)
    {
        if (!IsClient) return;

        spriteRenderer.color = recolors[newColorIndex];
    }

    //[ClientRpc]
    //private void ChangeColorClientRpc()
    //{
    //    if (IsOwner) return; //El color ya se ha cambiado en el cliente invocante

    //    spriteRenderer.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    //}


}
