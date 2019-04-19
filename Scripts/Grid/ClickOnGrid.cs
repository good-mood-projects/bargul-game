using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
* This script should be attached to the main Camera 
*/
public class ClickOnGrid : MonoBehaviour
{

    private Ray ray;
    public LayerMask layer;
    bool holding = false;

    GridCell lastCellSelected; //Out of bounds selection

    void ProcessHit(GameObject gObject)
    {
        GridCell cell;
        GridObject gridObject = gObject.GetComponent<GridObject>();
        if (gridObject != null)
        {
            cell = gridObject.getParentCell();
            if (cell == null)
            {
                cell = gridObject.getLastParentCell();
            }
        }
        else
        {
            cell = gObject.GetComponent<GridCell>();
        }
        if (cell != null)
        { 
            cell.SignalSelected();
        }
        else
        {
            lastCellSelected.SignalSelected();
        }
// lastCellSelected = null;
    }

    void ColorMovement(GameObject gObject)
    {
        GridCell cell;
        GridObject gridObject = gObject.GetComponent<GridObject>();
        if (gridObject == null)
        {
            cell = gObject.GetComponent<GridCell>();
            if (cell != null)
            {
                lastCellSelected = cell;
                cell.SignalHoldOver();
            }
        }
    }
	// Update is called once per frame
	void Update ()
    {
        if (!Utils.freezed)
        {
            if (holding)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 400.0f, layer))
                {
                    // ProcessHit
                    Debug.DrawLine(transform.position, hit.point, Color.cyan);
                    ColorMovement(hit.transform.gameObject);
                }
            }
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began && !holding || Input.GetTouch(0).phase == TouchPhase.Ended && holding)
                {
                    if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) // Avoids touches over the UI
                    {
                        holding = !holding;
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 400.0f, layer))
                        {
                            // ProcessHit
                            Debug.DrawLine(transform.position, hit.point, Color.cyan);
                            ProcessHit(hit.transform.gameObject);
                        }
                        else
                        {
                            if (lastCellSelected != null)
                            {
                                lastCellSelected.SignalSelected();
                            }
                        }
                    }
                }
            }
            else if (Input.GetMouseButton(0) && !holding || !Input.GetMouseButton(0) && holding)
            {
                if (!EventSystem.current.IsPointerOverGameObject()) // Avoids clicking over the UI
                {
                    holding = !holding;
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 400.0f, layer))
                    {
                        // ProcessHit
                        Debug.DrawLine(transform.position, hit.point, Color.cyan);
                        ProcessHit(hit.transform.gameObject);
                    }
                    else
                    {
                        if (lastCellSelected != null)
                        {
                            lastCellSelected.SignalSelected();
                        }
                    }
                }
            }
        }
    }
}
