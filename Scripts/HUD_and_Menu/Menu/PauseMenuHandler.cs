using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuHandler : MonoBehaviour
{
    GameObject child;
    public GameObject othersToControl; // GameObject to hide and show when menu procs
    public LevelManager levelManager;

    void Start()
    {
        if (othersToControl == null) Debug.LogError("The HUD to hide when menu opens isnt set. (PauseMenuHandler in PauseMenu game object)");
        child = GetComponentInChildren<RectTransform>().gameObject;
        child.SetActive(false);
    }

    public void Pause()
    {
        if (!levelManager.isPaused())
        {
            child.SetActive(true); // Shows the menu
            othersToControl.SetActive(false); // Hides the rest
            levelManager.PauseGame();
        }
    }


    public void Resume()
    {
        child.SetActive(false); // Hides the menu
        othersToControl.SetActive(true); // Shows the rest
        levelManager.UnpauseGame();
    }
}
