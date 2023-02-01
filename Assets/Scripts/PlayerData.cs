using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int lastLevelPlayed;
    public bool[][] packsUnlocked;

    public PlayerData (bool[][] allLevelsUnlockded, int lastLevelPlayed)
    {
        packsUnlocked = allLevelsUnlockded;
        this.lastLevelPlayed = lastLevelPlayed;
    }
}
