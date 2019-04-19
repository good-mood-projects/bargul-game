using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMovement : MonoBehaviour
{
    public float transitionTime = 0.4f;

    public GameObject normal;
    public GameObject settings;
    public GameObject credits;

    void Awake()
    {

    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Normal();
        }
    }

    public void Normal()
    {
        settings.SetActive(false);
        credits.SetActive(false);
        normal.SetActive(true);
    }

    public void Settings()
    {
        normal.SetActive(false);
        settings.SetActive(true);
    }

    public void Credits()
    {
        normal.SetActive(false);
        credits.SetActive(true);
    }

}
 