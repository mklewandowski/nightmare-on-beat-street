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
        "How did this start? You were dancing. Everything was normal. Then blistering light... followed by darkness... foul, ghoulish creatures appeared. They took your friends.", 
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

    List<GameObject> Rows = new List<GameObject>();
    List<GameObject> Enemies = new List<GameObject>();
    List<GameObject> EnemiesMissed = new List<GameObject>();
    List<GameObject> EnemiesOops = new List<GameObject>();

    float rowTimer = 0;
    float rowTimerMax = 1f;
    float inGoodThreshold = -70f;
    float inGreatThreshold = -57f;
    float inPerfectThreshold = -44f;
    float destroyThreshold = -36f;
    int combo = 0;
    int perfectCombo = 0;
    int addHitPointThreshold = 6;
    int killAllThreshold = 20;
    int life = 4;
    float lifebarMaxWidth = 43f;
    int maxLife = 4;
    float gameTime = 0;
    float speedTimer = 10f;
    float maxSpeedTimer = 10f;
    float rowSpeed = 100f;
    float enemySpeed = 60f;
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
        Globals.BestScore = Globals.LoadIntFromPlayerPrefs(Globals.BestScorePlayerPrefsKey);
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
        GetReady();
        PlayGame();
        ShowGameOver();
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
            string summary = "You survived for " + gameTime.ToString("0.0") + " seconds ";
            string[] results = {
                "and then a blood-sucking ghoul swallowed your organs.",
                "and then you succumbed to the nightmare.",
                "and then then you were dragged down into the lair of the beasts.",
                "and then your soul was wrenched from your lifeless corpse.",
                "and then a fiendish beast dined on your innards.",
                "and then the creatures ripped you limb from limb.",
                "and then the creatures tore through your chest cavity and devoured your heart.",
                "and then the creatures gouged your eyes and face until you slowly bled to death."
            };
            summary += results[Random.Range(0, results.Length)];
            SummaryText.GetComponent<TypewriterUI>().StartEffect("", summary);
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
        IntroText.GetComponent<TypewriterUI>().StartEffect("", introText[introIndex]);
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
        if (Globals.CurrentGameState == Globals.GameStates.Title)
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
        rowSpeed = 100f;
        enemySpeed = 60f;
        rowTimerMax = 1f;
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
        if (Rows.Count > 0 && Rows[0].GetComponent<Row>().CurrentScoreQuality != Globals.ScoreQualities.Invalid)
        {
            if (Rows[0].GetComponent<Row>().Orientation == inputOrientation)
            {
                if (Rows[0].GetComponent<Row>().CurrentScoreQuality == Globals.ScoreQualities.Good)
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
                else if (Rows[0].GetComponent<Row>().CurrentScoreQuality == Globals.ScoreQualities.Great)
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
                else if (Rows[0].GetComponent<Row>().CurrentScoreQuality == Globals.ScoreQualities.Perfect)
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
                StartCoroutine(ShowHighlight(Rows[0].GetComponent<Row>().Orientation, Color.yellow, .15f, .3f));
                AttackEnemy(Enemies[0].GetComponent<RectTransform>().anchoredPosition, inputOrientation == Globals.Orientations.Right || inputOrientation == Globals.Orientations.Up);
                Destroy(Enemies[0]);
                Enemies.RemoveAt(0);
                audioManager.PlayHitEnemySound();            
            }
            else 
            {
                audioManager.PlayBadInputSound();
                StartCoroutine(ShowHighlight(Rows[0].GetComponent<Row>().Orientation, badColor, .15f, .3f));
                if (RateCoroutine != null) StopCoroutine(RateCoroutine);
                RateCoroutine = StartCoroutine(ShowRate("WRONG", badColor));
                combo = 0;
                HideCombo();
                perfectCombo = 0;
                HidePerfectCombo();
                EnemiesMissed.Add(Enemies[0]);
                Enemies.RemoveAt(0);
            }
            Destroy(Rows[0]);
            Rows.RemoveAt(0);

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
        foreach (GameObject r in Rows)
        {
            r.transform.localPosition = new Vector3(r.transform.localPosition.x, r.transform.localPosition.y + rowSpeed * Time.deltaTime, r.transform.localPosition.z);
            Row row = r.GetComponent<Row>();
            if (r.transform.localPosition.y >= inGoodThreshold && r.transform.localPosition.y < inGreatThreshold && row.CurrentScoreQuality != Globals.ScoreQualities.Good)
            {
                r.GetComponent<Row>().SetGood();
            }
            else if (r.transform.localPosition.y >= inGreatThreshold && r.transform.localPosition.y < inPerfectThreshold && row.CurrentScoreQuality != Globals.ScoreQualities.Great)
            {
                r.GetComponent<Row>().SetGreat();
            }
            else if (r.transform.localPosition.y >= inPerfectThreshold && r.transform.localPosition.y < destroyThreshold && row.CurrentScoreQuality != Globals.ScoreQualities.Perfect)
            {
                r.GetComponent<Row>().SetPerfect();
            }
            else if (r.transform.localPosition.y >= destroyThreshold)
            {
                deleteFirst = true;
            }
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

            Destroy(Rows[0]);
            Rows.RemoveAt(0);

            EnemiesMissed.Add(Enemies[0]);
            Enemies.RemoveAt(0);
        }
    }

    void MoveEnemies()
    {
        bool deleteFirst = false;
        foreach (GameObject e in Enemies)
        {
            float xSpeed = 0;
            float ySpeed = enemySpeed * -1f;
            Enemy enemy = e.GetComponent<Enemy>();
            if (enemy.StartPosition == Globals.StartPositions.Left)
            {
                ySpeed = 0;
                xSpeed = enemySpeed;
            }
            else if (enemy.StartPosition == Globals.StartPositions.Right)
            {
                ySpeed = 0;
                xSpeed = enemySpeed * -1f;
            }
            e.transform.localPosition = new Vector3(e.transform.localPosition.x + xSpeed * Time.deltaTime, e.transform.localPosition.y + ySpeed * Time.deltaTime, e.transform.localPosition.z);
        }

        foreach (GameObject e in EnemiesMissed)
        {
            float xSpeed = 0;
            float ySpeed = enemySpeed * -1f;
            Enemy enemy = e.GetComponent<Enemy>();
            if (enemy.StartPosition == Globals.StartPositions.Left)
            {
                ySpeed = 0;
                xSpeed = enemySpeed;
                if (Mathf.Abs(e.transform.localPosition.x) < 10f)
                    deleteFirst = true;
            }
            else if (enemy.StartPosition == Globals.StartPositions.Right)
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
        }
        if (deleteFirst)
        {
            GameObject bGO = Instantiate(FlashPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            bGO.GetComponent<RectTransform>().anchoredPosition = EnemiesMissed[0].GetComponent<RectTransform>().anchoredPosition;
            Destroy(EnemiesMissed[0]);
            EnemiesMissed.RemoveAt(0);
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
            Destroy(EnemiesOops[0]);
            EnemiesOops.RemoveAt(0);
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
        foreach (GameObject e in Enemies)
        {
            GameObject bGO = Instantiate(FlashPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            bGO.GetComponent<RectTransform>().anchoredPosition = e.GetComponent<RectTransform>().anchoredPosition;
            Destroy(e);
        }
        foreach (GameObject e in EnemiesMissed)
        {
            GameObject bGO = Instantiate(FlashPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            bGO.GetComponent<RectTransform>().anchoredPosition = e.GetComponent<RectTransform>().anchoredPosition;
            Destroy(e);
        }
        foreach (GameObject e in EnemiesOops)
        {
            GameObject bGO = Instantiate(FlashPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            bGO.GetComponent<RectTransform>().anchoredPosition = e.GetComponent<RectTransform>().anchoredPosition;
            Destroy(e);
        }
        foreach (GameObject r in Rows)
        {
            GameObject bGO = Instantiate(FlashPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
            bGO.GetComponent<RectTransform>().anchoredPosition = r.GetComponent<RectTransform>().anchoredPosition;
            Destroy(r);
        }
        Enemies.Clear();
        EnemiesMissed.Clear();
        EnemiesOops.Clear();
        Rows.Clear();
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
            speedTimer = maxSpeedTimer;
            rowSpeed = Mathf.Min(205f, rowSpeed + 15f);
            enemySpeed = Mathf.Min(205f, enemySpeed + 15f);
            rowTimerMax = Mathf.Max(.5f, rowTimerMax - .1f);
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
            GameObject row = Instantiate(RowPrefab, new Vector3(0, -100f, 0), Quaternion.identity, RowContainer.transform);
            RectTransform rt = row.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y);
            rt.transform.localPosition = new Vector3(rt.transform.localPosition.x, -200f, rt.transform.localPosition.z);
            row.GetComponent<Row>().SetArrow(newOrientation);
            row.GetComponent<Row>().Orientation = newOrientation;
            Rows.Add(row);
            CreateEnemy(newOrientation);
        }
    }

    void CreateEnemy(Globals.Orientations newOrientation)
    {
        GameObject enemy = Instantiate(EnemyPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
        RectTransform rt = enemy.GetComponent<RectTransform>();
        float newY = 210f;
        if (newOrientation == Globals.Orientations.Left || newOrientation == Globals.Orientations.Right)
            newY = 50f;
        float newX = 0f;
        if (newOrientation == Globals.Orientations.Left)
            newX = -150f;
        else if (newOrientation == Globals.Orientations.Right)
            newX = 150f;
        rt.anchoredPosition = new Vector2(newX, newY);
        enemy.GetComponent<Enemy>().ConfigureEnemy(newOrientation);
        Enemies.Add(enemy);
    }

    void CreateOopsEnemy(Globals.Orientations newOrientation)
    {
        GameObject enemy = Instantiate(EnemyPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
        RectTransform rt = enemy.GetComponent<RectTransform>();
        float newY = 210f;
        float newX = 165f;
        rt.anchoredPosition = new Vector2(newX, newY);
        enemy.GetComponent<Enemy>().ConfigureEnemy(newOrientation);
        enemy.GetComponent<Enemy>().SetType(Globals.EnemyTypes.Pumpkin);
        EnemiesOops.Add(enemy);
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
        if (life > 4)
            life = 4;
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
        MobileButtons.SetActive(true);
        MobileToggleButtonText.text = showMobileButtons ? "mobile buttons: ON" : "mobile buttons: OFF";
    }

}
