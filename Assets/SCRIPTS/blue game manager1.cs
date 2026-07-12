using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bluegamemanager1 : MonoBehaviour
{
    public static bluegamemanager1 instance;
    public Transform[] blue_path;
    public GameObject blue_goti;
    public GameObject[] spawn_area;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        for (int i = 0; i < spawn_area.Length; i++)
        {
            GameObject goti = Instantiate(blue_goti, spawn_area[i].transform.position, Quaternion.identity);
            goti.transform.parent = this.transform;
            goti.GetComponent<goti>().path = blue_path;
        }
    }

    // Check if any token is unlocked
    public bool AnyGotiUnlocked()
    {
        foreach (Transform gotiTransform in this.transform)
        {
            goti gotiScript = gotiTransform.GetComponent<goti>();
            if (gotiScript != null && gotiScript.unlocked)
            {
                return true;
            }
        }
        return false;
    }

    // Check if all tokens are unlocked
    public bool AllGotisLocked()
    {
        foreach (Transform goti in this.transform)
        {
            goti script = goti.GetComponent<goti>();
            if (script != null && script.unlocked)
            {
                return false;  // Agar ek bhi goti unlocked hai, turn switch nahi hoga
            }
        }
        return true; // Agar sabhi gotiya locked hai, turn switch hoga
    }

}
