using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public Sprite size1;
    public Sprite size2;
    public Sprite size3;

    public ParticleSystem psTrail;
    public Transform psTrailTransform;

    private TrackController trackController; // Script wich controlls the tracks to play sounds.
    private SceneSoundLibrary soundLibrary; // Script wich contains all the used sounds for the scene. 
    
    public float[] meteorFallingTimers; // Duration of the falling sounds
    public float[] meteorCollisionTimers; // Offset needed to launch the collision sound so the hit & crash are at the same time
    int fallingSoundIndex;
    int collideSoundIndex;
    bool fallingSoundOn = false;

    protected SpriteRenderer sprite;
    bool initialized = false;
    bool falling = false;
    GridCell _fallingPosition;
    float timeToStartFalling;
    GameController _gameController;
    Shape hitShape;
    
    float fallingTimeLeft;
    float warningTime;

    bool hitRendered = false;
    bool shapeRendered = false;

    Vector2 screenPosition;
    bool warning = true;

    bool freezed = false;

    bool makesHoles = false;


    public void setMakeHoles(bool value)
    {
        makesHoles = value;
    }

    public bool doesMakeHoles()
    {
        return makesHoles;
    }

    public void freeze()
    {
        freezed = true;
    }

    public void unfreeze()
    {
        freezed = false;
    }

    void Awake()
    {
        
    }

    void Start()
    {
        if(size1 == null || size2 == null || size3 == null)
        {
            Debug.LogError("No sprite image of meteors referenced in meteor prefab.");
        }
        if (psTrail == null) Debug.LogError("ParticleSystem of the trail not set in the meteor inspector");
        if (psTrailTransform == null) Debug.LogError("Transform of the trail for rotation not set in the meteor inspector");

        sprite = GetComponentInChildren<SpriteRenderer>();

        Vector3 newPos;
        newPos.x = Random.Range(-7.0f, 12.0f);
        newPos.y = Random.Range(10.0f, 15.0f);
        newPos.z = 0;

        sprite.transform.localPosition = newPos;

        // Set the rotation of the trail particle system:
        Vector3 direction = Vector3.zero - newPos;
        float psTrailAngle = Vector3.Angle(direction, Vector3.left) + 180.0f; // Inverts the trail direction with the + 180
        psTrailTransform.eulerAngles = new Vector3(0.0f, 0.0f, psTrailAngle);


    }

    public void Initialize(GridCell positionToHit, float timeToFall, float timeToStart, float warningSetTime, GameController controller, Shape shape)
    {
        // Starting audio components
        trackController = GameObject.FindObjectOfType<TrackController>().GetComponent<TrackController>();
        soundLibrary = GameObject.FindObjectOfType<SceneSoundLibrary>().GetComponent<SceneSoundLibrary>();

        if (trackController == null) Debug.LogError("TrackController never got instanciated (Scene persistent object)");
        if (soundLibrary == null) Debug.LogError("SceneSoundLibrary not found in the scene");

        if (soundLibrary.meteorLibrary.Length != meteorFallingTimers.Length)
        {
            Debug.LogError("Check the audio arrays coherence in meteor lib with the sound timers array in the meteor."); 
        }

        // It may be suitable to change the collision sound to a new lib in the SceneSoundLibrary & change code line 100 to the new lib array
        if (soundLibrary.fxLibrary.Length != meteorCollisionTimers.Length)
        {
            Debug.LogError("Check the audio arrays coherence in fx lib with the sound timers array in the meteor.");
        }

        // Rest of initialization
        hitShape = shape;
        _fallingPosition = positionToHit;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(_fallingPosition.getScreenPosition().x, _fallingPosition.getScreenPosition().y, Camera.main.nearClipPlane+1.0f));
        fallingTimeLeft = timeToFall;
        timeToStartFalling = timeToStart;
        warningTime = warningSetTime;
        _gameController = controller;
        screenPosition = positionToHit.getScreenPosition();
        fallingSoundIndex = Random.Range(0, soundLibrary.meteorLibrary.Length); // Selects the falling audio to clip
        collideSoundIndex = Random.Range(0, soundLibrary.fxLibrary.Length); // Selects the collision audio to clip
        initialized = true;
    }


    public void Die()
    {
        psTrail.Stop(); // Stops the particle system before getting destroyed
        sprite.enabled = false;
        Destroy(this.gameObject, meteorCollisionTimers[collideSoundIndex]);
    }

    // Update is called once per frame
    void Update ()
    {
        if (!freezed)
        {
            if (initialized)
            {
                timeToStartFalling -= Time.deltaTime;
                if (timeToStartFalling <= 0.0f)
                {
                    timeToStartFalling = 0.0f; // To avoid mistakes on the time count if the var gets below 0.
                                               //Debug.Log("Meteor starts moving"); 
                    sprite.sprite = SelectSpriteSize(); // Selects the sprite image to be displayed
                    sprite.enabled = true;
                    StartCoroutine(Utils.MoveAToB(sprite.transform, Vector2.zero, fallingTimeLeft));
                    falling = true;
                    initialized = false;
                }

                // Launches the audio clip at the proper time (only once)
                if (!fallingSoundOn && fallingTimeLeft + timeToStartFalling <= meteorFallingTimers[fallingSoundIndex])
                {
                    fallingSoundOn = true;
                    if (trackController != null && soundLibrary != null) trackController.PlaySoundClip(1, soundLibrary.SelectSound("meteor", fallingSoundIndex));
                }

                // Renders the warning square once at the proper time
                if (fallingTimeLeft + timeToStartFalling <= warningTime && warning)
                {
                    // _fallingPosition.setHitColorWarning();
                    foreach (Vector2 position in hitShape.GetHitShape())
                    {
                        Vector2 hitPos = _fallingPosition.getGridPosition() + position;
                        GridCell hitCell = null;
                        _gameController.getHashMap().TryGetValue(hitPos, out hitCell);
                        if (hitCell != null)
                            hitCell.setHitColorWarning();
                    }
                    warning = false;
                }
            }
            if (falling)
            {
                if (!psTrail.isPlaying) psTrail.Play(); // Plays / resumes the particle system

                fallingTimeLeft -= Time.deltaTime;

                // Launches the audio clip at the proper time (only once)
                if (!fallingSoundOn && fallingTimeLeft <= meteorFallingTimers[fallingSoundIndex])
                {
                    fallingSoundOn = true;
                    if (trackController != null && soundLibrary != null) trackController.PlaySoundClip(1, soundLibrary.SelectSound("meteor", fallingSoundIndex));
                }

                // Renders the warning square once at the proper time
                if (fallingTimeLeft <= warningTime && warning)
                {
                    // _fallingPosition.setHitColorWarning();
                    foreach (Vector2 position in hitShape.GetHitShape())
                    {
                        Vector2 hitPos = _fallingPosition.getGridPosition() + position;
                        GridCell hitCell = null;
                        _gameController.getHashMap().TryGetValue(hitPos, out hitCell);
                        if (hitCell != null)
                            hitCell.setHitColorWarning();
                    }
                    warning = false;
                }

                if (fallingTimeLeft <= 0.0f)
                {
                    Collision();
                    falling = false;
                }
            }
        }
        else
        {
            if(!psTrail.isPaused) psTrail.Pause(); // Pauses the particle system with the game
        }
	}

    public void RenderHitColor()
    {
        if(!hitRendered)
        {
            hitRendered = true; // Avoids activating the render more than once when using a item multiple times
            getAsignedCell().setHitColor();
        }
    }

    public bool ShapeRendered()
    {
        return shapeRendered;
    }

    public void DeactivateNextShapeRenders()
    {
        shapeRendered = true;
    }

    //Getters for magic objects
    public Vector2 getHitPosition()
    {
        return _fallingPosition.getGridPosition();
    }

    public GridCell getAsignedCell()
    {
        return _fallingPosition;
    }

    public float getTimeToStartFalling()
    {
        return timeToStartFalling;
    }

    public Shape getHitShape()
    {
        return hitShape;
    }

    public float getTimeToHit()
    {
        return fallingTimeLeft;
    }
    // Manages the collision of the meteor with the ground.
    private void Collision()
    {
        if (trackController != null && soundLibrary != null) trackController.PlaySoundClip(1, soundLibrary.SelectSound("fx", collideSoundIndex));
        _gameController.MeteorCollision(this);
    }

    // Manages the sprite selection depending on the shape size.
    private Sprite SelectSpriteSize()
    {
        switch(hitShape.GetSpriteImage())
        {
            case 1:
                return size1;

            case 2:
                return size2;

            case 3:
                return size3;

            default:
                Debug.LogError("Error on shape size of a meteor");
                return null;
        }
    }

    // Returns the variables neded to set on HUD items.
    #region HUD methods

    public float TimeLeft()
    {
        return timeToStartFalling + fallingTimeLeft;
    }

    public Vector2 FallingPos()
    {
        return screenPosition;
    }

    #endregion
}
