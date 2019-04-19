using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
* This script should be attached to an empy object
* It contains the logic regarding UI menus for moving*
* picking and dropping objects and also stores the 
* result of instancing the grid.
*/
public class GameController : MonoBehaviour {

    public LayerMask layer;
    public GameObject player;
    public ScreenShake cameraShaker;
	
	//This maps each cell to their grid position.
    Dictionary<Vector2, GridCell> cellHashMap;

    /* Not used right now
    //Audio components
    private TrackController trackController; // Script wich controlls the tracks to play sounds.
    private SceneSoundLibrary soundLibrary; // Script wich contains all the used sounds for the scene. 
    */

    //Variable indicating if an UI menu is open (movement)
    bool selectingMove = false;
	
	//Variable indicating the last selectedCell (could be null)
    Vector2 lastSelectedCellPosition;

	//If a movement is confirmed, this contains lastSelectedCellPosition 
	// If not, a decision is being made by the user
    Vector2 destination;
	
    //List of possible Player actions
	enum PlayerAction { Move, Use, MoveTakeDrop};

    //Envent to emit when a cell is hit by a Meteor
    public delegate void MeteorHit(Vector2 position);
    public event MeteorHit OnMeteorHit;

    public float holePercentageChance;


    public void setHolePercentage(float value)
    {
        holePercentageChance = value;
    }

    public void increaseHolePercentage (float increaseAmount)
    {
        holePercentageChance += increaseAmount;
    }
    /* Audio components not used right now
    // Background audio calls
    void Start()
    {
        trackController = GameObject.FindObjectOfType<TrackController>().GetComponent<TrackController>();
        soundLibrary = GameObject.FindObjectOfType<SceneSoundLibrary>().GetComponent<SceneSoundLibrary>();

        if (trackController == null) Debug.LogError("TrackController never got instanciated (Scene persistent object)");
        if (soundLibrary == null) Debug.LogError("SceneSoundLibrary not found in the scene");

        if (trackController != null && soundLibrary != null) trackController.PlaySoundClip(0, soundLibrary.SelectSound("background", 0));
    }
    */

    public GameObject arrowMove;

    ObjectHandler objectHandler;

    public void registerObjectHandler(ObjectHandler newHandler)
    {
        objectHandler = newHandler;
    }
    // UI INTERACTIONS!

    public void reStartButton()
    {
        Utils.freezed = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Allways recalls the active scene
    }


    void Update()
    {
        if (!Utils.freezed)
        {
            RaycastHit hit;
            Vector2 standingGridPosition = new Vector2(-1.0f, -1.0f);
            //Get current position
            if (Physics.Raycast(player.GetComponent<Renderer>().bounds.center - new Vector3(0.0f, player.GetComponent<Renderer>().bounds.extents.y, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), out hit, 10000.0f, layer))
            {
                if (hit.transform.gameObject.GetComponent<GridCell>() != null)
                {
                    standingGridPosition = hit.transform.gameObject.GetComponent<GridCell>().getGridPosition();
                    if (Physics.Raycast(player.GetComponent<Renderer>().bounds.center - new Vector3(0.0f, player.GetComponent<Renderer>().bounds.extents.y+0.2f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), out hit, 10000.0f, layer))
                    {
                       if (hit.transform.gameObject.GetComponent<GridCell>() != null)
                       {
                            if (hit.transform.gameObject.GetComponent<GridCell>().getGridPosition() == standingGridPosition)
                            {
                                if (Physics.Raycast(player.GetComponent<Renderer>().bounds.center - new Vector3(0.0f, player.GetComponent<Renderer>().bounds.extents.y - 0.2f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), out hit, 10000.0f, layer))
                                {
                                    if (hit.transform.gameObject.GetComponent<GridCell>() != null)
                                    {


                                        if (hit.transform.gameObject.GetComponent<GridCell>().getGridPosition() == standingGridPosition && cellHashMap[standingGridPosition].getDisabledStatus())
                                        {
                                            finishRun();
                                        }
                                    }
                                }
                            }
                       }
                    }
                }
            }
        }
    }

