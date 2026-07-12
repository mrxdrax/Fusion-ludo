using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamemaneger : MonoBehaviour
{
    public static gamemaneger instance;

    // BUGFIX: Removed unused field 'turn_switch'

    public void Awake()
    {
        // IMPROVEMENT: Proper singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when goti cannot move. Checks if all gotis are locked before switching turn.
    /// IMPROVEMENT: Added null safety and proper manager type checking
    /// BUGFIX BUG2: Only called if player truly has no valid moves. Turn_Switch() should ONLY
    /// be called from this method OR from goti.Movement() after movement completes, never both.
    /// </summary>
    public void turn_change()
    {
        // IMPROVEMENT: Null check for dice instance
        if (dice.instance == null)
        {
            Debug.LogError("Dice instance not found!");
            return;
        }

        // BUGFIX: Check the correct manager type based on current turn
        bool allGotisLocked = false;
        
        switch (dice.instance.current_turn)
        {
            case 0:
                if (bluegamemanager1.instance != null)
                    allGotisLocked = bluegamemanager1.instance.AllGotisLocked();
                break;
            case 1:
                if (redgamemanager.instance != null)
                    allGotisLocked = redgamemanager.instance.AllGotisLocked();
                break;
            case 2:
                if (yellowmanager.instance != null)
                    allGotisLocked = yellowmanager.instance.AllGotisLocked();
                break;
            case 3:
                if (greenmanager.instance != null)
                    allGotisLocked = greenmanager.instance.AllGotisLocked();
                break;
        }

        if (allGotisLocked)
        {
            Debug.Log("✅ All gotis locked, switching turn...");
            dice.instance.Turn_Switch();
        }
        else
        {
            Debug.Log("⚠️ At least one goti unlocked, waiting for movement.");
        }
    }

    /// <summary>
    /// OPTIMIZATION: Removed GetCurrentGameManager() method since it's already in dice.cs
    /// Consolidating turn logic in one place reduces confusion.
    /// If turn logic changes, only update in one location.
    /// </summary>
}
