using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    public static bool AudioOn = true;
    public static int BestScore = 0;
    public const string BestScorePlayerPrefsKey = "BestScore";
    public const string ShowMobileButtonsPlayerPrefsKey = "MobileButtons";

    public enum GameStates {
        Title,
        Intro,
        GetReady,
        Playing,
        GameOver,
        Summary,
    }
    public static GameStates CurrentGameState = GameStates.Title;

    public enum Orientations {
        Left,
        Down,
        Up,
        Right,
        None,
    }

    public enum ScoreQualities {
        Invalid,
        Good,
        Great,
        Perfect
    }
    public enum StartPositions {
        Above,
        Left,
        Right
    }
    public enum EnemyTypes {
        Blob,
        Spider,
        Eye,
        Bird,
        ZombieRun,
        Ghost,
        Pumpkin,
        ZombieLadyRun,
        Robot,
        Drag,
        ZombieRun2,
        ZombieHeadless
    }

    public static void SaveIntToPlayerPrefs(string key, int val)
    {
        PlayerPrefs.SetInt(key, val);
    }
    public static int LoadIntFromPlayerPrefs(string key, int defaultVal = 0)
    {
        int val = PlayerPrefs.GetInt(key, defaultVal);
        return val;
    }

    public static string StringWithBreaks(string text, int charsPerLine)
    {
        string modifiedText = "";
        int charsOnCurrentLine = 0;
        char[] delimiterChars = {' '};
        string[] words = text.Split(delimiterChars);
        for (int index = 0; index < words.Length; index++)
        {
            if ((charsOnCurrentLine + words[index].Length) < charsPerLine)
            {
                modifiedText = modifiedText + words[index] + " ";
                //words still fit on current line
                charsOnCurrentLine += words[index].Length + 1;
            }
            else
            {
                modifiedText = modifiedText + "\n" + words[index] + " ";
                //start a new line
                charsOnCurrentLine = words[index].Length + 1;
            }
        }
        return modifiedText;
    }
    
}
