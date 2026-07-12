using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class greenmanager : MonoBehaviour
{
    public Transform[] green_path;
    public GameObject green_goti;
    public GameObject[] spawn_area;
    public static greenmanager instance;
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
            GameObject goti = Instantiate(green_goti, spawn_area[i].transform.position, Quaternion.identity);
            goti.transform.parent = this.transform;
            goti.GetComponent<goti>().path = green_path;
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