    // All purpose Actions sent to the player (translates positions to WorldPoitns and calls to functions inside PlayerController)
    void SendPlayerAction(PlayerAction action, Vector2 position, Vector2 finalPosition)
    {
        destination = cellHashMap[position].getScreenPosition();
        PlayerController playerController = player.GetComponent<PlayerController>();
        Vector3 positionWorld = Camera.main.ScreenToWorldPoint(new Vector3(destination.x, destination.y, Camera.main.nearClipPlane + position.x+position.y + 1.0f));
        playerController.setEndRenderPlane(positionWorld.z);
        if (action == PlayerAction.Move)
        {
            playerController.MoveTo(new Vector2(positionWorld.x, positionWorld.y));
            playerController.gameObject.GetComponent<GridObject>().xPosition = (int)position.x;
            playerController.gameObject.GetComponent<GridObject>().yPosition = (int)position.y;
        }
        else if (action == PlayerAction.Use)
        {
            playerController.MoveAndUse(new Vector2(positionWorld.x, positionWorld.y), cellHashMap[position].getObjectInside());
            playerController.gameObject.GetComponent<GridObject>().xPosition = (int)position.x;
            playerController.gameObject.GetComponent<GridObject>().yPosition = (int)position.y;
        }
        else if (action == PlayerAction.MoveTakeDrop)
        {
            Vector2 finalDestination = cellHashMap[finalPosition].getScreenPosition();
            Vector3 finalPositionWorld = Camera.main.ScreenToWorldPoint(new Vector3(finalDestination.x, finalDestination.y, Camera.main.nearClipPlane + finalPosition.x + finalPosition.y + 1.0f));
            playerController.MoveTakeDrop(new Vector2(positionWorld.x, positionWorld.y), cellHashMap[position].getObjectInside(), new Vector2(finalPositionWorld.x, finalPositionWorld.y), cellHashMap[finalPosition]);
            playerController.gameObject.GetComponent<GridObject>().xPosition = (int)finalPosition.x;
            playerController.gameObject.GetComponent<GridObject>().yPosition = (int)finalPosition.y;

        }

    }

    void MouseOverEffect(Vector2 position)
    {
        if (selectingMove)
        {
            if (cellHashMap != null)
            {
                foreach (GridCell cell in cellHashMap.Values)
                {
                    if (cell.getGridPosition() == position)
                    {
                        //cell.setColor(Color.cyan);
                        Vector2 originPos = cellHashMap[lastSelectedCellPosition].getScreenPosition();
                        float arrowAngle = Vector2.Angle(Vector2.up, cell.getScreenPosition() - originPos) *
                            Mathf.Sign(Vector2.up.x * (cell.getScreenPosition() - originPos).y - Vector2.up.y * (cell.getScreenPosition() - originPos).x);
                        float size = Vector3.Distance(Camera.main.ScreenToWorldPoint(originPos), Camera.main.ScreenToWorldPoint(cell.getScreenPosition()));
                        arrowMove.gameObject.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(originPos).x, Camera.main.ScreenToWorldPoint(originPos).y, arrowMove.gameObject.transform.position.z) ;
                        arrowMove.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f + arrowAngle);
                        arrowMove.GetComponent<SpriteRenderer>().size = new Vector2(arrowMove.GetComponent<SpriteRenderer>().size.x, size*5);
                        arrowMove.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }
        }
    }

    public void ResetMouseOverEffect()
    {
        arrowMove.GetComponent<SpriteRenderer>().enabled = false;
    }

    void CellSelected(Vector2 position)
    {
        if (cellHashMap != null && !player.GetComponent<PlayerController>().getMoving())
        {
            if (!selectingMove) //Not holding mouse 
            {
                ResetMouseOverEffect();
                lastSelectedCellPosition = position;
                if (cellHashMap[position].getObjectInside() != null && !cellHashMap[position].getObjectInside().gameObject.GetComponent<BasicObject>().isDestroyed()) //Object on cell
                {
                    selectingMove = true;
                }
                else
                {
                    SendPlayerAction(PlayerAction.Move, position, Vector2.zero);
                }
            }
            else //Relesease mouse
            {
                ResetMouseOverEffect();
                selectingMove = false;
                if (lastSelectedCellPosition == position)
                {
                    if (cellHashMap[position].getObjectInside() != null &&  !cellHashMap[position].getObjectInside().gameObject.GetComponent<BasicObject>().isDestroyed()) //Object on cell
                    {
                        SendPlayerAction(PlayerAction.Use, position, Vector2.zero);                        
                    }
                    else
                    {
                        SendPlayerAction(PlayerAction.Move, position, Vector2.zero);
                    }
                }
                else
                {   
                    if (cellHashMap[position].getObjectInside() == null && cellHashMap[lastSelectedCellPosition].getObjectInside() != null && !cellHashMap[lastSelectedCellPosition].getObjectInside().gameObject.GetComponent<BasicObject>().isDestroyed()) //NO Object on cell
                    {
                        SendPlayerAction(PlayerAction.MoveTakeDrop, lastSelectedCellPosition, position);
                    }
                    else
                    {
                        arrowMove.GetComponent<SpriteRenderer>().enabled = false;
                    }
                    //else, no action allowed
                    lastSelectedCellPosition = position;
                }
            }
        }
    }

