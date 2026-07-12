using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dice : MonoBehaviour
{
    public Sprite[] Dice_Image;
    public Transform[] turn;
    public Transform[] dice_rolling;
    public Image image;
    public GameObject dice_animation;
    public GameObject dice_image;
    public int dice_count;
    public bool Turn_change = false;
    public static dice instance;
    public int current_turn = 0;
    public bool waitingForTokenMovement = false;

    // OPTIMIZATION: Cache Button component to avoid repeated GetComponent calls
    private Button diceButton;
    private bool isRolling = false;
    
    // BUGFIX BUG2: Gate flag to prevent duplicate Turn_Switch() calls
    // PUBLIC so goti.cs can set it after movement completes
    public bool turnSwitchPending = false;

    public void Awake()
    {
        instance = this;
        // IMPROVEMENT: Cache Button component on initialization
        diceButton = transform.GetComponent<Button>();
        if (diceButton == null)
        {
            Debug.LogError("Dice button component not found!");
        }
    }

    public void OnButtonClick()
    {
        // BUGFIX: Prevent multiple simultaneous dice rolls
        if (waitingForTokenMovement || isRolling)
            return;

        isRolling = true;
        StartCoroutine(dice_roll());

        // IMPROVEMENT: Generate random value first (before coroutine)
        int random_no = Random.Range(1, 7);
        dice_count = random_no;

        // OPTIMIZATION: Only update sprite if image is assigned
        if (image != null && Dice_Image != null && Dice_Image.Length >= random_no)
        {
            image.sprite = Dice_Image[random_no - 1];
        }

        Debug.Log("🎲 Dice Rolled: " + dice_count);

        waitingForTokenMovement = true;

        // BUGFIX: Only skip turn if player has NO valid moves
        // A player has valid moves if:
        // 1. They have unlocked tokens that can move, OR
        // 2. They rolled 1 or 6 (can unlock a token)
        
        bool hasUnlockedTokens = false;
        bool canUnlock = (dice_count == 1 || dice_count == 6);

        // Check if current player has any unlocked tokens
        if (current_turn == 0 && bluegamemanager1.instance != null)
            hasUnlockedTokens = bluegamemanager1.instance.AnyGotiUnlocked();
        else if (current_turn == 1 && redgamemanager.instance != null)
            hasUnlockedTokens = redgamemanager.instance.AnyGotiUnlocked();
        else if (current_turn == 2 && yellowmanager.instance != null)
            hasUnlockedTokens = yellowmanager.instance.AnyGotiUnlocked();
        else if (current_turn == 3 && greenmanager.instance != null)
            hasUnlockedTokens = greenmanager.instance.AnyGotiUnlocked();

        // BUGFIX: Only skip turn if NO unlocked tokens AND cannot unlock
        if (!hasUnlockedTokens && !canUnlock)
        {
            // Player has no valid moves, skip their turn
            turnSwitchPending = true;
            StartCoroutine(DelayedTurnSwitch());
        }
        // Otherwise, wait for player to move (don't auto-skip)
    }

    public IEnumerator DelayedTurnSwitch()
    {
        yield return new WaitForSeconds(1f);
        
        // BUGFIX BUG2: Check gate flag to prevent duplicate Turn_Switch() calls
        if (turnSwitchPending)
        {
            turnSwitchPending = false;
            Turn_Switch();
        }
    }

    IEnumerator dice_roll()
    {
        // IMPROVEMENT: Null checks before accessing game objects
        if (dice_animation != null)
            dice_animation.SetActive(true);
        if (dice_image != null)
            dice_image.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        if (dice_animation != null)
            dice_animation.SetActive(false);
        if (dice_image != null)
            dice_image.SetActive(true);

        // OPTIMIZATION: Use cached button component instead of GetComponent
        if (diceButton != null)
            diceButton.interactable = false;

        isRolling = false;
    }

    public void Turn_Switch()
    {
        // IMPROVEMENT: Only switch turn if dice wasn't 6
        if (dice_count != 6)
        {
            current_turn = (current_turn + 1) % 4;
            
            // IMPROVEMENT: Null checks before setting positions
            if (current_turn < turn.Length && turn[current_turn] != null)
                transform.position = turn[current_turn].position;
            
            if (current_turn < dice_rolling.Length && dice_rolling[current_turn] != null)
                dice_animation.transform.position = dice_rolling[current_turn].position;
            
            if (current_turn < turn.Length && turn[current_turn] != null)
                dice_image.transform.position = turn[current_turn].position;
        }

        EnableDiceButton();
        Turn_change = false;
        waitingForTokenMovement = false;
    }

    public void EnableDiceButton()
    {
        // OPTIMIZATION: Use cached button instead of GetComponent
        if (diceButton != null)
            diceButton.interactable = true;
    }

    /// <summary>
    /// Get the current player's game manager based on turn index
    /// BUGFIX: Returns object type to be checked for correct manager type
    /// </summary>
    private object GetCurrentGameManager()
    {
        switch (current_turn)
        {
            case 0: 
                return bluegamemanager1.instance;
            case 1: 
                return redgamemanager.instance;
            case 2: 
                return yellowmanager.instance;
            case 3: 
                return greenmanager.instance;
            default:
                Debug.LogWarning("Invalid turn value: " + current_turn);
                return bluegamemanager1.instance;
        }
    }
}
