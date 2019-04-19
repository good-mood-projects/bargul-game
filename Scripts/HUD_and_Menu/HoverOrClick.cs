using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]

public class HoverOrClick : MonoBehaviour 
{
	private TrackController trackController; // Script wich controlls the tracks to play sounds.
    private SceneSoundLibrary soundLibrary; // Script wich contains all the used sounds for the scene. 
	private BoxCollider2D myCollider; // Collider component for hovering check.
	private bool hoverProc = false;
	private bool lastHoverState = false;

	void Start()
	{
        Utils.freezed = false;
		myCollider = GetComponent<BoxCollider2D>();
        if(myCollider == null)
        {
            Debug.LogError("Button collider for hovering not found.");
        }

        trackController = GameObject.FindObjectOfType<TrackController>().GetComponent<TrackController>();
        soundLibrary = GameObject.FindObjectOfType<SceneSoundLibrary>().GetComponent<SceneSoundLibrary>();

        if (trackController == null) Debug.LogError("TrackController never got instanciated (Scene persistent object)");
        if (soundLibrary == null) Debug.LogError("SceneSoundLibrary not found in the scene");
    }

	// Update is called once per frame
	void Update ()
	{
		if (myCollider != null)
		{
			if(myCollider.OverlapPoint(Input.mousePosition))
			{
				hoverProc = true;
				if (hoverProc != lastHoverState)
				{
					//Hover();
					lastHoverState = true;
				}
			}
			else
			{
                if(lastHoverState)
                {
                    hoverProc = false;
                    lastHoverState = false;
                }
			}
		}
	}

    /// <summary>
    /// Hover activates.
    /// </summary>
	public void Hover()
	{
        if (trackController != null && soundLibrary != null) trackController.PlaySoundClip(2, soundLibrary.SelectSound("ui", 0));
	}

    /// <summary>
    /// Click activates.
    /// </summary>
	public void Click()
    {
        if (trackController != null && soundLibrary != null) trackController.PlaySoundClip(2, soundLibrary.SelectSound("ui", 0));
    }
}
 