using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUsesDisplayManager : MonoBehaviour
{
    public float imagesDistance = 60.0f;
    public float timeToFade = 1.0f; // Time the images need to appear / dissapear

    public GameObject imagePrefab;
    

    List<Image> imageList;
    int listPointer;
    int amountOfUses = 5;

    /// <summary>
    /// Sets and dysplays all the respawning HUD uses.
    /// </summary>
    /// <param name="amount"></param>
    public void SetAmountOfUses(int amount)
    {
        amountOfUses = amount;
        Instantiate();
    }

    /// <summary>
    /// Call this method after setting the "amountOfUses" to respawn objects to instantiate the HUD images
    /// </summary>
    /// <returns> True if executed correctly </returns>
    // Uncoment this line for testing purposes: [ContextMenu("Instantiate")]
    bool Instantiate ()
    {
        imageList = new List<Image>(); // Avoids double listing IF intantiating multiple times (wich shouldnt, just deffensive purposes)
        // Avoids duplication
        foreach (RectTransform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (imagePrefab == null) return false; // Cant execute properly
        // Two options:
        // Pair amount of uses
        if (amountOfUses % 2 == 0)
        {
            // Instantiates the prefabs in the correct position & Adds the images to the manager list
            for (int aux = 0 - (amountOfUses / 2); aux < amountOfUses / 2; aux++)
            {
                GameObject useImage = Instantiate(imagePrefab, Vector3.zero, transform.rotation, transform); // Instantiates the prefabs in the correct position
                useImage.transform.localPosition = Vector3.up * (imagesDistance * aux + imagesDistance / 2); // Sets the local position correctly
                imageList.Add(useImage.GetComponent<Image>()); // Adds the image to the manager list
            }
            listPointer = imageList.Count-1; // Sets the index for list managing
            return true;
        }
        // Odd amount of uses
        else
        {
            // Instantiates the prefabs in the correct position & Adds the images to the manager list
            for (int aux = 0 - (amountOfUses / 2); aux <= amountOfUses / 2; aux++)
            {
                GameObject useImage = Instantiate(imagePrefab, Vector3.zero, transform.rotation, transform);
                useImage.transform.localPosition = Vector3.up * imagesDistance * aux; // Sets the local position correctly
                imageList.Add(useImage.GetComponent<Image>());
            }
            listPointer = imageList.Count-1; // Sets the index for list managing
            return true;
        }
    }
    
    /// <summary>
    /// Call this method to hide the next image
    /// </summary>
    public void Used()
    {
        if(listPointer > -1)
        {
            imageList[listPointer].CrossFadeAlpha(0.0f, timeToFade, true);
            listPointer--;
        }
    }
    
    /// <summary>
    /// Call this method to show the last hiden image
    /// </summary>
    public void Restored()
    {
        if (listPointer < imageList.Count-1)
        {
            listPointer++;
            imageList[listPointer].CrossFadeAlpha(255.0f, timeToFade, true);
        }
    }

    public bool HasUses()
    {
        if (listPointer > -1) return true;
        else return false;
    }
}
