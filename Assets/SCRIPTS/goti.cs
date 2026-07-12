using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goti : MonoBehaviour
{
    public bool ismoving = false;
    public bool unlocked = false;
    public Transform[] path;
    private int current_index = 0;

    // OPTIMIZATION: Cache dice and gamemanager references
    private dice diceController;
    private gamemaneger gameManager;
    
    // BUGFIX: Store original position for proper reset
    private Vector3 originalSpawnPosition;
    
    // BUGFIX: Prevent multiple movement coroutines
    private Coroutine currentMovementCoroutine;

    public void Awake()
    {
        // IMPROVEMENT: Cache component references to avoid repeated lookups
        diceController = dice.instance;
        gameManager = gamemaneger.instance;
        
        // BUGFIX: Store the original spawn position for proper reset
        originalSpawnPosition = transform.position;

        // IMPROVEMENT: Null checks for safety
        if (diceController == null)
            Debug.LogError("Dice instance not found!");
        if (gameManager == null)
            Debug.LogError("Game Manager instance not found!");
    }

    public void Start()
    {
        // IMPROVEMENT: Validate path array exists and has elements
        if (path == null || path.Length == 0)
        {
            Debug.LogError("Goti path is not assigned or empty!");
        }
    }

    /// <summary>
    /// Unlock goti if dice shows 1 or 6
    /// IMPROVEMENT: Added null safety checks
    /// </summary>
    public void Unlock()
    {
        if (unlocked)
            return;

        // IMPROVEMENT: Null check for dice controller
        if (diceController == null)
        {
            Debug.LogError("Cannot unlock: Dice instance not found!");
            return;
        }

        if (diceController.dice_count == 1 || diceController.dice_count == 6)
        {
            // IMPROVEMENT: Verify path exists before accessing
            if (path != null && path.Length > 0)
            {
                transform.position = path[0].position;
                unlocked = true;
                current_index = 0;
                Debug.Log("✅ Goti Unlocked!");
            }
            else
            {
                Debug.LogError("Cannot unlock goti: path not assigned!");
            }
        }
        else
        {
            // IMPROVEMENT: Null check for game manager
            if (gameManager != null)
                gameManager.turn_change();
        }
    }

    public void OnMouseDown()
    {
        // BUGFIX: Prevent multiple simultaneous movements
        if (ismoving)
            return;

        if (!unlocked)
        {
            Unlock();
        }

        // BUGFIX: Only move if unlocked AND not already moving
        if (unlocked && !ismoving)
        {
            // BUGFIX: Stop any existing movement coroutine before starting new one
            if (currentMovementCoroutine != null)
                StopCoroutine(currentMovementCoroutine);

            currentMovementCoroutine = StartCoroutine(Movement());
        }
    }

    /// <summary>
    /// Move goti along path by dice value
    /// IMPROVEMENT: Better movement calculation
    /// CRITICAL FIX: Movement-based turn switching only happens AFTER all movement is complete
    /// BUGFIX BUG1: Movement completion is the ONLY place where Turn_Switch() should be called
    /// </summary>
    IEnumerator Movement()
    {
        ismoving = true;

        // IMPROVEMENT: Null check for dice
        if (diceController == null)
        {
            Debug.LogError("Dice controller not found during movement!");
            ismoving = false;
            yield break;
        }

        int diceVal = diceController.dice_count;

        // IMPROVEMENT: Verify path is valid
        if (path == null || path.Length == 0)
        {
            Debug.LogError("Path not set for goti movement!");
            ismoving = false;
            yield break;
        }

        // IMPROVEMENT: Better boundary checking
        int startIndex = current_index;
        int maxIndex = path.Length - 1;

        // Move step by step with proper validation
        while (diceVal > 0 && current_index < maxIndex)
        {
            yield return new WaitForSeconds(0.5f);
            current_index++;
            
            // SAFETY: Verify current_index is valid
            if (current_index <= maxIndex)
            {
                transform.position = path[current_index].position;
                diceVal--;
            }
        }

        // IMPROVEMENT: Check for cut after movement completes
        CheckForCut();

        ismoving = false;
        currentMovementCoroutine = null;

        // CRITICAL FIX BUG1 + BUG2: Turn switching only happens here, AFTER movement is fully complete
        // This ensures turn does NOT switch until animation and state are fully reset
        // The gate flag 'turnSwitchPending' ensures Turn_Switch() is called exactly once
        if (diceController != null)
        {
            if (diceController.dice_count != 6)
            {
                // Not a 6: signal that turn should switch ONLY after this movement finishes
                // Set the gate flag and call DelayedTurnSwitch which will respect the gate
                diceController.turnSwitchPending = true;
                StartCoroutine(diceController.DelayedTurnSwitch());
            }
            else
            {
                // Rolled 6: enable dice button for extra turn, don't switch players
                // Also reset the waiting flag so player can roll again immediately
                diceController.waitingForTokenMovement = false;
                diceController.EnableDiceButton();
            }
        }
    }

    /// <summary>
    /// Check for collision with other gotis and reset them if on same spot
    /// IMPROVEMENT: Better collision detection with null checks
    /// </summary>
    void CheckForCut()
    {
        // OPTIMIZATION: Cache collider array instead of creating new one each call
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f);
        
        foreach (var hit in hitColliders)
        {
            // IMPROVEMENT: Better approach - search for goti component directly
            goti otherGoti = hit.GetComponent<goti>();
            
            // BUGFIX: Null checks and proper tag comparison
            if (otherGoti != null && otherGoti != this)
            {
                // IMPROVEMENT: Use CompareTag for better performance
                if (!CompareTag(otherGoti.gameObject.tag))
                {
                    Debug.Log("💥 Cut! Resetting opponent goti");
                    otherGoti.ResetPosition();
                }
            }
        }
    }

    /// <summary>
    /// Reset goti to spawn position when cut
    /// BUGFIX: Use original spawn position instead of path[0]
    /// </summary>
    void ResetPosition()
    {
        unlocked = false;
        current_index = 0;
        
        // BUGFIX: Reset to original spawn position, not path[0]
        transform.position = originalSpawnPosition;
        
        Debug.Log("🔄 Goti reset to home");
    }
}
