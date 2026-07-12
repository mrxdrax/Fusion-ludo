using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class redgamemanager : MonoBehaviour
{
    public Transform[] red_path;
    public GameObject red_goti;
    public GameObject[] spawn_area;
    public static redgamemanager instance;
    public void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Spawn();
    }
    void Update()
    {
    }
    public void Spawn()
    {

        for (int i = 0; i < spawn_area.Length; i++)
        {
            GameObject goti = Instantiate(red_goti, spawn_area[i].transform.position, Quaternion.identity);
            goti.transform.parent = this.transform;
            goti.GetComponent<goti>().path = red_path;
        };
    }
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
