using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    public TMP_InputField nicknameField;
    public TMP_InputField passwordField;

    public static MainMenuUI Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton == null) Singleton = this;
    }
}
