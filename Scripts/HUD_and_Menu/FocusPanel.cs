using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusPanel : MonoBehaviour {

    bool focusing = false;
    bool defocusing = false;
    bool finished = false;

    public float alphaMax = 0.8f;
    public float speed = 2.0f;
    
	
    public void InitializeFocus(Vector2 screenPos)
    {
        GetComponent<RectTransform>().position = new Vector2(screenPos.x, screenPos.y - 15.0f);
        
        Color currentColor = GetComponent<Image>().color;
        currentColor.a = 0.0f;
        GetComponent<Image>().color = currentColor;
        focusing = true;
        defocusing = false;
        finished = false;
    }

    public void FinishFocus()
    {
        focusing = false;
        defocusing = true;
    }

    public bool isFinished()
    {
        return finished;
    }

	// Update is called once per frame
	void Update () {
        Color currentColor = GetComponent<Image>().color;
        float alphaCurrent = currentColor.a;
        if (focusing)
        {
            if (currentColor.a < 0.8f)
            {
                alphaCurrent = currentColor.a + Time.deltaTime * speed;
                GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, alphaCurrent);
            }
            else
            {
                focusing = false;
            }
        }

        if (defocusing)
        {
            if (currentColor.a > 0.0f)
            {
                alphaCurrent = currentColor.a - Time.deltaTime * speed;
                GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, alphaCurrent);
            }
            else
            {
                defocusing = false;
                finished = true;
            }
        }
    }
}
