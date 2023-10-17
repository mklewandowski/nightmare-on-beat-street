using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{

    AudioManager audioManager;
    [SerializeField]
    GameObject Title;
    [SerializeField]
    GameObject TitleButtons;
    [SerializeField]
    GameObject IntroText;
    [SerializeField]
    GameObject NextButton;
    [SerializeField]
    TextMeshProUGUI NextButtonText;
    string[] introText = {
        "\"It's just another Halloween. It's just another dance party,\" you tell yourself as sweat drips from your brow and pain shoots through your legs.",
        "How did this start? You were dancing. Everything was normal. Then blistering light... followed by darkness... creatures appeared... dark, ghoulish creatures. They took your friends. Ripped at them, tore at them, devoured them.", 
        "And now it's just you. And you must DANCE. It's the only things that keeps them away... that keeps YOU alive."
    };
    int introIndex = 0;

    void Awake()
    {
        audioManager = this.GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Title.GetComponent<MoveNormal>().MoveDown();
        TitleButtons.GetComponent<MoveNormal>().MoveUp();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectStartButton()
    {      
        audioManager.StopMusic();
        audioManager.PlayButtonSound();
        Title.GetComponent<MoveNormal>().MoveUp();
        TitleButtons.GetComponent<MoveNormal>().MoveDown();   

        IntroText.GetComponent<TypewriterUI>().StartEffect("", introText[introIndex]);
    }

    public void EndText()
    {
        if (introIndex >= introText.Length - 1)
            NextButtonText.text = "START";
        NextButton.GetComponent<MoveNormal>().MoveUp();
    }

    public void AdvanceIntro()
    {
        audioManager.PlayButtonSound();
        introIndex++;
        if (introIndex < introText.Length)
        {
            IntroText.GetComponent<TypewriterUI>().StartEffect("", introText[introIndex]);
            NextButton.GetComponent<MoveNormal>().MoveDown();
        }
        else
        {
            // START GAME
            IntroText.GetComponent<TextMeshProUGUI>().text = "";
            NextButton.GetComponent<MoveNormal>().MoveDown();
        }
    }

    public void HandleInput()
    {
        Globals.Orientations inputOrientation = Globals.Orientations.None;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown("a"))
        {
            inputOrientation = Globals.Orientations.Left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown("d"))
        {
            inputOrientation = Globals.Orientations.Right;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown("w"))
        {
            inputOrientation = Globals.Orientations.Up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown("s"))
        {
            inputOrientation = Globals.Orientations.Down;
        }
        if (inputOrientation != Globals.Orientations.None)
            VetInput(inputOrientation);
    }

    public void SelectUp()
    {
        VetInput(Globals.Orientations.Up);
    }
    public void SelectDown()
    {
        VetInput(Globals.Orientations.Down);
    }
    public void SelectLeft()
    {
        VetInput(Globals.Orientations.Left);
    }
    public void SelectRight()
    {
        VetInput(Globals.Orientations.Right);
    }

    void VetInput(Globals.Orientations inputOrientation)
    {

    }
}
