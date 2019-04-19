using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Translate_Menu : MonoBehaviour {

    public Text playButton;
    public Text optionsButton;
    public Text exitButton;
    public Text backButton;
    public Text musicText;
    public Text UIText;
    public Text FXText;
    public Text twitterText;

	// Use this for initialization
	void Awake()
    {
        if ( Application.systemLanguage != SystemLanguage.Spanish)
        {
            SetupEnglish();
        }
    }

    void SetupEnglish()
    {
        playButton.text = "Play";
        optionsButton.text = "Settings";
        exitButton.text = "Exit";
        backButton.text = "Back";
        musicText.text = "Music:";
        UIText.text = "Interface:";
        FXText.text = "Sound FX:";
        twitterText.text = "Follow us\non Twitter!";
    }
}
