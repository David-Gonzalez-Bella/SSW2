using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (!Input.GetKeyDown(KeyCode.Space)) return;

        ChangeColorServerRpc(); //Decirle al servidor que cambie nuestro color a ojos de todos

        spriteRenderer.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)); //Cambiamos nuestro color al instante de forma local (mejor experiencia local)
    }

    [ServerRpc]
    private void ChangeColorServerRpc()
    {
        ChangeColorClientRpc(); //Ejecutar el cambio de color en cada cliente
    }

    [ClientRpc]
    private void ChangeColorClientRpc()
    {
        if (IsOwner) return; //El color ya se ha cambiado en el cliente invocante

        spriteRenderer.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
}
