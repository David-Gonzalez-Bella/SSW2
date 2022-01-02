using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    //Componentes
    private SpriteRenderer spriteRenderer;

    //Referencias
    [SerializeField] private Color[] recolors;

    //Variables sincronizadas
    [HideInInspector] public NetworkVariable<byte> colorIndex = new NetworkVariable<byte>();

    private void Start()
    {
        colorIndex.OnValueChanged += OnColorChanged;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [ServerRpc]
    public void ChangeColorServerRpc(byte newColorIndex)
    {
        if (newColorIndex > 4) return;

        colorIndex.Value = newColorIndex;
    }

    private void OnEnable()
    {
        //Suscribimos la variable sincronizada al evento de cambio de color
        colorIndex.OnValueChanged += OnColorChanged;
    }

    private void OnDisable()
    {
        //Dessuscribimos la variable sincronizada al evento de cambio de color
        colorIndex.OnValueChanged -= OnColorChanged;
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
