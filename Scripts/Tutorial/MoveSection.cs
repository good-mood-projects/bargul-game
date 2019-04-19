using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This tutorial section asks the user to move to other point (3,2) from original position
public class MoveSection : MonoBehaviour, ITutorialSection
{

    public string locale = "ES";
    bool setUp = false;
    bool enabledSection = false;

    public ArrowMovPointer pointArrow;
    public GridObject playerGrid;

    public void setLanguage(string value)
    {
        locale = value;
    }

    public void loadMeteorMap(MeteorPattern[] newPatternList)
    {
        throw new NotImplementedException();
    }

    public void setUpSection()
    {
        GridObject[] objects = FindObjectsOfType<GridObject>();
        foreach (GridObject element in objects)
        {
            if (element.gameObject.GetComponent<BasicObject>() != null)
            {
                element.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                element.gameObject.GetComponent<BasicObject>().setDestroyed(true);
                element.setParentCell(null);
            }
        }
        pointArrow.initiateAnimation(Camera.main.ScreenToWorldPoint(gameObject.GetComponent<GameController>().getScreenPositionFromGrid(new Vector2(3.0f,2.0f))));
    }

    public bool waitForCondition()
    {
        if (playerGrid.getPosition() == new Vector2(3, 2))
        {
            pointArrow.finishAnimation();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool waitForUserInteraction()
    {
        throw new NotImplementedException();
    }

    
     public void enableSection()
    {
        enabledSection = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (enabledSection)
        {
            if (!setUp)
            {
                setUpSection();
                setUp = true;
            }
            else
            {
                if (waitForCondition())
                {
                    gameObject.GetComponent<LevelManager>().nextTutorialSection();
                    enabledSection = false;
                }
            }
        }

	}
}
