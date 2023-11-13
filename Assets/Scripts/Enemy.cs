using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Globals.EnemyTypes Type = Globals.EnemyTypes.Bird;
    public Globals.StartPositions StartPosition = Globals.StartPositions.Above;

    public GameObject[] Sprites;

    public bool InUse = false;

    public void Activate(Globals.Orientations orientation)
    {
        ConfigureEnemy(orientation);
        InUse = true;
        this.gameObject.SetActive(true);
    }
    public void DeActivate()
    {
        InUse = false;
        this.gameObject.SetActive(false);
        foreach (GameObject g in Sprites)
        {
            g.SetActive(false);
            g.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void ConfigureEnemy(Globals.Orientations orientation)
    {
        if (orientation == Globals.Orientations.Down || orientation == Globals.Orientations.Up)
        {
            Type = (Globals.EnemyTypes)Random.Range(0, 3);
            StartPosition = Globals.StartPositions.Above;
        }
        else
        {
            Type = (Globals.EnemyTypes)Random.Range(0, 12);
            if (orientation == Globals.Orientations.Left)
            {
                StartPosition = Globals.StartPositions.Left;
                Sprites[(int)Type].transform.localEulerAngles = new Vector3(0, 180f, 0);
            }
            else if (orientation == Globals.Orientations.Right)
            {
                StartPosition = Globals.StartPositions.Right;
            }
        }
            
        Sprites[(int)Type].SetActive(true);
    }

    public void SetType(Globals.EnemyTypes newType)
    {
        Sprites[(int)Type].SetActive(false);
        Type = newType;
        Sprites[(int)Type].SetActive(true);
    }
        
}
