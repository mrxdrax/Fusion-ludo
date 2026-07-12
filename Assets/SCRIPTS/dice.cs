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

    public void Awake()
    {
        instance = this;
    }

    public void OnButtonClick()
    {
        if (waitingForTokenMovement) return;

        print("dice");
        StartCoroutine(dice_roll());

        int random_no = Random.Range(1, 7);
        image.sprite = Dice_Image[random_no - 1];
        dice_count = random_no;
        Debug.Log("🎲 Dice Rolled: " + dice_count);

        waitingForTokenMovement = true;

        var currentManager = GetCurrentGameManager() as bluegamemanager1; // Type cast to specific manager

        if (currentManager != null && currentManager.AllGotisLocked())
        {
            StartCoroutine(DelayedTurnSwitch());
        }
    }


    public IEnumerator DelayedTurnSwitch()
    {
        yield return new WaitForSeconds(1f);
        Turn_Switch();
    }

    IEnumerator dice_roll()
    {
        dice_animation.SetActive(true);
        dice_image.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        dice_animation.SetActive(false);
        dice_image.SetActive(true);
        transform.GetComponent<Button>().interactable = false;
    }

    public void Turn_Switch()
    {
        if (dice_count != 6)
        {
            print("switch");
            current_turn = (current_turn + 1) % 4;
            transform.position = turn[current_turn].position;
            dice_animation.transform.position = dice_rolling[current_turn].position;
            dice_image.transform.position = turn[current_turn].position;
        }

        EnableDiceButton();
        Turn_change = false;
        waitingForTokenMovement = false;
    }

    public void EnableDiceButton()
    {
        transform.GetComponent<Button>().interactable = true;
    }

    private object GetCurrentGameManager()
    {
        switch (current_turn)
        {
            case 0: return bluegamemanager1.instance;
            case 1: return redgamemanager.instance;
            case 2: return yellowmanager.instance;
            case 3: return greenmanager.instance;
            default: return null;
        }
    }




    private IEnumerator WaitForTokenMovement()
    {
        yield return new WaitUntil(() => !goti.instance.ismoving);
        Debug.Log("🔄 Goti movement completed, switching turn.");
        Turn_Switch();
    }
}
