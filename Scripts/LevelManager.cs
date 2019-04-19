using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
// For comet hotfix
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {
    public LayerMask layer;
    ObjectHandler _objectHandler;
    GameController _controller;
    public MeteorSpawner _spawner;
    public PlayerController _playerController;
    ITutorialSection[] tutorialSections;
    int tutorialSectionIndex = 0;
    public GameObject tutorialText;

    public float luckDivider = 1.2f; // If divider is set to 1: timeLuck takes 20 seconds to get from 0 to 1.
    float luck;
    Vector2 lastStandingGridPosition = new Vector2(-1.0f, -1.0f);
    float elapsedTime;
    bool paused = false;
    float numberSecondsStill = 0.0f;
    float timeSinceReset = 0.0f;
    public float timeStandingPenalty = 3.0f;

    // Public score multipliers
    public float timeMultiplier = 10.0f;
    public float timeNumMovedObjects = 1.0f;
    public float timeNumUsedObjects = 20.0f;
    public float timeMeteorsSurvived = 1.0f;

    float score = 0.0f;
    int numObjects = 0;
    int numUsedObjects = 0;
    int numMovedObjects = 0;
    int meteorSurvived = 0;
    bool finished = false;

    PatternGenerator patternGen;

    public GameObject endMenu;
    public Text finalScore;
    int curretTutPanel;

    public void finishedRun()
    {
        if (_spawner.getTutorialEnded())
        {
            resetMeteorMap();
            Meteor[] meteors = FindObjectsOfType<Meteor>();
            foreach (Meteor met in meteors)
            {
                Destroy(met.gameObject);
            }
            generateFinalStats();
            Debug.Log("FINAL SCORE: " + getScore());
            _spawner.StopCoroutine("GenerateMeteors");
            _spawner.freezeMeteors();
            endMenu.SetActive(true);
        }
        else
        {
            tutorialSectionIndex--;
            nextTutorialSection();
        }
    }
    public void PauseGame()
    {
        paused = true;
        _spawner.StopCoroutine("GenerateMeteors");
        Utils.freezed = true;
        _spawner.freezeMeteors();
    }


    public bool isPaused()
    {

        return paused;
    }
    public void UnpauseGame()
    {
        paused = false;
        Utils.freezed = false;
        _spawner.unfreezeMeteors();
        _spawner.StartCoroutine("GenerateMeteors");
    }

    void setLuckParameter()
    {
        float timeLuck = 1.0f / (1.0f + Mathf.Exp((4 - timeSinceReset / 4) / luckDivider)); // If divider is set to 1: this variable takes 20 seconds to get from 0 to 1.
        if (timeStandingPenalty < numberSecondsStill)
        {
            luck = 1.0f;
        }
        else
        {
            luck = timeLuck;
        }
        _spawner.setLuck(luck, lastStandingGridPosition);
        if (timeLuck >= 0.95f)
        {
            timeSinceReset = 0.0f;
            luck = 0.0f;
            MeteorPattern[] patternList = patternGen.getRandomPattern();
            _spawner.setPattern(patternList);
        }
    }

    void CheckPlayerPosition()
    {
        // Get last known position
        float standingXGridPosition = _playerController.gameObject.GetComponent<GridObject>().xPosition;
        float standingYGridPosition = _playerController.gameObject.GetComponent<GridObject>().yPosition;
        if (lastStandingGridPosition.x == standingXGridPosition && lastStandingGridPosition.y == standingYGridPosition)
        {
            numberSecondsStill += Time.deltaTime;
        }
        else
        {
            numberSecondsStill = 0.0f;
        }
        setLuckParameter();
        lastStandingGridPosition.x = standingXGridPosition;
        lastStandingGridPosition.y = standingYGridPosition;
    }

    void IncreaseTimer()
    {
        elapsedTime += Time.deltaTime;
        timeSinceReset += Time.deltaTime;
    }

    public void resetMeteorMap()
    {
        _spawner.resetMeteorMap();
    }

    // EVENTS

    void onMeteorHit(Vector2 pos)
    {
        meteorSurvived++;
    }

    public int getMeteorsHit()
    {
        return meteorSurvived;
    }

    public void decreaseMeteorsHit(int amount)
    {
        meteorSurvived -= amount;
    }

    void onObjectDestroyed()
    {
        numObjects--;
    }

    void onObjectGenerated()
    {
        numObjects++;
    }

    void onObjectUsed()
    {
        numUsedObjects++;
    }

    void onObjectMoved()
    {
        numMovedObjects++;
    }

    public int getNumObjects()
    {
        return numObjects;
    }
    void increaseScore()
    {
        score = score + Time.deltaTime * timeMultiplier * numObjects;
    }

    public void generateFinalStats()
    {
        finished = true;
        Debug.Log("Elapsed Time: " + elapsedTime);
        Debug.Log("Base Score: " + score);
        score = score + numMovedObjects * timeNumMovedObjects + numUsedObjects * timeNumUsedObjects + meteorSurvived * timeMeteorsSurvived;
        finalScore.text = getScore().ToString();
        Debug.Log("numMovedObjects: " + numMovedObjects);
        Debug.Log("numUsedObjects: " + numUsedObjects);
        Debug.Log("meteorSurvived: " + meteorSurvived);
    }

    void checkAchievements()
    {
        
    }

    void subscribeEvents()
    {
        _objectHandler.OnObjectAdded += onObjectGenerated;
        _objectHandler.OnObjectDestroyed += onObjectDestroyed;
        _playerController.OnObjectUsed += onObjectUsed;
        _playerController.OnObjectMoved += onObjectMoved;
        _controller.OnMeteorHit += onMeteorHit;
    }

    public float getScore()
    {
        return score;
    }

    void Awake()
    {
        tutorialSections = GetComponents<ITutorialSection>();
        patternGen = new PatternGenerator();
        _objectHandler = GetComponent<ObjectHandler>();
        _controller = GetComponent<GameController>();
        if (_objectHandler == null)
        {
            Debug.LogError("No handler assigned to Game Controller");
        }
        if (_controller == null)
        {
            Debug.LogError("No controller assigned to Game Controller");
        }
        subscribeEvents(); 

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        //IF TUTORIAL ENABLED
        if (PlayerPrefs.GetInt("playedOnce") == 0 || PlayerPrefs.GetInt("forceTut") != 0)
        {
            SaveTutValues(0, PlayerPrefs.GetInt("playedOnce"));
            nextTutorialSection();
        }
        else
        {
            _spawner.setTutorialEnded(true);
            tutorialText.SetActive(false);
            paused = false;
        }
    }

    void SaveTutValues(int forceTut, int playedOnce)
    {
        PlayerPrefs.SetInt("forceTut", forceTut);
        PlayerPrefs.SetInt("playedOnce", playedOnce);
    }

    public void nextTutorialSection()
    {
        if (tutorialSectionIndex  < tutorialSections.Length)
        {
            tutorialSections[tutorialSectionIndex].enableSection();
            tutorialSectionIndex++;
        }
        else
        {
            SaveTutValues(0, 1);

            GridObject[] objects = FindObjectsOfType<GridObject>();
            tutorialText.SetActive(false);
            foreach (GridObject element in objects)
            {
                if (element.gameObject.GetComponent<BasicObject>() != null)
                {
                    element.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    element.gameObject.GetComponent<BasicObject>().setDestroyed(false);
                    element.setParentCell(null);                    
                }
            }

            _spawner.setTutorialEnded(true);
            paused = false;
            gameObject.GetComponent<GameController>().setInitialObjects(false);
        }
    }

	// Use this for initialization
	void Start () {
        elapsedTime = 0.0f;
        curretTutPanel = 0;
    }
	
	// Update is called once per frame
	void Update () {
		if (!paused && !finished && _spawner.getTutorialEnded())
        {
            CheckPlayerPosition();
            IncreaseTimer();
            increaseScore();
        }
	}


    public void SaveScore()
    {
        GraphicRaycaster r = FindObjectOfType<GraphicRaycaster>();
        r.enabled = false;
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("login success");
                Debug.Log(Social.localUser.id);
                Debug.Log(Social.localUser.userName);
                Social.ReportScore((long)score, "CgkIuvj6vc0IEAIQAw", (bool result) => {
                });
                Social.ShowLeaderboardUI();
            }
            else
            {
                Debug.Log("login fail");
            }

        });
        r.enabled = true;
    }

}
