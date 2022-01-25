using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public string nickname { get; private set; }

    public PlayerData(string _nickname)
    {
        nickname = _nickname;
    }
}
