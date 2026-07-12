using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yellowmanager : MonoBehaviour
{
    public Transform[] yellow_path;
    public GameObject yellow_goti;
    public GameObject[] spawn_area;
    public static yellowmanager instance;
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
            GameObject goti = Instantiate(yellow_goti, spawn_area[i].transform.position, Quaternion.identity);
            goti.transform.parent = this.transform;
            goti.GetComponent<goti>().path = yellow_path;
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
