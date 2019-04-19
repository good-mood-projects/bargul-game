using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class TrackController : MonoBehaviour {

    public static TrackController instance; // Instance of the singleton
    private SceneSoundLibrary soundLibrary; // Script wich contains all the used sounds for the scene. 

    // Not used directly but prepared in case of need of a specific track call
    AudioSource backgroundTrack;
    AudioSource fxTrack;
    AudioSource uiTrack;

    AudioSource[] audioTracks; // 0 is Background; 1 is FX; 2 is UI

    // Slider volume values (Between 0 & 1)
    float _masterVolume       = 1f;
    float _backgroundVolume   = .5f;
    float _fxVolume           = .3f;
    float _uiVolume           = .2f;

    // Mute/Unmute control variables
    bool backgroundMuted = false;
    bool fxMuted = false;
    bool uiMuted = false;

    #region Volume Setters & Getters
    public float masterVolume
    {
        get
        {
            return _masterVolume;
        }

        set
        {
            value = Mathf.Clamp(value / 100.0f , 0.0f, 1.0f);
            _masterVolume = value;

            // Sets the volume of the other tracks proportionally in the AudioSource
            ChangeTrackVolume(0, masterVolume * backgroundVolume); 
            ChangeTrackVolume(1, masterVolume * fxVolume);
            ChangeTrackVolume(2, masterVolume * uiVolume);
        }
    }

    public float backgroundVolume
    {
        get
        {
            return _backgroundVolume;
        }

        set
        {
            value = Mathf.Clamp(value / 100.0f, 0.0f, 1.0f);
            _backgroundVolume = value;

            // Sets the volume of the track proportionally to the master
            ChangeTrackVolume(0, masterVolume * backgroundVolume);
        }
    }

    public float fxVolume
    {
        get
        {
            return _fxVolume;
        }

        set
        {
            value = Mathf.Clamp(value / 100.0f, 0.0f, 1.0f);
            _fxVolume = value;

            // Sets the volume of the track proportionally to the master
            ChangeTrackVolume(1, masterVolume * fxVolume);
        }
    }

    public float uiVolume
    {
        get
        {
            return _uiVolume;
        }

        set
        {
            value = Mathf.Clamp(value / 100.0f, 0.0f, 1.0f);
            _uiVolume = value;

            // Sets the volume of the track proportionally to the master
            ChangeTrackVolume(2, masterVolume * uiVolume);
        }
    }
    #endregion

    // Use this for avoid get destroyed on scene load & not getting declarated twice
    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;

            // Requires 3 tracks to function correctly
            // Creates and sets the tracks.
            uiTrack         = this.gameObject.AddComponent<AudioSource>();
            fxTrack         = this.gameObject.AddComponent<AudioSource>(); 
            backgroundTrack = this.gameObject.AddComponent<AudioSource>();

            // Adds the tracks into an array for numeric calls. Tracks are stored in the array in the create inverse order.
            audioTracks = FindObjectsOfType<AudioSource>();

            // Sets the AudioSource components correctly
            foreach (AudioSource track in audioTracks)
            {
                track.playOnAwake = false;
            }
            audioTracks[0].loop = true; // Prepares the background track to loop at the end of the clip

            // Loads audio volumes from previous seasons (if were any)
            LoadAudioValues();
        }
    }

    void Start() // Called everytime the object loads on a new scene
    {
        // Loads the new scene sound library
        soundLibrary = GameObject.FindObjectOfType<SceneSoundLibrary>().GetComponent<SceneSoundLibrary>();
        if (soundLibrary == null) Debug.LogError("SceneSoundLibrary not found in the scene");

        if(soundLibrary.backgroundLibrary.Length != 0) // Checks if there is any background clip to play on the scene
        {
            // Plays the new scene background cancelling the last one
            PlayBackgroundClip(soundLibrary.SelectSound("background", 0));
        }
        else
        {
            backgroundTrack.Stop(); // Avoids sounds from last scene keep playing when a new background is not set.
        }
    }

    void OnLevelWasLoaded() // Recalls "Start()" on new scenes loaded
    {
        Start();
    }

    void OnApplicationQuit() // Saves before quiting the application
    {
        SaveAudioValues();
    }

    void OnApplicationPause() // Saves when the "home" button is pressed on Android & IOS devices
    {
        SaveAudioValues();
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update ()
    {
        /* Debuging purposes before sliders implemention 
        if (Input.GetKeyDown(KeyCode.Q)) masterVolume -= 0.1f;
        if (Input.GetKeyDown(KeyCode.W)) backgroundVolume -= 0.1f;
        if (Input.GetKeyDown(KeyCode.E)) fxVolume -= 0.1f;
        if (Input.GetKeyDown(KeyCode.R)) uiVolume -= 0.1f;
        */
    }

    /// <summary>
    /// Loads a sound on a track and plays it.
    /// </summary>
    /// <param name="trackNumber"> Index of the track to play (0 is Background; 1 is FX; 2 is UI) </param>
    /// <param name="clipToPlay"> Clip to play from the scene sound library </param>
    public void PlaySoundClip(int trackNumber, AudioClip clipToPlay)
    {
        audioTracks[trackNumber].PlayOneShot(clipToPlay);
    }

    /// <summary>
    /// Loads a sound on the background track and plays it on loop.
    /// </summary>
    /// <param name="clipToPlay"> Clip to play from the scene sound library </param>
    public void PlayBackgroundClip(AudioClip clipToPlay)
    {
        backgroundTrack.clip = clipToPlay;
        backgroundTrack.Play();
    }

    /// <summary>
    /// Changes the volume of a given track.
    /// </summary>
    /// <param name="trackNumber"> Index of the track to change volume (0 is Background; 1 is FX; 2 is UI) </param>
    /// <param name="vol"> New volume value (0.0 to 1.0) </param>
    void ChangeTrackVolume(int trackNumber, float vol)
    {
        audioTracks[trackNumber].volume = vol;
        switch(trackNumber)
        {
            case 0:
                if (backgroundMuted)
                {
                    backgroundMuted = false; // Unmute
                    VolSlidersController localScript = GameObject.FindObjectOfType<VolSlidersController>().GetComponent<VolSlidersController>();
                    if(localScript != null) // Check if any
                    {
                        localScript.SwapMutedIconsSprite(0); // Sets the muted icon to unmuted
                    }
                }
                    break;

            case 1:
                if (fxMuted)
                {
                    fxMuted = false; // Unmute
                    VolSlidersController localScript = GameObject.FindObjectOfType<VolSlidersController>().GetComponent<VolSlidersController>();
                    if (localScript != null) // Check if any
                    {
                        localScript.SwapMutedIconsSprite(1); // Sets the muted icon to unmuted
                    }
                }
                    break;

            case 2:
                if (uiMuted)
                {
                    uiMuted = false; // Unmute
                    VolSlidersController localScript = GameObject.FindObjectOfType<VolSlidersController>().GetComponent<VolSlidersController>();
                    if (localScript != null) // Check if any
                    {
                        localScript.SwapMutedIconsSprite(2); // Sets the muted icon to unmuted
                    }
                }
                break;

            default:
                Debug.LogError("Some parameters are incorrect in the change volume method (Must be 0 Background, 1 FX or 2 UI)");
                break;
        }
    }

    /// <summary>
    /// Toggles between muting or unmuting a given track.
    /// </summary>
    /// <param name="trackNumber"> Index of the track to mute/unmute (0 is Background; 1 is FX; 2 is UI) </param>
    public void MuteToggle(int trackNumber)
    {
        switch(trackNumber)
        {
            case 0:
                if (backgroundMuted) // Unmute
                {
                    ChangeTrackVolume(0, masterVolume * backgroundVolume);
                    backgroundMuted = false;
                }
                else //Mute
                {
                    ChangeTrackVolume(0, 0.0f);
                    backgroundMuted = true;
                }
                break;

            case 1:
                if (fxMuted) // Unmute
                {
                    ChangeTrackVolume(1, masterVolume * fxVolume);
                    fxMuted = false;
                }
                else //Mute
                {
                    ChangeTrackVolume(1, 0.0f);
                    fxMuted = true;
                }
                break;

            case 2:
                if (uiMuted) // Unmute
                {
                    ChangeTrackVolume(2, masterVolume * uiVolume);
                    uiMuted = false;
                }
                else //Mute
                {
                    ChangeTrackVolume(2, 0.0f);
                    uiMuted = true;
                }
                break;

            default:
                Debug.LogError("Some parameters are incorrect in the toggle mute buttons (Must be 0 Background, 1 FX or 2 UI)");
                break;
        }
    }

    /// <summary>
    /// Returns true if the track is muted
    /// </summary>
    /// <param name="trackNumber"> Index of the track to check (0 is Background; 1 is FX; 2 is UI) </param>
    /// <returns></returns>
    public bool MuteStatus(int trackNumber)
    {
        switch (trackNumber)
        {
            case 0:
                return backgroundMuted;

            case 1:
                return fxMuted;

            case 2:
                return uiMuted;
        }

        Debug.LogError("Something went wrong while checking tracks 'mute status'");
        return true;
    }

    /// <summary>
    /// Saves the audio values for the next season
    /// </summary>
    // Uncoment this to save manually through the inspector: [ContextMenu("Save vol values")]
    void SaveAudioValues()
    {
        PlayerPrefs.SetFloat("masterVolume",masterVolume);
        PlayerPrefs.SetFloat("backgroundVolume",backgroundVolume);
        PlayerPrefs.SetFloat("fxVolume",fxVolume);
        PlayerPrefs.SetFloat("uiVolume",uiVolume);
    }

    /// <summary>
    /// Loads the audio values used on the last season
    /// </summary>
    void LoadAudioValues()
    {
        masterVolume = PlayerPrefs.GetFloat("masterVolume", masterVolume) * 100.0f;
        Debug.Log(masterVolume);
        backgroundVolume = PlayerPrefs.GetFloat("backgroundVolume", backgroundVolume) * 100.0f;
        fxVolume = PlayerPrefs.GetFloat("fxVolume",fxVolume) * 100.0f;
        uiVolume = PlayerPrefs.GetFloat("uiVolume",uiVolume) * 100.0f;
    }
}
