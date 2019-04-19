using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicGrid : MonoBehaviour {

    public int height;
    public int width;

    public float widthCell;
    public float heightCell;

    public Vector3 center;
    public Vector3 gridRotation = new Vector3(-26.169f, -43.278f, 22.552f);

    public GameObject cellPrefab;
    public GameObject auxCellPrefab;
    public GameController _mainController;
    public MeteorSpawner _spawner;

    Dictionary<Vector2, GridCell> cellHashMap;
    
    void Awake()
    {
		//Check that the object has everything it needs to create a Grid.
        if (cellPrefab == null)
        {
            Debug.LogError("Missing cell prefab");
        }
        if (widthCell == 0.0f || heightCell == 0.0f || height == 0 ||width == 0)
        {
            Debug.LogError("Sizing parameters are empty or zero");
        }
        if (_mainController == null)
        {
            Debug.LogError("Missing GameController reference on grid");
        }
        if (_spawner == null)
            Debug.LogError("Missing MeteorSpawner reference on grid");
        cellHashMap = new Dictionary<Vector2, GridCell>();
    }

    void Start()
    {
        StartGrid();
    }
	// Use this for initialization
	public void StartGrid () {
        
        // DYNAMIC INSTANTIATION
        for (int i = 0; i<width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject newCell = cellPrefab;
				// THIS "IF" is for debugging purposes
                if (auxCellPrefab != null && (i+j)%2 == 0)
                {
                    newCell = auxCellPrefab;
                }
				// Instantiation
                GameObject cell = Instantiate(newCell, new Vector3((i - width / 2) * widthCell + center.x,  center.y, (j - height / 2) * heightCell + center.z), Quaternion.identity);
                cell.gameObject.transform.parent = gameObject.transform;
                //Initialize Cell object, add it to hashmap and subscribe GameController to its events.
				GridCell cellEvents = cell.GetComponent<GridCell>();
                cellEvents.InitializeColor();
                cellEvents.setGridPosition(i, j);
                cellHashMap.Add(new Vector2((float)i, (float)j), cellEvents);
                _mainController.subscribeToEvents(cellEvents);
            }
        }

        gameObject.transform.Rotate(gridRotation);
        _mainController.setHashMap(cellHashMap); //Pass map to gameController
        //_mainController.gameObject.GetComponent<LevelManager>().PauseGame();
        _spawner.setGridInfo(cellHashMap, width, height);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
