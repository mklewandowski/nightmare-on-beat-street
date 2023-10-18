using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    public static bool AudioOn = true;
    public static int BestScore = 0;

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
        // ZombieLady,
        // ZombieMan,
    }
    
}
