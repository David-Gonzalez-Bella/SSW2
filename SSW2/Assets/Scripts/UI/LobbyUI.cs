using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public List<Button> colorButtons;

    private void Awake()
    {
        foreach (Button b in colorButtons)
        {
            b.onClick.AddListener(() => ConnectionManager.Singleton.SetColorIndex(colorButtons.IndexOf(b)));
        }
    }
}
