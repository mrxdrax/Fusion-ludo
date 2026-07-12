using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bluegamemanager1 : MonoBehaviour
{
    public static bluegamemanager1 instance;
    public Transform[] blue_path;
    public GameObject blue_goti;
    public GameObject[] spawn_area;

    // OPTIMIZATION: Cache gotis to avoid repeated GetComponent calls
    private goti[] cachedGotis;
    private bool gotisInitialized = false;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // IMPROVEMENT: Validate references before spawning
        if (blue_goti == null)
        {
            Debug.LogError("Blue goti prefab not assigned!");
            return;
        }
        if (spawn_area == null || spawn_area.Length == 0)
        {
            Debug.LogError("Blue spawn area not assigned!");
            return;
        }
        if (blue_path == null || blue_path.Length == 0)
        {
            Debug.LogError("Blue path not assigned!");
            return;
        }

        Spawn();
    }

    /// <summary>
    /// Spawn all gotis for blue player
    /// IMPROVEMENT: Better error handling
    /// </summary>
    public void Spawn()
    {
        for (int i = 0; i < spawn_area.Length; i++)
        {
            // IMPROVEMENT: Null check for spawn position
            if (spawn_area[i] == null)
            {
                Debug.LogWarning("Spawn area " + i + " is null!");
                continue;
            }

            GameObject gotiInstance = Instantiate(blue_goti, spawn_area[i].transform.position, Quaternion.identity);
            gotiInstance.transform.parent = this.transform;
            
            // IMPROVEMENT: Get component with null check
            goti gotiScript = gotiInstance.GetComponent<goti>();
            if (gotiScript != null)
            {
                gotiScript.path = blue_path;
            }
            else
            {
                Debug.LogError("Spawned goti doesn't have goti component!");
            }
        }

        // OPTIMIZATION: Cache gotis after spawning for faster lookups
        CacheGotis();
    }

    /// <summary>
    /// Cache all goti components for faster subsequent checks
    /// OPTIMIZATION: Reduces GetComponent calls in loops
    /// </summary>
    private void CacheGotis()
    {
        cachedGotis = new goti[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            goti g = transform.GetChild(i).GetComponent<goti>();
            if (g != null)
                cachedGotis[i] = g;
        }
        gotisInitialized = true;
    }

    /// <summary>
    /// Check if any token is unlocked
    /// OPTIMIZATION: Uses cached gotis instead of repeated GetComponent calls
    /// </summary>
    public bool AnyGotiUnlocked()
    {
        // IMPROVEMENT: Use cached gotis if available
        if (gotisInitialized && cachedGotis != null)
        {
            for (int i = 0; i < cachedGotis.Length; i++)
            {
                if (cachedGotis[i] != null && cachedGotis[i].unlocked)
                {
                    return true;
                }
            }
            return false;
        }

        // FALLBACK: If cache not ready, use regular loop
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

    /// <summary>
    /// Check if all tokens are locked (at home)
    /// OPTIMIZATION: Uses cached gotis instead of repeated GetComponent calls
    /// </summary>
    public bool AllGotisLocked()
    {
        // IMPROVEMENT: Use cached gotis if available
        if (gotisInitialized && cachedGotis != null)
        {
            for (int i = 0; i < cachedGotis.Length; i++)
            {
                if (cachedGotis[i] != null && cachedGotis[i].unlocked)
                {
                    return false;
                }
            }
            return true;
        }

        // FALLBACK: If cache not ready, use regular loop
        foreach (Transform gotiTransform in this.transform)
        {
            goti gotiScript = gotiTransform.GetComponent<goti>();
            if (gotiScript != null && gotiScript.unlocked)
            {
                return false;
            }
        }
        return true;
    }
}
