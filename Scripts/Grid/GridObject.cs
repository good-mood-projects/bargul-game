using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* This script should be attached to any 
* object located on the grid (player 
* objects or even the player itself)
*/
public class GridObject : MonoBehaviour {

	//Initial position (to be accessed by the GameController when instancing)
    public int xPosition;
    public int yPosition;

    int initialXPosition;
    int initialYPosition;

	//Used to establish render priority
    public int Plane = 1;

	//A parent cell, "null" if the object is not asigned to a cell
    GridCell parentCell;
    GridCell lastParentCell;

	//Getter of Vector2 Position on Grid.
    public Vector2 getPosition()
    {
        return new Vector2(xPosition, yPosition);
    }

    public Vector2 getInitialPosition()
    {
        return new Vector2(initialXPosition, initialYPosition);
    }

	//Getter of render priority (to be added to nearclipPlane value)
    public int getPlane()
    {
        return Plane;
    }


    public void Awake()
    {
        initialYPosition = yPosition;
        initialXPosition = xPosition;
    }

    public void attachToPreviousCell()
    {
        if (lastParentCell != null)
        {
            lastParentCell.setObjectInside(gameObject);
        }
    }

	//Allows to indicate parent cell and also propagates this decision to the cell itself
    public void setParentCell(GridCell cell)
    {
        if (cell == null)
        {
            if (parentCell != null)
            {
                parentCell.detatchObjectFromCell();
                lastParentCell = parentCell;
            }
        }
        else
        {
            int desiredPlane = /*(int)cell.getGridPosition().x +*/ 7 - (int)cell.getGridPosition().y;
            Vector2 screenPos = cell.getScreenPosition();
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane + desiredPlane));
            xPosition = (int)cell.getGridPosition().x;
            yPosition = (int)cell.getGridPosition().y;
            Plane = desiredPlane;
        }
        parentCell = cell;
    }

    public GridCell getParentCell()
    {
        return parentCell;
    }

    public GridCell getLastParentCell()
    {
        return lastParentCell;
    }
}
