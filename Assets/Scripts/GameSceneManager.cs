using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        "How did this start? You were dancing. Everything was normal. Then blistering light... followed by darkness... revolting, ghoulish creatures appeared. They took your friends.", 
        "And now it's just you. And you must DANCE. It's the only thing that keeps them away... that keeps YOU alive."
    };
    // string[] introText = {
    //     "start"
    // };
    int introIndex = 0;

    [SerializeField]
    GameObject SummaryText;
    [SerializeField]
    GameObject TryAgainButton;
    [SerializeField]
    GameObject HomeButton;
    [SerializeField]
    GameObject SummaryScore;
    [SerializeField]
    TextMeshProUGUI SummaryScoreText;
    [SerializeField]
    TextMeshProUGUI SummaryScoreRearText;
    [SerializeField]
    GameObject SummaryBestScore;
    [SerializeField]
    TextMeshProUGUI SummaryBestScoreText;
    [SerializeField]
    TextMeshProUGUI SummaryBestScoreRearText;
    [SerializeField]
    GameObject Level;

    [SerializeField]
    GameObject Player;
    [SerializeField]
    GameObject PlayerShadow;
    [SerializeField]
    GameObject[] Highlights;
    [SerializeField]
    GameObject[] Glows;
    [SerializeField]
    GameObject RowPrefab;
    [SerializeField]
    GameObject RowContainer;
    [SerializeField]
    GameObject Rate;
    [SerializeField]
    TextMeshProUGUI RateText;
    [SerializeField]
    TextMeshProUGUI RateRearText;
    [SerializeField]
    GameObject Combo;
    [SerializeField]
    TextMeshProUGUI ComboText;
    [SerializeField]
    TextMeshProUGUI ComboRearText;
    [SerializeField]
    GameObject PerfectCombo;
    [SerializeField]
    TextMeshProUGUI PerfectComboText;
    [SerializeField]
    TextMeshProUGUI PerfectComboRearText;
    [SerializeField]
    GameObject HitPointPlus;
    [SerializeField]
    GameObject KillAllEnemies;
    [SerializeField]
    GameObject Explosion;

    [SerializeField]
    GameObject EnemyPrefab;
    [SerializeField]
    GameObject EnemyContainer;
   [SerializeField]
    GameObject LifeBarContainer;
    [SerializeField]
    GameObject LifeBar;
    [SerializeField]
    TextMeshProUGUI GameTime;
    [SerializeField]
    TextMeshProUGUI GameScore;

    [SerializeField]
    GameObject BloodPrefab;
    [SerializeField]
    GameObject AttackPrefab;
    [SerializeField]
    GameObject FlashPrefab;
    [SerializeField]
    GameObject Ready;
    [SerializeField]
    TextMeshProUGUI ReadyText;
    [SerializeField]
    TextMeshProUGUI ReadyRearText;
    [SerializeField]
    GameObject GameOver;
    [SerializeField]
    GameObject MobileButtons;
    [SerializeField]
    TextMeshProUGUI MobileToggleButtonText;
    bool showMobileButtons = true;
    [SerializeField]
    GameObject TutorialPanel;

    List<GameObject> RowPool = new List<GameObject>();
    List<Row> RowPoolScripts = new List<Row>();
    List<GameObject> Rows = new List<GameObject>();
    List<Row> RowScripts = new List<Row>();
    List<GameObject> EnemyPool = new List<GameObject>();
    List<GameObject> Enemies = new List<GameObject>();
    List<GameObject> EnemiesMissed = new List<GameObject>();
    List<GameObject> EnemiesOops = new List<GameObject>();
    List<Enemy> EnemyPoolScripts = new List<Enemy>();
    List<Enemy> EnemiesScripts = new List<Enemy>();
    List<Enemy> EnemiesMissedScripts = new List<Enemy>();
    List<Enemy> EnemiesOopsScripts = new List<Enemy>();

    float rowTimer = 0;
    float rowTimerMax = 1.8f;
    float inGoodThreshold = -70f;
    float inGreatThreshold = -58f;
    float inPerfectThreshold = -46f;
    float destroyThreshold = -34f;
    int combo = 0;
    int perfectCombo = 0;
    int addHitPointThreshold = 6;
    int killAllThreshold = 20;
    int life = 6;
    float lifebarMaxWidth = 43f;
    int maxLife = 6;
    float gameTime = 0;
    float speedTimer = 10f;
    float maxSpeedTimer = 8f; // changes to 10
    float rowSpeed = 50f;
    float enemySpeed = 50f;
    int gameScore = 0;

    Coroutine RateCoroutine;
    Color goodColor = new Color(255f/255f, 216f/255f, 0/255f);
    Color badColor = new Color(240f/255f, 16f/255f, 16f/255f);

    float readyTimer = 2f;
    float readyTimerMax = 1f;
    string[] readyStrings = {"3", "2", "1", "GO"};
    int readyIndex = 0;

    float gameOverTimer = 3f;
    float gameOverTimerMax = 3f;

    void Awake()
    {
        Application.targetFrameRate = 60;
        Globals.BestScore = Globals.LoadIntFromPlayerPrefs(Globals.BestScorePlayerPrefsKey);
        int showMobile = Globals.LoadIntFromPlayerPrefs(Globals.ShowMobileButtonsPlayerPrefsKey, 1);
        showMobileButtons = showMobile == 1;
        SetMobileButtons();
        audioManager = this.GetComponent<AudioManager>();

        for (int x = 0; x < 10; x++)
        {
            GameObject row = Instantiate(RowPrefab, new Vector3(0, -100f, 0), Quaternion.identity, RowContainer.transform);
            RectTransform rt = row.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y);
            rt.transform.localPosition = new Vector3(rt.transform.localPosition.x, -189f, rt.transform.localPosition.z);
            RowPool.Add(row);
            RowPoolScripts.Add(row.GetComponent<Row>());
        }
        for (int x = 0; x < 10; x++)
        {
            GameObject enemy = Instantiate(EnemyPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            EnemyPool.Add(enemy);
            EnemyPoolScripts.Add(enemy.GetComponent<Enemy>());
        }
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
        HandleTitle();
        HandleIntro();
        GetReady();
        PlayGame();
        ShowGameOver();
        HandleSummary();
    }

    void HandleTitle()
    {
        if (Globals.CurrentGameState != Globals.GameStates.Title)
            return; 

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (showTutorial)
                HideTutorial();
            else
                SelectStartButton();
        }
    }
    bool typing = true;
    void HandleIntro()
    {
        if (Globals.CurrentGameState != Globals.GameStates.Intro || typing)
            return; 

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            AdvanceIntro();
        }
    }
    bool isPlayAgain = true;
    void HandleSummary()
    {
        if (Globals.CurrentGameState != Globals.GameStates.Summary || typing)
            return; 

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isPlayAgain)
                SelectPlayAgainButton();
            else
                SelectHomeButton();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            isPlayAgain = false;
            audioManager.PlayMenuSound();
            HomeButton.GetComponent<Image>().color = new Color(255f/255f, 151f/255f, 151f/255f);
            TryAgainButton.GetComponent<Image>().color = Color.white;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            isPlayAgain = true;
            audioManager.PlayMenuSound();
            TryAgainButton.GetComponent<Image>().color = new Color(255f/255f, 151f/255f, 151f/255f);
            HomeButton.GetComponent<Image>().color = Color.white;
        }
    }

    void GetReady()
    {
        if (Globals.CurrentGameState != Globals.GameStates.GetReady)
            return;

        readyTimer -= Time.deltaTime;
        if (readyTimer <= 0)
        {
            readyIndex++;
            if (readyIndex < readyStrings.Length)
            {
                ReadyText.text = readyStrings[readyIndex];
                ReadyRearText.text = readyStrings[readyIndex];
                readyTimer = readyTimerMax;
                if (readyIndex < readyStrings.Length - 1)
                    audioManager.PlayCountdownSound();
                else
                    audioManager.PlayStartSound();
            }
            else
            {
                Ready.SetActive(false);
                Globals.CurrentGameState = Globals.GameStates.Playing;
                Player.GetComponent<Player>().SetDance();
                audioManager.StartMusic();
            }
        }
    }

    void PlayGame()
    {
        if (Globals.CurrentGameState != Globals.GameStates.Playing)
            return;
        HandleInput();
        MoveRows();
        MoveEnemies();
        HandleRowCreation();
        HandleTime();
        HandleSpeed();
    }

    void ShowGameOver()
    {
        if (Globals.CurrentGameState != Globals.GameStates.GameOver)
            return;

        gameOverTimer -= Time.deltaTime;
        if (gameOverTimer <= 0)
        {
            Level.GetComponent<MoveNormal>().MoveDown();
            string summary = "You survived for " + gameTime.ToString("0.0") + " seconds, ";
            string[] results = {
                "and then a blood-sucking ghoul swallowed your organs.",
                "and then you succumbed to the nightmare.",
                "and then then you were dragged down into the lair of the beasts.",
                "and then your soul was wrenched from your lifeless corpse.",
                "and then a fiendish beast dined on your innards.",
                "and then the creatures ripped you limb from limb.",
                "and then the creatures tore the flesh from your face.",
                "and then the monsters plucked your eyeballs from your face.",
                "and then the monsters devoured your flesh.",
                "and then the monsters ate the flesh from your face.",
                "and then the monsters disemboweled you.",
                "and then the creatures sucked the blood from your body.",
                "and then the creatures tore through your chest cavity and devoured your heart.",
                "and then the creatures gouged your eyes and face until you slowly bled to death."
            };
            summary += results[Random.Range(0, results.Length)];
            SummaryText.GetComponent<TypewriterUI>().StartEffect("", Globals.StringWithBreaks(summary, 25));
            typing = true;
            Globals.CurrentGameState = Globals.GameStates.Summary;
            audioManager.StartAmbient();
        }
    }

    public void SelectStartButton()
    {      
        audioManager.StopMusic();
        audioManager.PlayButtonSound();
        Title.GetComponent<MoveNormal>().MoveUp();
        TitleButtons.GetComponent<MoveNormal>().MoveDown();   
        IntroText.GetComponent<TypewriterUI>().StartEffect("", Globals.StringWithBreaks(introText[introIndex], 25));
        typing = true;
        Globals.CurrentGameState = Globals.GameStates.Intro;
    }

    public void SelectPlayAgainButton()
    {
        audioManager.StopMusic();
        audioManager.PlayButtonSound();
        SummaryText.GetComponent<TextMeshProUGUI>().text = "";
        SummaryBestScore.SetActive(false);
        SummaryScore.SetActive(false);
        TryAgainButton.GetComponent<MoveNormal>().MoveDown();
        HomeButton.GetComponent<MoveNormal>().MoveDown();
        StartGetReady();
    }

    public void SelectHomeButton()
    {
        audioManager.PlayButtonSound();
        audioManager.StartIntroMusic();
        SummaryText.GetComponent<TextMeshProUGUI>().text = "";
        SummaryBestScore.SetActive(false);
        SummaryScore.SetActive(false);
        TryAgainButton.GetComponent<MoveNormal>().MoveDown();
        HomeButton.GetComponent<MoveNormal>().MoveDown();
        Title.GetComponent<MoveNormal>().MoveDown();
        TitleButtons.GetComponent<MoveNormal>().MoveUp();   
        introIndex = 0;
        Globals.CurrentGameState = Globals.GameStates.Title;
    }

    public void EndText()
    {
        if (Globals.CurrentGameState == Globals.GameStates.Intro)
        {
            if (introIndex >= introText.Length - 1)
                NextButtonText.text = "PLAY";
            NextButton.GetComponent<MoveNormal>().MoveUp();
        }
        else if (Globals.CurrentGameState == Globals.GameStates.Summary)
        {
            TryAgainButton.GetComponent<MoveNormal>().MoveUp();
            HomeButton.GetComponent<MoveNormal>().MoveUp();
            SummaryScoreText.text = gameScore.ToString();
            SummaryScoreRearText.text = SummaryScoreText.text;
            SummaryScore.transform.localScale = new Vector3(.1f, .1f, .1f);
            SummaryScore.SetActive(true);
            SummaryScore.GetComponent<GrowAndShrink>().StartEffect();

            if (gameScore > Globals.BestScore)
            {
                Globals.BestScore = gameScore;
                Globals.SaveIntToPlayerPrefs(Globals.BestScorePlayerPrefsKey, Globals.BestScore);
            }

            SummaryBestScoreText.text = Globals.BestScore.ToString();
            SummaryBestScoreRearText.text = SummaryBestScoreText.text;
            SummaryBestScore.transform.localScale = new Vector3(.1f, .1f, .1f);
            SummaryBestScore.SetActive(true);
            SummaryBestScore.GetComponent<GrowAndShrink>().StartEffect();
        }
        typing = false;
    }

    public void AdvanceIntro()
    {
        audioManager.PlayButtonSound();
        introIndex++;
        if (introIndex < introText.Length)
        {
            IntroText.GetComponent<TypewriterUI>().StartEffect("", Globals.StringWithBreaks(introText[introIndex], 25));
            NextButton.GetComponent<MoveNormal>().MoveDown();
            typing = true;
        }
        else
        {
            IntroText.GetComponent<TextMeshProUGUI>().text = "";
            NextButton.GetComponent<MoveNormal>().MoveDown();
            StartGetReady();
        }
    }

    void StartGetReady()
    {
        Ready.SetActive(true);
        readyIndex = 0;
        readyTimer = readyTimerMax;
        ReadyText.text = readyStrings[readyIndex];
        ReadyRearText.text = readyStrings[readyIndex];
        Globals.CurrentGameState = Globals.GameStates.GetReady;
        Player.GetComponent<Player>().SetIdle();
        Player.SetActive(true);
        PlayerShadow.SetActive(true);
        life = maxLife;
        float newLifebarWidth = lifebarMaxWidth * (float)life / (float)maxLife;
        LifeBar.GetComponent<RectTransform>().sizeDelta = new Vector2(newLifebarWidth, 7f);
        gameScore = 0;
        gameTime = 0;
        GameScore.text = "<mspace=.6em>" + gameScore.ToString();
        GameTime.text = "<mspace=.6em>" + gameTime.ToString("0.0");
        GameOver.SetActive(false);
        rowSpeed = 50f;
        enemySpeed = 50f;
        rowTimerMax = 1.8f;
        maxSpeedTimer = 8f;
        speedTimer = maxSpeedTimer;
        Level.GetComponent<MoveNormal>().MoveUp();
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
        if (Globals.CurrentGameState != Globals.GameStates.Playing)
            return;
        if (Rows.Count > 0 && RowScripts[0].CurrentScoreQuality != Globals.ScoreQualities.Invalid)
        {
            if (RowScripts[0].Orientation == inputOrientation)
            {
                if (RowScripts[0].CurrentScoreQuality == Globals.ScoreQualities.Good)
                {
                    if (RateCoroutine != null) StopCoroutine(RateCoroutine);
                    RateCoroutine = StartCoroutine(ShowRate("GOOD", goodColor));
                    AddScore(5);
                    combo++;
                    if (combo > 1)
                        ShowCombo();
                    perfectCombo = 0;
                    HidePerfectCombo();
                }
                else if (RowScripts[0].CurrentScoreQuality == Globals.ScoreQualities.Great)
                {
                    if (RateCoroutine != null) StopCoroutine(RateCoroutine);
                    RateCoroutine = StartCoroutine(ShowRate("GREAT", goodColor));
                    AddScore(10);
                    combo++;
                    if (combo > 1)
                        ShowCombo();
                    perfectCombo = 0;
                    HidePerfectCombo();
                }
                else if (RowScripts[0].CurrentScoreQuality == Globals.ScoreQualities.Perfect)
                {
                    if (RateCoroutine != null) StopCoroutine(RateCoroutine);
                    RateCoroutine = StartCoroutine(ShowRate("PERFECT", goodColor));
                    AddScore(20);
                    combo++;
                    perfectCombo++;
                    if (combo > 1)
                        ShowCombo();
                    if (perfectCombo > 1)
                        ShowPerfectCombo();
                    if (perfectCombo > 0 && perfectCombo % addHitPointThreshold == 0)
                    {
                        AddHitPoints();
                    }
                }
                StartCoroutine(ShowHighlight(RowScripts[0].Orientation, Color.yellow, .15f, .3f));
                AttackEnemy(Enemies[0].GetComponent<RectTransform>().anchoredPosition, inputOrientation == Globals.Orientations.Right || inputOrientation == Globals.Orientations.Up);
                EnemiesScripts[0].DeActivate();
                Enemies.RemoveAt(0);
                EnemiesScripts.RemoveAt(0);
                audioManager.PlayHitEnemySound();            
            }
            else 
            {
                audioManager.PlayBadInputSound();
                StartCoroutine(ShowHighlight(RowScripts[0].Orientation, badColor, .15f, .3f));
                if (RateCoroutine != null) StopCoroutine(RateCoroutine);
                RateCoroutine = StartCoroutine(ShowRate("WRONG", badColor));
                combo = 0;
                HideCombo();
                perfectCombo = 0;
                HidePerfectCombo();
                EnemiesMissed.Add(Enemies[0]);
                EnemiesMissedScripts.Add(EnemiesScripts[0]);
                Enemies.RemoveAt(0);
                EnemiesScripts.RemoveAt(0);
            }
            RowScripts[0].DeActivate();
            Rows.RemoveAt(0);
            RowScripts.RemoveAt(0);

            if (combo > 0 && combo % killAllThreshold == 0)
            {
                ClearEnemies();
            }    
        }
        else
        {
            audioManager.PlayBadInputSound();
            if (RateCoroutine != null) StopCoroutine(RateCoroutine);
            RateCoroutine = StartCoroutine(ShowRate("OOPS", badColor));
            combo = 0;
            HideCombo();
            perfectCombo = 0;
            HidePerfectCombo();
            CreateOopsEnemy(inputOrientation);
        }
    }

    void AttackEnemy(Vector2 pos, bool flip)
    {
        GameObject aGO = Instantiate(AttackPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
        aGO.GetComponent<RectTransform>().anchoredPosition = pos;
        if (flip)
            aGO.transform.localEulerAngles = new Vector3(0, 180f, 0);
        GameObject bGO = Instantiate(BloodPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
        bGO.GetComponent<RectTransform>().anchoredPosition = pos;
        if (flip)
            bGO.transform.localEulerAngles = new Vector3(0, 180f, 0);
    }

    void MoveRows()
    {
        bool deleteFirst = false;
        int index = 0;
        foreach (GameObject r in Rows)
        {
            r.transform.localPosition = new Vector3(r.transform.localPosition.x, r.transform.localPosition.y + rowSpeed * Time.deltaTime, r.transform.localPosition.z);
            if (r.transform.localPosition.y >= inGoodThreshold && r.transform.localPosition.y < inGreatThreshold && RowScripts[index].CurrentScoreQuality != Globals.ScoreQualities.Good)
            {
                RowScripts[index].SetGood();
            }
            else if (r.transform.localPosition.y >= inGreatThreshold && r.transform.localPosition.y < inPerfectThreshold && RowScripts[index].CurrentScoreQuality != Globals.ScoreQualities.Great)
            {
                RowScripts[index].SetGreat();
            }
            else if (r.transform.localPosition.y >= inPerfectThreshold && r.transform.localPosition.y < destroyThreshold && RowScripts[index].CurrentScoreQuality != Globals.ScoreQualities.Perfect)
            {
                RowScripts[index].SetPerfect();
            }
            else if (r.transform.localPosition.y >= destroyThreshold)
            {
                deleteFirst = true;
            }
            index++;
        }
        if (deleteFirst)
        {
            audioManager.PlayBadInputSound();
            if (RateCoroutine != null) StopCoroutine(RateCoroutine);
            RateCoroutine = StartCoroutine(ShowRate("MISS", badColor));
            combo = 0;
            HideCombo();
            perfectCombo = 0;
            HidePerfectCombo();

            RowScripts[0].DeActivate();
            Rows.RemoveAt(0);
            RowScripts.RemoveAt(0);

            EnemiesMissed.Add(Enemies[0]);
            EnemiesMissedScripts.Add(EnemiesScripts[0]);
            Enemies.RemoveAt(0);
            EnemiesScripts.RemoveAt(0);
        }
    }

    void MoveEnemies()
    {
        bool deleteFirst = false;
        int index = 0;
        foreach (GameObject e in Enemies)
        {
            float xSpeed = 0;
            float ySpeed = enemySpeed * -1f;
            if (EnemiesScripts[index].StartPosition == Globals.StartPositions.Left)
            {
                ySpeed = 0;
                xSpeed = enemySpeed;
            }
            else if (EnemiesScripts[index].StartPosition == Globals.StartPositions.Right)
            {
                ySpeed = 0;
                xSpeed = enemySpeed * -1f;
            }
            e.transform.localPosition = new Vector3(e.transform.localPosition.x + xSpeed * Time.deltaTime, e.transform.localPosition.y + ySpeed * Time.deltaTime, e.transform.localPosition.z);
            index++;
        }
        index = 0;
        foreach (GameObject e in EnemiesMissed)
        {
            float xSpeed = 0;
            float ySpeed = enemySpeed * -1f;
            if (EnemiesMissedScripts[index].StartPosition == Globals.StartPositions.Left)
            {
                ySpeed = 0;
                xSpeed = enemySpeed;
                if (Mathf.Abs(e.transform.localPosition.x) < 10f)
                    deleteFirst = true;
            }
            else if (EnemiesMissedScripts[index].StartPosition == Globals.StartPositions.Right)
            {
                ySpeed = 0;
                xSpeed = enemySpeed * -1f;
                if (Mathf.Abs(e.transform.localPosition.x) < 10f)
                    deleteFirst = true;
            }
            else
            {
                if (e.transform.localPosition.y < 70f)
                    deleteFirst = true;
            }
            e.transform.localPosition = new Vector3(e.transform.localPosition.x + xSpeed * Time.deltaTime, e.transform.localPosition.y + ySpeed * Time.deltaTime, e.transform.localPosition.z);
            index++;
        }
        if (deleteFirst)
        {
            GameObject bGO = Instantiate(FlashPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            bGO.GetComponent<RectTransform>().anchoredPosition = EnemiesMissed[0].GetComponent<RectTransform>().anchoredPosition;
            EnemiesMissedScripts[0].DeActivate();
            EnemiesMissed.RemoveAt(0);
            EnemiesMissedScripts.RemoveAt(0);
            HitPlayer();
        }

        deleteFirst = false;
        foreach (GameObject e in EnemiesOops)
        {
            float xSpeed = enemySpeed * -3.5f;
            float ySpeed = enemySpeed * -3.5f;
            if (Mathf.Abs(e.transform.localPosition.x) < 10f)
                deleteFirst = true;
            if (e.transform.localPosition.y < 70f)
                deleteFirst = true;
            e.transform.localPosition = new Vector3(e.transform.localPosition.x + xSpeed * Time.deltaTime, e.transform.localPosition.y + ySpeed * Time.deltaTime, e.transform.localPosition.z);
        }
        if (deleteFirst)
        {
            GameObject bGO = Instantiate(FlashPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            bGO.GetComponent<RectTransform>().anchoredPosition = EnemiesOops[0].GetComponent<RectTransform>().anchoredPosition;
            EnemiesOopsScripts[0].DeActivate();
            EnemiesOops.RemoveAt(0);
            EnemiesOopsScripts.RemoveAt(0);
            HitPlayer();
        }
    }

    void HitPlayer()
    {
        audioManager.PlayHitPlayerSound();
        audioManager.PlayHitEnemySound();
        life--;
        float newLifebarWidth = lifebarMaxWidth * (float)life / (float)maxLife;
        LifeBar.GetComponent<RectTransform>().sizeDelta = new Vector2(newLifebarWidth, 7f);

        GameObject bGO = Instantiate(BloodPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
        bGO.GetComponent<RectTransform>().anchoredPosition = Player.GetComponent<RectTransform>().anchoredPosition;

        Player.GetComponent<Player>().Hit();

        if (life == 0)
            EndGame();
    }

    void EndGame()
    {
        gameOverTimer = gameOverTimerMax;
        Globals.CurrentGameState = Globals.GameStates.GameOver;
        audioManager.StopMusic();
        audioManager.PlayHitPlayerSound();
        audioManager.PlayHitEnemySound();
        audioManager.PlayDeathSound();
        GameOver.transform.localScale = new Vector3(.1f, .1f, .1f);
        GameOver.SetActive(true);
        GameOver.GetComponent<GrowAndShrink>().StartEffect();
        Player.SetActive(false);
        PlayerShadow.SetActive(false);
        ClearArrowsAndEnemies();
    }

    void ClearArrowsAndEnemies()
    {
        int index = 0;
        foreach (GameObject e in Enemies)
        {
            GameObject bGO = Instantiate(FlashPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            bGO.GetComponent<RectTransform>().anchoredPosition = e.GetComponent<RectTransform>().anchoredPosition;
            EnemiesScripts[index].DeActivate();
            index++;
        }
        index = 0;
        foreach (GameObject e in EnemiesMissed)
        {
            GameObject bGO = Instantiate(FlashPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            bGO.GetComponent<RectTransform>().anchoredPosition = e.GetComponent<RectTransform>().anchoredPosition;
            EnemiesMissedScripts[index].DeActivate();
            index++;
        }
        index = 0;
        foreach (GameObject e in EnemiesOops)
        {
            GameObject bGO = Instantiate(FlashPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            bGO.GetComponent<RectTransform>().anchoredPosition = e.GetComponent<RectTransform>().anchoredPosition;
            EnemiesOopsScripts[index].DeActivate();
            index++;
        }
        index = 0;
        foreach (GameObject r in Rows)
        {
            GameObject bGO = Instantiate(FlashPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            bGO.GetComponent<RectTransform>().anchoredPosition = r.GetComponent<RectTransform>().anchoredPosition;
            RowScripts[index].DeActivate();
            index++;
        }
        Enemies.Clear();
        EnemiesMissed.Clear();
        EnemiesOops.Clear();
        EnemiesScripts.Clear();
        EnemiesMissedScripts.Clear();
        EnemiesOopsScripts.Clear();
        Rows.Clear();
        RowScripts.Clear();
    }

    void HandleTime()
    {
        gameTime += Time.deltaTime;
        GameTime.text = "<mspace=.6em>" + gameTime.ToString("0.0");
    }

    void AddScore(int num)
    {
        gameScore += num;
        GameScore.text = "<mspace=.6em>" + gameScore.ToString();
        GameScore.gameObject.GetComponent<GrowAndShrink>().StartEffect();
    }

    void HandleSpeed()
    {
        speedTimer -= Time.deltaTime;
        if (speedTimer <= 0)
        {
            float increment = 10f;
            float decrement = .2f;
            if (rowSpeed >= 80f)
            {
                maxSpeedTimer = 10f;
                increment = 15f;
                decrement = .1f;
            }
            rowSpeed = Mathf.Min(215f, rowSpeed + increment);
            enemySpeed = Mathf.Min(215f, enemySpeed + increment);
            rowTimerMax = Mathf.Max(.5f, rowTimerMax - decrement);
            speedTimer = maxSpeedTimer;
        }
    }

    public void HandleRowCreation()
    {
        rowTimer -= Time.deltaTime;
        if (rowTimer <= 0)
        {
            CreateRow();
            rowTimer = rowTimerMax;
        }
    }

    void CreateRow()
    {
        Globals.Orientations newOrientation = (Globals.Orientations)Random.Range(0, 4);
        
        if (newOrientation != Globals.Orientations.None)
        {
            for (int x = 0; x < RowPool.Count; x++)
            {
                if (!RowPoolScripts[x].InUse)
                {
                    RectTransform rt = RowPool[x].GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y);
                    rt.transform.localPosition = new Vector3(rt.transform.localPosition.x, -189f, rt.transform.localPosition.z);
                    RowPoolScripts[x].Activate(newOrientation);
                    Rows.Add(RowPool[x]);
                    RowScripts.Add(RowPoolScripts[x]);
                    break;
                }
            }            
            CreateEnemy(newOrientation);
        }
    }

    void CreateEnemy(Globals.Orientations newOrientation)
    {
        for (int x = 0; x < EnemyPool.Count; x++)
        {
            if (!EnemyPoolScripts[x].InUse)
            {
                RectTransform rt = EnemyPool[x].GetComponent<RectTransform>();
                float newY = 245f;
                if (newOrientation == Globals.Orientations.Left || newOrientation == Globals.Orientations.Right)
                    newY = 50f;
                float newX = 0f;
                if (newOrientation == Globals.Orientations.Left)
                    newX = -185f;
                else if (newOrientation == Globals.Orientations.Right)
                    newX = 185f;
                rt.anchoredPosition = new Vector2(newX, newY);
                EnemyPoolScripts[x].Activate(newOrientation);
                Enemies.Add(EnemyPool[x]);
                EnemiesScripts.Add(EnemyPoolScripts[x]);
                break;
            }
        } 
    }

    void CreateOopsEnemy(Globals.Orientations newOrientation)
    {
        for (int x = 0; x < EnemyPool.Count; x++)
        {
            if (!EnemyPoolScripts[x].InUse)
            {
                RectTransform rt = EnemyPool[x].GetComponent<RectTransform>();
                float newY = 210f;
                float newX = 165f;
                rt.anchoredPosition = new Vector2(newX, newY);
                EnemyPoolScripts[x].Activate(newOrientation);
                EnemyPoolScripts[x].SetType(Globals.EnemyTypes.Pumpkin);
                EnemiesOops.Add(EnemyPool[x]);
                EnemiesOopsScripts.Add(EnemyPoolScripts[x]);
                break;
            }
        } 
    }

    IEnumerator ShowHighlight(Globals.Orientations o, Color c, float inTime, float outTime)
    {
        int index = (int)o;
        float maxInTime = inTime;
        float maxOutTime = outTime;
        Highlights[index].GetComponent<Image>().color = c;
        Glows[index].GetComponent<Image>().color = c;
        Highlights[index].SetActive(true);
        Glows[index].SetActive(true);
        while (inTime >= 0.0f) 
        {
            Glows[index].GetComponent<Image>().color = new Color(c.r, c.g, c.b, 1f - inTime / maxInTime);
            inTime -= Time.deltaTime;
            yield return null; 
        }
        while (outTime >= 0.0f) 
        {
            Glows[index].GetComponent<Image>().color = new Color(c.r, c.g, c.b, outTime / maxOutTime);
            Highlights[index].GetComponent<Image>().color = new Color(c.r, c.g, c.b, outTime / maxOutTime);
            outTime -= Time.deltaTime;
            yield return null; 
        }
        Highlights[index].SetActive(false);
        Glows[index].SetActive(false);
    }

    IEnumerator ShowRate (string text, Color c)
    {
        float maxTime = .7f;
        RateText.color = c;
        RateText.text = text;
        RateRearText.text = text;
        Rate.transform.localScale = new Vector3(.1f, .1f, .1f);
        Rate.SetActive(true);
        Rate.GetComponent<GrowAndShrink>().StartEffect();
        while (maxTime >= 0.0f) 
        {
            maxTime -= Time.deltaTime;
            yield return null; 
        }
        Rate.SetActive(false);
    }

    void ShowCombo()
    {
        ComboText.text = combo.ToString();
        ComboRearText.text = combo.ToString();
        Combo.transform.localScale = new Vector3(.1f, .1f, .1f);
        Combo.SetActive(true);
        Combo.GetComponent<GrowAndShrink>().StartEffect();
    }

    void HideCombo()
    {
        Combo.SetActive(false);
    }

    void ShowPerfectCombo()
    {
        PerfectComboText.text = perfectCombo.ToString();
        PerfectComboRearText.text = perfectCombo.ToString();
        PerfectCombo.transform.localScale = new Vector3(.1f, .1f, .1f);
        PerfectCombo.SetActive(true);
        PerfectCombo.GetComponent<GrowAndShrink>().StartEffect();
    }

    void HidePerfectCombo()
    {
        PerfectCombo.SetActive(false);
    }

    void AddHitPoints()
    {
        audioManager.PlayAddHitPointSound();
        life++;
        if (life > maxLife)
            life = maxLife;
        float newLifebarWidth = lifebarMaxWidth * (float)life / (float)maxLife;
        LifeBar.GetComponent<RectTransform>().sizeDelta = new Vector2(newLifebarWidth, 7f);
        LifeBarContainer.GetComponent<GrowAndShrink>().StartEffect();
        AddScore(100);
        StartCoroutine(ShowHitPointsPlus());
    }

    void ClearEnemies()
    {
        Explosion.GetComponent<GrowAndShrink>().StartEffect();
        audioManager.PlayClearAllSound();
        ClearArrowsAndEnemies();
        AddScore(100);
        StartCoroutine(ShowKillAllEnemies());
    }

    IEnumerator ShowKillAllEnemies()
    {
        float maxTime = 1f;
        KillAllEnemies.transform.localScale = new Vector3(.1f, .1f, .1f);
        KillAllEnemies.SetActive(true);
        KillAllEnemies.GetComponent<GrowAndShrink>().StartEffect();
        while (maxTime >= 0.0f) 
        {
            maxTime -= Time.deltaTime;
            yield return null; 
        }
        KillAllEnemies.SetActive(false);
    }

    IEnumerator ShowHitPointsPlus()
    {
        float maxTime = 1f;
        HitPointPlus.transform.localScale = new Vector3(.1f, .1f, .1f);
        HitPointPlus.SetActive(true);
        HitPointPlus.GetComponent<GrowAndShrink>().StartEffect();
        while (maxTime >= 0.0f) 
        {
            maxTime -= Time.deltaTime;
            yield return null; 
        }
        HitPointPlus.SetActive(false);
    }

    public void ToggleMobileButtons()
    {
        audioManager.PlayMenuSound();
        showMobileButtons = !showMobileButtons;
        Globals.SaveIntToPlayerPrefs(Globals.ShowMobileButtonsPlayerPrefsKey, showMobileButtons ? 1 : 0);
        SetMobileButtons();
    }

    void SetMobileButtons()
    {
        MobileButtons.SetActive(showMobileButtons);
        MobileToggleButtonText.text = showMobileButtons ? "mobile buttons: ON" : "mobile buttons: OFF";
    }

    bool showTutorial = false;
    public void ShowTutorial()
    {
        audioManager.PlayMenuSound();
        TutorialPanel.GetComponent<MoveNormal>().MoveUp();
        Title.GetComponent<MoveNormal>().MoveUp();
        TitleButtons.GetComponent<MoveNormal>().MoveDown();
        showTutorial = true;
    }
    public void HideTutorial()
    {
        audioManager.PlayMenuSound();
        TutorialPanel.GetComponent<MoveNormal>().MoveDown();
        Title.GetComponent<MoveNormal>().MoveDown();
        TitleButtons.GetComponent<MoveNormal>().MoveUp();
        showTutorial = false;
    }
}
