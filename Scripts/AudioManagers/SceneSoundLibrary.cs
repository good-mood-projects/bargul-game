using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSoundLibrary : MonoBehaviour
{
    public AudioClip[] backgroundLibrary;
    public AudioClip[] fxLibrary;
    public AudioClip[] uiLibrary;
    public AudioClip[] meteorLibrary;
    public AudioClip[] playerLibrary;
     
    /// <summary>
    /// Returns a sound from the scene sound library, in the given array. 
    /// </summary>
    /// <param name="arrayName"> background, fx, ui, meteor, player </param>
    /// <param name="soundIndex"> Index number in the array of the sound to play </param>
    public AudioClip SelectSound(string arrayName, int soundIndex)
    {
        switch(arrayName)
        {
            case "background":
                return backgroundLibrary[soundIndex];

            case "fx":
                return fxLibrary[soundIndex];

            case "ui":
                return uiLibrary[soundIndex];

            case "meteor":
                return meteorLibrary[soundIndex];

            case "player":
                return playerLibrary[soundIndex];

            default:
                return null;
        }
    }
}
