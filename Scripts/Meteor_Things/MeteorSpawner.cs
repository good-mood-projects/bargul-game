using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour {

    public GameObject meteorPrefab;
    public GameController _gameController; //Pass to meteors

    public float spawnRate = 10.0f;

    public float StartMin = 5.0f;
    public float StartMax = 6.0f;

    public float FallMin = 3.0f;
    public float FallMax = 5.0f;

    public float warningTime = 1.5f;

    MeteorShapeGenerator generator;

    Dictionary<Vector2, GridCell> hashMap;
    int gridWidth;
    int gridHeight;
    // Use this for initialization
    bool spawning = false;
    bool patternMode = false;
    bool tutorialEnded = false;

    bool wasSpawning = true;
    bool wasPatternMode = false;

    float patternInitTime = 0.0f;
    float patternEndTime = 0.0f;
    MeteorPattern[] currentPatternList;
    int patternIndex = -1;

    float currentLuck = 0.0f;
    Vector2 centralPoint = new Vector2(0.0f, 0.0f);

    float timeDifficulty = 1.0f;

    public float timeDifficultyIncrease = 0.2f;
    public float holePercentageIncrease = 0.05f;

    Dictionary<Vector2, List<Meteor>> meteorMap;
    void Awake()
    {
        generator = new MeteorShapeGenerator();
        if (_gameController == null)
        {
            Debug.LogError("No GameController referenced on MeteorSpawner");
        }
        meteorMap = new Dictionary<Vector2,List<Meteor>>();
        _gameController.OnMeteorHit += MeteorHit;
    }

    void InitializeMeteorMap()
    {
        for (int i = 0; i<gridWidth; i++)
        {
            for (int j=0; j<gridHeight; j++) {
                Vector2 pos = new Vector2((float)i,(float)j);
                meteorMap.Add(pos, new List<Meteor>());
                //Debug.Log("Added " + pos);
            }
        }
    }

    public void resetMeteorMap()
    {
        meteorMap = new Dictionary<Vector2, List<Meteor>>();
        InitializeMeteorMap();
    }

    public void freezeMeteors()
    {
        foreach (var value in meteorMap.Values)
        {
            foreach (Meteor meteor in value)
            {
                meteor.freeze();
            }
        }
        Utils.freezed = true;
        wasSpawning = spawning;
        spawning = false;
        wasPatternMode = patternMode;
        patternMode = false;

    }

    public void setTutorialEnded(bool value)
    {
        tutorialEnded = value;
    }

    public bool getTutorialEnded()
    {
        return tutorialEnded;
    }

    public void unfreezeMeteors()
    {
        foreach (var value in meteorMap.Values)
        {
            foreach (Meteor meteor in value)
            {
                meteor.unfreeze();
            }
        }
        Utils.freezed = false;
        spawning = wasSpawning;
        patternMode = wasPatternMode;
    }

    //When a meteor hits, remove the meteor from the position
    void MeteorHit(Vector2 position)
    {
        Meteor destroyedMeteor = meteorMap[position][0];
        meteorMap[position].RemoveAt(0);
        //Debug.Log("Removed meteor");
    }

	void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setPattern(MeteorPattern[] newPatternList)// MeteorPattern)
    {
        if (!patternMode)
        {
            patternIndex = -1;
            Debug.Log("PATTERN");
            currentPatternList = newPatternList;
        }
        else
        {
            Array.Resize(ref currentPatternList, currentPatternList.Length + newPatternList.Length);
            for (int i=0;i < newPatternList.Length; i++)
            {
                currentPatternList[i + currentPatternList.Length - newPatternList.Length] = newPatternList[i];
            }
        }
        //loadPattern();
        patternMode = true;
        spawning = false;
        if (tutorialEnded)
        {
            timeDifficulty += timeDifficultyIncrease;
            _gameController.increaseHolePercentage(holePercentageIncrease);
        }
    }

    void loadPattern()
    {
        //DEBUG
        int totalMet = 0;
       foreach (KeyValuePair<Vector2, List<Meteor>> entry in meteorMap)
        {
            totalMet += entry.Value.Count;
        }
        Debug.Log("Number of meteors before pattern: " + totalMet);
//DEBUG
        patternIndex++;
        MeteorPattern patternToLoad = currentPatternList[patternIndex];
        Vector2[] positions = patternToLoad.initialPos.ToArray();
        float[] fallTime = patternToLoad.timeToFall.ToArray();
        Shape[] shapes = patternToLoad.meteorShape.ToArray();
        float[] startTime = patternToLoad.timeToStart.ToArray();
        int size = positions.Length;
        Debug.Log("Adding " + size + " meteors as pattern");
        patternEndTime = patternToLoad.initTime;
        patternInitTime = patternToLoad.endTime;
        for (int i=0; i<size; i++)
        {
            GameObject newMeteor = Instantiate(meteorPrefab, new Vector3(100.0f, 100.0f, 100.0f), Quaternion.identity);//Instance
            Meteor meteorRef = newMeteor.GetComponent<Meteor>();
            if (meteorMap[positions[i]] != null)
            {
                meteorMap[positions[i]].Add(meteorRef);
            }
            else
            {
                Debug.Log("Emptied meteormap on position:"+ positions[i]);
                meteorMap.Add(positions[i], new List<Meteor>());
                meteorMap[positions[i]].Add(meteorRef);
            }
            //Debug.Log("Ok "+positionToHit);
            //registerHitShapeOnCells(hashMap[positionToHit], newShape);
            meteorRef.Initialize(hashMap[positions[i]], fallTime[i], startTime[i], patternToLoad.warningTime, _gameController, shapes[i]);
            if (!tutorialEnded)
            {
                meteorRef.setMakeHoles(true);
            }
        }
        //DEBUG
        totalMet = 0;
        foreach (KeyValuePair<Vector2, List<Meteor>> entry in meteorMap)
        {
            totalMet += entry.Value.Count;
        }
        Debug.Log("Number of meteors after pattern: " + totalMet);
        //DEBUG
    }

    public void setGridInfo(Dictionary<Vector2, GridCell> cellMap, int width, int height)
    {
        //Debug.Log("Setting grid info");
        hashMap = cellMap;
        gridWidth = width;
        gridHeight = height;
        InitializeMeteorMap();
        // spawning = true;
        StartCoroutine(GenerateMeteors());
        // START COROUTINE
    }

    IEnumerator GenerateMeteors()
    {
        while (true)
        {
            yield return null;
            while (spawning && !Utils.freezed) //Safely stop this coroutine
            {
                float xRelativePositionHit = Mathf.Floor(UnityEngine.Random.Range(-1.0f * (float)gridWidth * (1.0f - currentLuck), (float)gridWidth) * (1.0f - currentLuck));
                float yRelativePositionHit = Mathf.Floor(UnityEngine.Random.Range(-1.0f * (float)gridHeight * (1.0f - currentLuck), (float)gridHeight) * (1.0f - currentLuck));
                float xPositionToHit = centralPoint.x + xRelativePositionHit;
                float yPositionToHit = centralPoint.y + yRelativePositionHit;
                if (xPositionToHit < 0.0f)
                {
                    xPositionToHit = 0.0f;
                }
                else if (xPositionToHit > gridWidth - 1)
                {
                    xPositionToHit = gridWidth - 1;
                }

                if (yPositionToHit < 0.0f)
                {
                    yPositionToHit = 0.0f;
                }
                else if (yPositionToHit > gridHeight - 1)
                {
                    yPositionToHit = gridHeight - 1;
                }

                Vector2 positionToHit = new Vector2(xPositionToHit, yPositionToHit);
                Shape newShape = generator.generateSquareShape(UnityEngine.Random.Range(1, 4)); // TO DO: Change
                float timeToFall = UnityEngine.Random.Range(FallMin, FallMax);
                float timeToStart = UnityEngine.Random.Range(StartMin, StartMax);
                if (spawning) { 
                    GameObject newMeteor = Instantiate(meteorPrefab, new Vector3(100.0f, 100.0f, 100.0f), Quaternion.identity);//Instance
                    Meteor meteorRef = newMeteor.GetComponent<Meteor>();
                    meteorMap[positionToHit].Add(meteorRef);                    
                    //Debug.Log("Ok "+positionToHit);
                    //registerHitShapeOnCells(hashMap[positionToHit], newShape);
                    meteorRef.Initialize(hashMap[positionToHit], timeToFall, timeToStart, warningTime, _gameController, newShape);
                    meteorRef.setMakeHoles(true);
                    //Debug.Log("Spawned Meteor to hit " + positionToHit + " starting in " + timeToStart + " and hitting in " + timeToFall);
                }
                else
                {
                    Debug.Log("Added from random routine");
                }
                yield return new WaitForSeconds(spawnRate / timeDifficulty); //TO DO: Change
            }
            while (patternMode && !Utils.freezed)
            {
                yield return new WaitForSeconds(patternInitTime);
                if (patternMode)
                    loadPattern();
                    if (patternIndex == currentPatternList.Length - 1)
                    {
                        patternMode = false;
                        if (tutorialEnded)
                        {
                            spawning = true;
                        }
                        Debug.Log("SPAWNING");
                    }
                else
                    break;
                yield return new WaitForSeconds(patternEndTime);
            } 
            while (!patternMode && !spawning || Utils.freezed)
            {      
                if (tutorialEnded)
                {
                    spawning = true;
                }        
                yield return new WaitForSeconds(spawnRate / timeDifficulty); ;
            }
        }
    }

    public void registerHitShapeOnCells(GridCell cell, Shape shape)
    {
        Vector2 mainPos = cell.getGridPosition();
        foreach (Vector2 position in shape.GetHitShape())
        {
            GridCell selectedCell;
            hashMap.TryGetValue(mainPos + position, out selectedCell);
            if (selectedCell != null)
            {
                if (position == Vector2.zero)
                    selectedCell.IncreaseHitCounter();
                else
                    selectedCell.IncreaseShapeCounter();
            }
        }
    }

    public List<Meteor> getMeteorList()
    {
        List<Meteor> meteorList = new List<Meteor>();
        foreach (KeyValuePair<Vector2,List<Meteor>> meteorL in meteorMap)
        {
            List<Meteor> copyQueue = meteorL.Value;
            if (copyQueue.Count > 0)
            {
                meteorList.Add(copyQueue[0]);
            }
        }
        return meteorList;
    }

    public Dictionary<Vector2, GridCell> getGridMap()
    {
        return hashMap;
    }

    public void setLuck(float luck, Vector2 newCentralPoint)
    {
        currentLuck = luck;
        centralPoint = newCentralPoint;
    }

}
