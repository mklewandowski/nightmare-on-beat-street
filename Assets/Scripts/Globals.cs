using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    public static bool AudioOn = true;
    public static int BestScore = 0;
    public const string BestScorePlayerPrefsKey = "BestScore";

    public enum GameStates {
        Title,
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
        ZombieRun2
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
    
}
