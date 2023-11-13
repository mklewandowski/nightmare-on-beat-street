using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Row : MonoBehaviour
{
    public Globals.Orientations Orientation;
    public Globals.ScoreQualities CurrentScoreQuality = Globals.ScoreQualities.Invalid;

    [SerializeField]
    GameObject[] Arrows;

    [SerializeField]
    GameObject[] Good;
    [SerializeField]
    GameObject[] Great;
    [SerializeField]
    GameObject[] Perfect;

    public bool InUse = false;

    public void Activate(Globals.Orientations newOrientation)
    {
        SetArrow(newOrientation);
        InUse = true;
        this.gameObject.SetActive(true);
    }
    public void DeActivate()
    {
        InUse = false;
        CurrentScoreQuality = Globals.ScoreQualities.Invalid;
        this.gameObject.SetActive(false);
        foreach (GameObject g in Good)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in Great)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in Perfect)
        {
            g.SetActive(false);
        }
    }

    public void SetArrow(Globals.Orientations newOrientation)
    {
        Orientation = newOrientation;
        for (int i = 0; i < Arrows.Length; i++)
        {
            Arrows[i].SetActive(i == (int)Orientation);
        }
    }

    public void SetGood()
    {
        CurrentScoreQuality = Globals.ScoreQualities.Good;
        foreach (GameObject g in Good)
        {
            g.SetActive(true);
        }
    }
    public void SetGreat()
    {
        CurrentScoreQuality = Globals.ScoreQualities.Great;
        foreach (GameObject g in Great)
        {
            g.SetActive(true);
        }
    }
    public void SetPerfect()
    {
        CurrentScoreQuality = Globals.ScoreQualities.Perfect;
        foreach (GameObject g in Perfect)
        {
            g.SetActive(true);
        }
    }
}
