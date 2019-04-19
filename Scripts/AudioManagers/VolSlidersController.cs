using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolSlidersController : MonoBehaviour
{
    public Slider master;       // Soundtrack volume controller
    public Slider background;   // Soundtrack volume controller
    public Slider fx;           // Soundtrack volume controller
    public Slider ui;           // Soundtrack volume controller

    public Image[] MuteIconArray;    // Images to display the mute/unmute feedback

    public Sprite soundIcon;    // Sprite to display when a track is not muted
    public Sprite mutedIcon;    // Sprite to display when a track is muted

    private TrackController trackController; // Script wich controlls the tracks to play sounds.
    private SceneSoundLibrary soundLibrary; // Script wich contains all the used sounds for the scene.

    // Use this for initialization
    void Start ()
    {
        trackController = GameObject.FindObjectOfType<TrackController>().GetComponent<TrackController>();
        soundLibrary = GameObject.FindObjectOfType<SceneSoundLibrary>().GetComponent<SceneSoundLibrary>();

        if (trackController == null) Debug.LogError("TrackController never got instanciated (Scene persistent object)");
        if (soundLibrary == null) Debug.LogError("SceneSoundLibrary not found in the scene");
        if (MuteIconArray.Length != 3) Debug.LogError("'MuteIconArray' not set correctly in 'VolSlidersController': 0 is background, 1 is FX, 2 is UI");
        if (soundIcon == null || mutedIcon == null) Debug.LogError("Mute/Unmute icons not set in 'VolSlidersController' in the settings object of the UI");

        // Multiplies on 100 to set the correct slider values (o - 100 instead of 0 - 1)
        master.value = trackController.masterVolume * 100;
        background.value = trackController.backgroundVolume * 100;
        fx.value = trackController.fxVolume * 100;
        ui.value = trackController.uiVolume * 100;
    } 

    public void MasterSlider()
    {
        trackController.masterVolume = master.value;
    }

    public void BackgroundSlider()
    {
        trackController.backgroundVolume = background.value;
    }

    public void FxSlider()
    {
        trackController.fxVolume = fx.value;
        // Call a sound
        //if (trackController != null && soundLibrary != null) trackController.PlaySoundClip(1, soundLibrary.SelectSound("fx", 0));
    }

    public void UISlider()
    {
        trackController.uiVolume = ui.value;
        // Call a sound
        //if (trackController != null && soundLibrary != null) trackController.PlaySoundClip(2, soundLibrary.SelectSound("ui", 0));
    }

    public void MuteUnmuteToggle(int trackToToggle)
    {
        /* SI QUEREMOS QUE LAS BARRAS SE PONGAN A 0
        switch (trackToToggle)
        {
            case 0:
                background.value = 0;
                break;

            case 1:
                fx.value = 0;
                break;

            case 2:
                ui.value = 0;
                break;        
        }*/
        trackController.MuteToggle(trackToToggle);
        SwapMutedIconsSprite(trackToToggle);
    }

    public void SwapMutedIconsSprite(int trackToToggle)
    {
        if (trackController.MuteStatus(trackToToggle) == true)
        {
            MuteIconArray[trackToToggle].sprite = mutedIcon;
        }
        else
        {
            MuteIconArray[trackToToggle].sprite = soundIcon;
        }
    }
}