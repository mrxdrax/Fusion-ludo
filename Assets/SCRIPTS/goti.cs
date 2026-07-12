using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goti : MonoBehaviour
{
    public bool ismoving = false;
    public static goti instance;
    public bool unlocked = false;
    public Transform[] path;
    private int current_index = 0;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        
    }

    public void Unlock()
    {
        if (!unlocked)
        {
            if (dice.instance.dice_count == 1 || dice.instance.dice_count == 6)
            {
                transform.position = path[0].position;
                unlocked = true;
                Debug.Log("Goti Unlocked!");
            }
            else
            {
                gamemaneger.instance.turn_change();
            }
        }
    }

    public void OnMouseDown()
    {
        if (!unlocked)
        {
            Unlock(); // Agar goti locked hai, toh yeh unlock karne ki koshish karega
        }

        if (unlocked)
        {
            StartCoroutine(Movement());
        }
    }

    IEnumerator Movement()
    {
        ismoving = true;
        int diceVal = dice.instance.dice_count;

        while (diceVal > 0 && current_index < path.Length - 1)
        {
            yield return new WaitForSeconds(0.5f);
            current_index++;
            transform.position = path[current_index].position;
            diceVal--;
        }

        CheckForCut();

        ismoving = false;
        if (dice.instance.dice_count != 6)
        {
            StartCoroutine(dice.instance.DelayedTurnSwitch());
        }
        else
        {
            dice.instance.EnableDiceButton();
        }
    }

    void CheckForCut()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (var hit in hitColliders)
        {
            goti otherGoti = hit.GetComponent<goti>();
            if (otherGoti != null && otherGoti != this && otherGoti.gameObject.tag != this.gameObject.tag)
            {
                otherGoti.ResetPosition();
            }
        }
    }

    void ResetPosition()
    {
        unlocked = false;
        current_index = 0;
        transform.position = path[0].position;
    }
}
