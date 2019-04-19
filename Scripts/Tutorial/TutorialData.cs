using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class to save and load the tutorial status
/// </summary>
[System.Serializable]
public class TutorialData {


    bool forceTutorial;
    bool tutorialPlayedOnce = false;

    public TutorialData()
    {
        forceTutorial = false;
        tutorialPlayedOnce = false;
    }

    public TutorialData(bool _forceTut, bool _tutPlayed)
    {
        forceTutorial = _forceTut;
        tutorialPlayedOnce = _tutPlayed;
    }

    public bool getForceTut()
    {
        return forceTutorial;
    }

    public bool getPlayedOnce()
    {
        return tutorialPlayedOnce;
    }
}
