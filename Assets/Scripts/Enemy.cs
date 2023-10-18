using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Globals.EnemyTypes Type = Globals.EnemyTypes.Bird;
    public Globals.StartPositions StartPosition = Globals.StartPositions.Above;

    public GameObject[] Sprites;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            Type = (Globals.EnemyTypes)Random.Range(3, 10);
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
}
