using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamemaneger : MonoBehaviour
{
    public static gamemaneger instance;
    public bool turn_switch = false;

    public void Awake()
    {
        instance = this;
    }

    public void turn_change()
    {
        var currentManager = GetCurrentGameManager() as bluegamemanager1;  // Adjust if needed

        if (currentManager != null && currentManager.AllGotisLocked())
        {
            Debug.Log("✅ All gotis locked, switching turn...");
            dice.instance.Turn_Switch();
        }
        else
        {
            Debug.Log("🔴 At least one goti unlocked, waiting for movement.");
        }
    }

    private object GetCurrentGameManager()
    {
        switch (dice.instance.current_turn)
        {
            case 0: return bluegamemanager1.instance;
            case 1: return redgamemanager.instance;
            case 2: return yellowmanager.instance;
            case 3: return greenmanager.instance;
            default: return bluegamemanager1.instance;
        }
    }
}