    //Called after setting up the hashmap, finds all objects with a GridObject component
    // and places them on the grid, while asigning them to a cell.
    public void setInitialObjects(bool setPlayer)
    {
        //Player
        foreach (GameObject g0 in FindObjectsOfType<GameObject>())
        {
            if (g0.GetComponent<GridObject>() != null)
            {
                if ((setPlayer && g0.GetComponent<PlayerController>() != null) || g0.GetComponent<PlayerController>() == null)
                {
                    Vector2 initialPos = g0.GetComponent<GridObject>().getInitialPosition();
                    Vector2 initialScreenPos = cellHashMap[initialPos].getScreenPosition();
                    g0.transform.position = Camera.main.ScreenToWorldPoint(
                        new Vector3(initialScreenPos.x, initialScreenPos.y, Camera.main.nearClipPlane + g0.GetComponent<GridObject>().getPlane()));
                    if (g0.GetComponent<PlayerController>() == null)
                    {
                        if (g0.GetComponent<GridObject>().getParentCell() != null)
                        {
                            g0.GetComponent<GridObject>().getParentCell().detatchObjectFromCell();
                        }
                        cellHashMap[initialPos].setObjectInside(g0);
                    }
                }
            }
        }
    }


    public Vector2 getScreenPositionFromGrid(Vector2 gridPosition)
    {
        return cellHashMap[gridPosition].getScreenPosition();
    }

    //Called by a DynamicGrid to subscribe the GameController to each cell raycast hit events
    public void subscribeToEvents(GridCell cell)
    {
        cell.OnSelected += CellSelected;
        cell.OnMouseOverGrid += MouseOverEffect;
    }

    //Called by a DynamidGrid to pass the current grid hashmap
    public void setHashMap(Dictionary<Vector2, GridCell> map)
    {
        cellHashMap = map;
        setInitialObjects(true);
    }

    public Dictionary<Vector2, GridCell> getHashMap()
    {
        return cellHashMap;
    }


    public void resetGridColor()
    {
        foreach (KeyValuePair<Vector2, GridCell> entry in cellHashMap)
        {
            entry.Value.resetStatus();
            // do something with entry.Value or entry.Key
        }
    }

    public void MeteorCollision(Meteor meteor)
    {
        Vector2 position = meteor.getHitPosition();

        RaycastHit hit;
        Vector2 standingGridPosition = new Vector2(-1.0f,-1.0f);
        //Get current position
        if (Physics.Raycast(player.GetComponent<Renderer>().bounds.center - new Vector3(0.0f, player.GetComponent<Renderer>().bounds.extents.y/2,0.0f), new Vector3(0.0f,0.0f,1.0f),out hit,10000.0f,layer))
        {
            if (hit.transform.gameObject.GetComponent<GridCell>() != null)
            {
                standingGridPosition = hit.transform.gameObject.GetComponent<GridCell>().getGridPosition();
            }
        }

        GridCell hitTargetGridCell;
        cellHashMap.TryGetValue(position, out hitTargetGridCell);
        if (hitTargetGridCell != null)
        {
            hitTargetGridCell.removeHitColor();
            hitTargetGridCell.InitExplosionPs();
        }

        foreach (Vector2 cell in meteor.getHitShape().GetHitShape())
        {
            GridCell targetGridCell;
            Vector2 targetPosition;
            targetPosition = position + cell;
            cellHashMap.TryGetValue(targetPosition, out targetGridCell);
            if(targetGridCell != null)
            {
                if (targetGridCell.getGridPosition() == standingGridPosition)
                {
                    finishRun();
                }
                if (cell != Vector2.zero)
                {
                    targetGridCell.removeShapeColor();
                }
                else
                {
                    if (meteor.doesMakeHoles())
                    {
                        if (Random.Range(0.0f, 1.0f) < holePercentageChance && !objectHandler.checkIfObjectInPosition(targetGridCell.getGridPosition()))
                        {
                            targetGridCell.hitDisable();
                        }
                    }
                }
                targetGridCell.DestroyObjectInside();

                bool destroyed = objectHandler.checkAndRemove(targetGridCell.getObjectInside());
                targetGridCell.detatchObjectFromCell();
                if (destroyed && targetGridCell.getGridPosition() == lastSelectedCellPosition && selectingMove)
                {
                    ResetMouseOverEffect();
                }
                /*if (result)
                {
                    if(lastSelectedCellPosition == targetPosition)
                    {
                        movingMenu.SetActive(false);
                        droppingMenu.SetActive(false);
                        selectingMove = false;
                    }
                }*/
                //Debug.Log("Hit on position " + targetPosition);

            }
            //else
                //Debug.Log("Accesing a grid out of the dictionary " + targetPosition);
        }
        cameraShaker.Shake(0.5f);
        OnMeteorHit(position);
        meteor.Die();
    }

    public void finishRun()
    {
        LevelManager manager = GetComponent<LevelManager>();
        manager.finishedRun();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}