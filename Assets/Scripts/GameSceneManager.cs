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
        "How did this start? You were dancing. Everything was normal. Then blistering light... followed by darkness... creatures appeared... dark, ghoulish creatures. They took your friends. Ripped at them, tore at them, devoured them.", 
        "And now it's just you. And you must DANCE. It's the only thing that keeps them away... that keeps YOU alive."
    };
    int introIndex = 0;

    [SerializeField]
    GameObject Level;
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
    GameObject EnemyPrefab;
    [SerializeField]
    GameObject EnemyContainer;
    [SerializeField]
    GameObject LifeBar;
    [SerializeField]
    TextMeshProUGUI GameTime;

    [SerializeField]
    GameObject BloodPrefab;
    [SerializeField]
    GameObject AttackPrefab;

    List<GameObject> Rows = new List<GameObject>();
    List<GameObject> Enemies = new List<GameObject>();
    List<GameObject> EnemiesMissed = new List<GameObject>();

    float rowTimer = 0;
    float rowTimerMax = 1f;
    float inGoodThreshold = -70f;
    float inGreatThreshold = -57f;
    float inPerfectThreshold = -44f;
    float destroyThreshold = -37f;
    int combo = 0;
    int life = 4;
    float lifebarMaxWidth = 43f;
    int maxLife = 4;
    float gameTime = 0;
    float speedTimer = 10f;
    float maxSpeedTimer = 10f;
    float rowSpeed = 100f;
    float enemySpeed = 60f;

    Coroutine RateCoroutine;
    Color goodColor = new Color(255f/255f, 216f/255f, 0/255f);
    Color badColor = new Color(255f/255f, 0, 110f/255f);

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
        PlayGame();
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
            NextButtonText.text = "PLAY";
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
            IntroText.GetComponent<TextMeshProUGUI>().text = "";
            NextButton.GetComponent<MoveNormal>().MoveDown();
            Globals.CurrentGameState = Globals.GameStates.Playing;
            Level.GetComponent<MoveNormal>().MoveUp();
            audioManager.StartMusic();
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
                    combo = 0;
                    HideCombo();
                }
                else if (Rows[0].GetComponent<Row>().CurrentScoreQuality == Globals.ScoreQualities.Great)
                {
                    if (RateCoroutine != null) StopCoroutine(RateCoroutine);
                    RateCoroutine = StartCoroutine(ShowRate("GREAT", goodColor));
                    combo = 0;
                    HideCombo();
                }
                else if (Rows[0].GetComponent<Row>().CurrentScoreQuality == Globals.ScoreQualities.Perfect)
                {
                    if (RateCoroutine != null) StopCoroutine(RateCoroutine);
                    RateCoroutine = StartCoroutine(ShowRate("PERFECT", goodColor));
                    combo++;
                    if (combo > 1)
                        ShowCombo();
                }
                StartCoroutine(ShowHighlight(Rows[0].GetComponent<Row>().Orientation, Color.yellow, .15f, .3f));
                AttackEnemy(Enemies[0].GetComponent<RectTransform>().anchoredPosition);
                Destroy(Enemies[0]);
                Enemies.RemoveAt(0);
            }
            else 
            {
                StartCoroutine(ShowHighlight(Rows[0].GetComponent<Row>().Orientation, badColor, .15f, .3f));
                if (RateCoroutine != null) StopCoroutine(RateCoroutine);
                RateCoroutine = StartCoroutine(ShowRate("OOPS", badColor));
                combo = 0;
                HideCombo();
                EnemiesMissed.Add(Enemies[0]);
                Enemies.RemoveAt(0);
            }
            Destroy(Rows[0]);
            Rows.RemoveAt(0);
        }
        else
        {
            if (RateCoroutine != null) StopCoroutine(RateCoroutine);
            RateCoroutine = StartCoroutine(ShowRate("OOPS", badColor));
            combo = 0;
            HideCombo();
        }
    }

    void AttackEnemy(Vector2 pos)
    {
        GameObject aGO = Instantiate(AttackPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
        aGO.GetComponent<RectTransform>().anchoredPosition = pos;
        GameObject bGO = Instantiate(BloodPrefab, new Vector3(0, 0, 0), Quaternion.identity, EnemyContainer.transform);
        bGO.GetComponent<RectTransform>().anchoredPosition = pos;
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
            if (RateCoroutine != null) StopCoroutine(RateCoroutine);
            RateCoroutine = StartCoroutine(ShowRate("MISS", badColor));
            combo = 0;
            HideCombo();

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
            Destroy(EnemiesMissed[0]);
            EnemiesMissed.RemoveAt(0);
            HitPlayer();
        }
    }

    void HitPlayer()
    {
        life--;
        float newLifebarWidth = lifebarMaxWidth * (float)life / (float)maxLife;
        LifeBar.GetComponent<RectTransform>().sizeDelta = new Vector2(newLifebarWidth, 7f);

        if (life == 0)
            EndGame();
    }

    void EndGame()
    {

    }

    void HandleTime()
    {
        gameTime += Time.deltaTime;
        GameTime.text = "<mspace=.6em>" + gameTime.ToString("0.0");
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
            newY = 60f;
        float newX = 0f;
        if (newOrientation == Globals.Orientations.Left)
            newX = -150f;
        else if (newOrientation == Globals.Orientations.Right)
            newX = 150f;
        rt.anchoredPosition = new Vector2(newX, newY);
        enemy.GetComponent<Enemy>().ConfigureEnemy(newOrientation);
        Enemies.Add(enemy);
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

}
