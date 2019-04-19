using DragonBones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* This object should be attached to 
* a player object.
*/
public class PlayerController : MonoBehaviour {

    //Player Sprite controllers
    public Sprite idle;
    public Sprite carryingObj;

    private TrackController trackController; // Script wich controlls the tracks to play sounds.
    private SceneSoundLibrary soundLibrary; // Script wich contains all the used sounds for the scene. 

    SpriteRenderer render;

	//Movement speed for the player
    public float speed = 5.0f;

	//Vector indicating the direction of movement (could be null)
    Vector2 movementDirection;
   
	//Indicates if the player is currently moving
	bool moving = false;
    
	//Destination of the player (can be initially null)
	Vector2 objective;
    Vector2 finalObjective;
    float objectiveRenderPlane;

	//Placeholder of an object
    public GameObject objectHolder;
    
	//State variables complementary to the boolean "moving"
	bool takingObject = false;
    bool droppingObject = false;
    bool usingObject = false;
	
	//Destination cell when holding
    GridCell droppingCell;
	
	//Object to be held by the player (not final)
    GameObject targetObject;

	//Object held by this player
    GameObject currentObject;

    public ArrowMovPointer pointer;

    //Events
    public delegate void ObjectUsed();
    public event ObjectUsed OnObjectUsed;
    public delegate void ObjectMoved();
    public event ObjectMoved OnObjectMoved;

    public GameObject arrowMove;

    UnityArmatureComponent _armature;

    void Awake()
    {
        _armature = GetComponent<UnityArmatureComponent>();
        /*render = GetComponent<SpriteRenderer>();
        if(render == null)
        {
            Debug.LogError("No sprite renderer in player character.");
        }*/
    }

    void Start()
    {
        trackController = GameObject.FindObjectOfType<TrackController>().GetComponent<TrackController>();
        soundLibrary = GameObject.FindObjectOfType<SceneSoundLibrary>().GetComponent<SceneSoundLibrary>();

        if (trackController == null) Debug.LogError("TrackController never got instanciated (Scene persistent object)");
        if (soundLibrary == null) Debug.LogError("SceneSoundLibrary not found in the scene");
    }

    //Simple move fucntion
    public void MoveTo(Vector2 position)
    {
        if (!moving)
        {
            objective = position;
            pointer.initiateAnimation(objective);
            moving = true;
            _armature.animation.Play("Run_Loop");
        }
    }

    //Move and use, means there is a target object to use
    public void MoveAndUse(Vector2 position, GameObject objectToUse)
    {
        usingObject = true;
        targetObject = objectToUse;
        if (!moving)
        {
            objective = position;
            pointer.initiateAnimation(objective);
            moving = true;
            _armature.animation.FadeIn("Run_Loop", 0.1f, -1, 0);
        }
    }

    //Move and take, means there is a target object to take
    public void MoveTakeDrop(Vector2 objectPosition, GameObject objectToTake, Vector2 destinationPosition, GridCell destinationCell)
    {
        if (!destinationCell.getDisabledStatus())
        {
            takingObject = true;
            targetObject = objectToTake;
            if (!moving)
            {
                objective = objectPosition;
                moving = true;
                _armature.animation.FadeIn("Run_Loop", 0.1f, -1, 0);
            }

            droppingCell = destinationCell;
            finalObjective = destinationPosition;
        }
        else
        {
            arrowMove.GetComponent<SpriteRenderer>().enabled = false;
        }
    }


	//Returns the currently held object (could be null)
	public GameObject getCurrentObject()
    {
        return currentObject;
    }

	//Sets the currently held object (This could be private I think!)
    public void setCurrentObject(GameObject newObject)
    {
        currentObject = newObject;
    }
	
    public void setEndRenderPlane(float plane)
    {
        objectiveRenderPlane = plane;
    }

    public bool getMoving()
    {
        return moving;
    }

    public bool isPlayerMovingObjectTo(Vector2 positionCheck)
    {
        Debug.Log("Checking: " + positionCheck);
        Debug.Log("Final objective: " + finalObjective);
        return (positionCheck == finalObjective && takingObject) || (droppingObject && positionCheck == objective);
    }
    // Update is called once per frame - Movement on FixedUpdate avoids influence of variable FPS.
    void FixedUpdate () {
		//When its decided that the player has to move.
		if (moving && !Utils.freezed)
        {
            Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
			//If the player hasn't reach the destination
            if (Vector2.SqrMagnitude(objective - currentPos) > Time.fixedDeltaTime * speed)
            {
                Vector2 newPos = Vector2.MoveTowards(currentPos, objective, speed * Time.fixedDeltaTime);
                transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            }
            else
            {
                //If the player reaches its destination
                pointer.finishAnimation();
                moving = false; //No longer moving
                transform.position = new Vector3(objective.x, objective.y, objectiveRenderPlane);
                // If the purpose of the movement was using an object.
                _armature.animation.FadeIn("Idle_Loop", 0.1f, -1, 0);
                if (usingObject && targetObject != null)
                {
                    if (trackController != null && soundLibrary != null && !targetObject.GetComponent<BasicObject>().isActive() && !targetObject.GetComponent<BasicObject>().isDestroyed())
                        trackController.PlaySoundClip(1, soundLibrary.SelectSound("player", 0));
                    targetObject.GetComponent<BasicObject>().Use();
                    usingObject = false; //Action is finished
                    setCurrentObject(null); //Object UNasigned to player
                }
                // If the purpose of the movement was taking an object.
                if (takingObject && targetObject != null && !targetObject.GetComponent<BasicObject>().isDestroyed())
                { 
					//This places the object in a position relative to player
                    targetObject.transform.parent = objectHolder.transform;
                    targetObject.transform.localPosition = Vector3.zero;
                    //Detaches it from the cell
					targetObject.GetComponent<GridObject>().setParentCell(null);
                    if (trackController != null && soundLibrary != null) trackController.PlaySoundClip(1, soundLibrary.SelectSound("player", 1));
                    takingObject = false; //Action is finished
                    setCurrentObject(targetObject); //Object asigned to player
                    // render.sprite = carryingObj; //Sets the animation
                    _armature.animation.Play("Pick_Object_OnePlay", 1);
                    droppingObject = true;
                    targetObject = null;
                    objective = finalObjective;
                    moving = true;
                    _armature.animation.FadeIn("Run_Obj_Loop", 0.2f, -1, 0);
                }
				// If the purpose of the movement was dropping an object.
                else if (droppingObject && targetObject == null)
                {
                    arrowMove.GetComponent<SpriteRenderer>().enabled = false;
                    //Detach object from player and place it on the ground
                    currentObject.transform.parent = null;
                    getCurrentObject().transform.position = transform.position;
                    getCurrentObject().transform.rotation = transform.rotation;
                    //Attach it to the cell
                    //getCurrentObject().GetComponent<GridObject>().setParentCell(droppingCell);
                    droppingCell.setObjectInside(getCurrentObject());
                    if (trackController != null && soundLibrary != null) trackController.PlaySoundClip(1, soundLibrary.SelectSound("player", 2));
                    OnObjectMoved();
                    droppingObject = false; //Action finished
                    setCurrentObject(null); //Object UNasigned to player
                    _armature.animation.Play("Drop_Object_OnePlay", 1);
                    // render.sprite = idle; //Sets the animation
                    moving = false;
                }  
            }
        }
	}
}
