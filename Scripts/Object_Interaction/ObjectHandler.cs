using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandler : MonoBehaviour {

    public GameObject[] initialobjects;
    List<GameObject> objectList;
    GameController _controller;
    public MeteorSpawner _spawner;
    public PlayerController _playerController;
    public RespawnUsesDisplayManager displayRespawn;

    public delegate void ObjectAdded();
    public event ObjectAdded OnObjectAdded;

    public delegate void ObjectDestroyed();
    public event ObjectDestroyed OnObjectDestroyed;

    public int uses = 5;

    public int getRemainingUses()
    {
        return uses;
    }

    void setInitialObjects()
    {
        objectList = new List<GameObject>();
        objectList.AddRange(initialobjects);
        foreach (GameObject obj in objectList)
            OnObjectAdded();

        if (displayRespawn == null) Debug.LogError("Respawn HUD manager not set in the object handler");
        else displayRespawn.SetAmountOfUses(uses);
    }

    void AddObject(GameObject newObject)
    {
        //Instantiate object
        objectList.Add(newObject);
        OnObjectAdded();
    }

    public void recoverObject(int i)
    {
        /*if (uses > 0 && !objectList.Contains(initialobjects[i]))
        {
            foreach (KeyValuePair<Vector2, GridCell> entry in _controller.getHashMap())
            {
                if (!entry.Value.isBeingHit() && entry.Value.getObjectInside() == null)
                {
                    initialobjects[i].GetComponent<GridObject>().xPosition = (int)entry.Key.x;
                    initialobjects[i].GetComponent<GridObject>().yPosition = (int)entry.Key.y;
                    GameObject g0 = Instantiate(initialobjects[i]);
                    Vector2 initialPos = g0.GetComponent<GridObject>().getPosition();
                    Vector2 initialScreenPos = entry.Value.getScreenPosition();
                    g0.transform.position = Camera.main.ScreenToWorldPoint(
                        new Vector3(initialScreenPos.x, initialScreenPos.y, Camera.main.nearClipPlane + g0.GetComponent<GridObject>().getPlane()));
                    entry.Value.setObjectInside(initialobjects[i]);
                    uses--;
                    break;
                }
            }
        }*/
        if(uses > 0 && !objectList.Contains(initialobjects[i]) && _controller.getHashMap()[initialobjects[i].GetComponent<GridObject>().getPosition()].getObjectInside() == null)
        {
            Vector2 destination = _controller.getHashMap()[initialobjects[i].GetComponent<GridObject>().getPosition()].getScreenPosition();
            Vector3 positionWorld = Camera.main.ScreenToWorldPoint(new Vector3(destination.x, destination.y, Camera.main.nearClipPlane + initialobjects[i].GetComponent<GridObject>().getPosition().x + initialobjects[i].GetComponent<GridObject>().getPosition().y + 1.0f));
            if (!_playerController.isPlayerMovingObjectTo(positionWorld))
            {
                //initialobjects[i].SetActive(true);
                initialobjects[i].GetComponent<BasicObject>().setDestroyed(false);
                objectList.Add(initialobjects[i]);
                if (_spawner.getTutorialEnded())
                {
                    uses--;
                    displayRespawn.Used();
                }
                displayRespawn.GetComponent<RespawnUsesSpriteManager>().SwapSwitch(i);
                _controller.getHashMap()[initialobjects[i].GetComponent<GridObject>().getPosition()].setObjectInside(initialobjects[i]);
                OnObjectAdded();
                initialobjects[i].GetComponent<BasicObject>().Use();
            }
        }
        /*else
        {
         // PLAY ERROR SOUND
        }
        */

    }

    public bool checkIfObjectInPosition(Vector2 pos)
    {
        bool res = false;
        foreach (GameObject g0 in initialobjects) {
            if (g0.GetComponent<GridObject>().getPosition() == pos)
            {
                res = true;
            }
        }
        return res;
    }

    public bool checkAndRemove(GameObject objectToRemove)
    {
        if (objectToRemove != null)
        {
            objectList.Remove(objectToRemove);
            displayRespawn.GetComponent<RespawnUsesSpriteManager>().SwapByObject(objectToRemove);
            OnObjectDestroyed();
            if (objectList.Count == 0)
                _controller.finishRun();
            else
            {
                return true;
            }
        }
        return false;
    }
	// Use this for initialization
	void Awake () {
        _controller = GetComponent<GameController>();
		if (_controller == null)
        {
            Debug.LogError("No controller asigned to ObjectHandler");
        }
        if (initialobjects.Length == 0)
        {
            Debug.LogError("No objects asigned to ObjectHandler");
        }
	}
	void Start()
    {
        _controller.registerObjectHandler(this);
        setInitialObjects();
    }
	// Update is called once per frame
	void Update () {
		
	}
}
