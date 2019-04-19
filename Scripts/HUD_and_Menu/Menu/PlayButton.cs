using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayButton : MonoBehaviour
{
    private TrackController trackController; // Script wich controlls the tracks to play sounds.
    private SceneSoundLibrary soundLibrary; // Script wich contains all the used sounds for the scene. 

    int interactionCounter; // Number of times the button has been pressed.

    public Image fadePanel;
    public float timeToFade = .5f;
    public float timeToMove; // Time used on each "dance".
    public float timeToFall = 2.0f; // Time used to meteor fall.

    public Slider loader;
    private AsyncOperation async;

    void Awake()
    {
        interactionCounter = 0;
        InitializeTutValues();
    }

    void Start()
    {
        trackController = GameObject.FindObjectOfType<TrackController>().GetComponent<TrackController>();
        soundLibrary = GameObject.FindObjectOfType<SceneSoundLibrary>().GetComponent<SceneSoundLibrary>();

        if (trackController == null) Debug.LogError("TrackController never got instanciated (Scene persistent object)");
        if (soundLibrary == null) Debug.LogError("SceneSoundLibrary not found in the scene");
        if (fadePanel == null) Debug.LogError("Fade panel not referenced in the playButton");
    }

    /// <summary>
    /// Manages the button dance and when to actually start the game
    /// </summary>
    public void OnClick()
    {
        Play(); // Runs the game.

        /*
        switch (interactionCounter)
        {
            case 0:
                if (trackController != null && soundLibrary != null)
                {
                    trackController.PlaySoundClip(1, soundLibrary.SelectSound("fx", 0));
                    trackController.PlaySoundClip(1, soundLibrary.SelectSound("fx", 1));
                }
                meteor.SetActive(false);
                StartCoroutine(Utils.MoveAToB(transform, targetPos1.localPosition, timeToMove));
                break;

            case 1:
                if (trackController != null && soundLibrary != null)
                {
                    trackController.PlaySoundClip(1, soundLibrary.SelectSound("fx", 0));
                    trackController.PlaySoundClip(1, soundLibrary.SelectSound("fx", 1));
                    trackController.PlaySoundClip(1, soundLibrary.SelectSound("fx", 2));
                }
                StartCoroutine(Utils.MoveAToB(transform, targetPos2.localPosition, timeToMove));
                Fall(meteorPiece2);
                Fall(meteorPiece4);
                Fall(meteorPiece6);
                break;

            case 2:
                Play(); // Runs the game.
                break;

            default:
                return;
        } 
        interactionCounter++;
        */
    }

    public void OnClickTutorial()
    {
        SaveTutValues(1, PlayerPrefs.GetInt("playedOnce"));
        Play();
    }

    /// <summary>
    /// What happens when play is triggered.
    /// </summary>
    void Play()
    {
        StartCoroutine(LoadLevelWithBar(1)); //Load Scene 1
        StartCoroutine(Utils.FadeOut(fadePanel, timeToFade));
        // ESPACIO PARA INTEGRAR EL CAMBIO A ESCENA DE JUEGO Y TODAS LAS OPERACIONES QUE ELLO REQUIERA. 
    }

    /*
    void Fall(RectTransform piece)
    {
        piece.SetParent(earthPosition.parent);
        piece.GetComponent<Spiner>().rotationSpeed = Random.Range(-375.0f, 375.0f);
        StartCoroutine(Utils.MoveAToB(piece, earthPosition.localPosition, timeToFall));
        StartCoroutine(Utils.Shrink(piece, piece.localScale, timeToFall));
    }
    */

    //Temporary async loader
    IEnumerator LoadLevelWithBar(int level)
    {
        loader.gameObject.SetActive(true);
        async = SceneManager.LoadSceneAsync(level);
        while (!async.isDone)
        {
            loader.value = async.progress;
            yield return null;
        }
    }





    void InitializeTutValues()
    {
        if (!PlayerPrefs.HasKey("forceTut"))
        {
            PlayerPrefs.SetInt("forceTut", 0);
            PlayerPrefs.SetInt("playedOnce", 0);

        }
    }
    void SaveTutValues(int forceTut, int playedOnce)
    {
        PlayerPrefs.SetInt("forceTut", forceTut);
        PlayerPrefs.SetInt("playedOnce", playedOnce);
    }

}
