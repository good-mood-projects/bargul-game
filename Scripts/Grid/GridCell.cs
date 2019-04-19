using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public GameObject hole;
    public Color initialColor = new Color(1.0f, 1.0f, 1.0f, 0.29019f);
    public Color hitColor = new Color(0.973f, 0.341f, 0.341f, 0.29019f);
    public Color shapeColor = new Color(0.973f, 0.341f, 0.341f, 0.29019f);
    public Color warningColor = new Color(0.973f, 0.341f, 0.341f, 0.29019f);

    public PsPauseAndResume psExplosion; // Collision particle system manager

    //Internal position on the grid.
    int yPosition;
    int xPosition;

    int warning = 0;

    private int meteorHitCounter = 0;
    private int meteorShapeCounter = 0;

    //Envent to emit when the cell is hit by a raycast
    public delegate void SelectAction(Vector2 position);
    public event SelectAction OnSelected;

    public delegate void HoldOver(Vector2 position);
    public event HoldOver OnMouseOverGrid;

    //Single gameobject inside the cell
    GameObject objectInside;

    bool disabled = false;

    public void resetStatus()
    {
        meteorHitCounter = 0;
        meteorShapeCounter = 0;
        warning = 0;
        SetNewColor();
    }

    public void hitDisable()
    {
        //setColor(Color.black);
        hole.SetActive(true);
        disabled = true;
    }

    public void hitEnable()
    {
        disabled = false;
        hole.SetActive(false);
        //SetNewColor();
    }

    public bool getDisabledStatus()
    {
        return disabled;
    }

	//Setter of object in the cell
    public void setObjectInside(GameObject newObject)
    {
        objectInside = newObject;
        newObject.GetComponent<GridObject>().setParentCell(this);
    }

	//Remove object from cell
    public void detatchObjectFromCell()
    {
        //Debug.Log("Object no longer in cell");
        objectInside = null;
    }

	//Getter of current object inside (can be null)
    public GameObject getObjectInside()
    {
        return objectInside;
    }

	//Signaling of raycast hit.
    public void SignalSelected()
    {
        //if (!disabled)
            OnSelected(getGridPosition());
    }

    public void SignalHoldOver()
    {   //if (!disabled)
            OnMouseOverGrid(getGridPosition());
    }
	
	//Getter of grid position (used on signal)
    public Vector2 getGridPosition()
    {
        return new Vector2((float)xPosition, (float) yPosition);
    }

	//Setter used on GameController when instancing.
    public void setGridPosition(int x, int y)
    {
        xPosition = x;
        yPosition = y;
    }

	//To allow movement of sprites based on this object position.
    public Vector2 getScreenPosition()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    public bool DestroyObjectInside()
    {
        if (objectInside != null)
        {
            //DestroyObject(objectInside, 0.1f);
            //objectInside.SetActive(false);
            objectInside.GetComponent<BasicObject>().setDestroyed(true);
            InitExplosionPs(); // Activates explosion particles to blend the object disapearing
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void InitExplosionPs()
    {
        psExplosion.ActivatePS(); // Activates the particle system wich automanages itself
    }

    public void setColor(Color color)
    {
        if (!disabled)
        {
            gameObject.GetComponent<Renderer>().material.color = color;
        }
    }

    public void InitializeColor()
    {
        setColor(initialColor);
    }

    public void setHitColor()
    {
        IncreaseHitCounter();
        if (warning == 0) // Avoids overriding the warning color
            SetNewColor();
    }

    public void setShapeColor()
    {
        IncreaseShapeCounter();
        if (warning == 0) // Avoids overriding the warning color
            SetNewColor();
    }

    public void setHitColorWarning()
    {
        IncreaseWarningCounter();
        SetNewColor();
    }

    public void removeHitColor()
    {
        meteorHitCounter--;
        if (warning > 0)
        {
            warning--;
        }
        if (meteorHitCounter <= 0)
            meteorHitCounter = 0;
        SetNewColor();
    }

    public void removeShapeColor()
    {
        meteorShapeCounter--;
        if (warning > 0)
        {
            warning--;
        }
        if (meteorShapeCounter <= 0)
            meteorShapeCounter = 0;
        SetNewColor();
    }

    public void SetNewColor()
    {
        if(meteorShapeCounter == 0 && meteorHitCounter == 0)
        {
            if (warning == 0)
            {
                InitializeColor();
            }
            else
            {
                setColor(warningColor);
            }
        }
        else
        {
            if (warning == 0) // Avoids overriding the warning color
                setColor(hitColor);
            else
                setColor(warningColor);
        }
        /*
        else if (meteorHitCounter == 0)
        {
            if(warning == 0)
            {
                setColor(shapeColor);
            }
            else
            {
                setColor(warningColor);
            }

        }*/
    }

    public void IncreaseHitCounter()
    {
        meteorHitCounter++;
    }

    public void IncreaseShapeCounter()
    {
        meteorShapeCounter++;
    }

    public void IncreaseWarningCounter()
    {
        warning++;
    }

    public bool isBeingHit()
    {
        return (meteorHitCounter + meteorShapeCounter == 0);
    }
}
