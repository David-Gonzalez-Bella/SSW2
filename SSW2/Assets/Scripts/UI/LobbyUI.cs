using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    //Variables
    public Color[] recolors = { Color.white, Color.red, Color.green, Color.blue, Color.yellow };

    public List<Button> characterButtons;

    public static LobbyUI Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
        characterButtons.ForEach(b => b.onClick.AddListener(() => ConnectionManager.Singleton.SetCharacterIndex(characterButtons.IndexOf(b))));
    }
}
