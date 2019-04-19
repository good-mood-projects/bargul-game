using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUsesSpriteManager : MonoBehaviour {

    // Sprites to be swaped on the HUD items
    public Sprite telescopeColor;
    public Sprite telescopeGray;

    public Sprite abacusColor;
    public Sprite abacusGray;

    public Sprite repairColor;
    public Sprite repairGray;

    // HUD buttons to swap sprites
    public Image telescopeButton;
    public Image abacusButton;
    public Image repairButton;

    public ParticleSystem telescopePS;
    public ParticleSystem abacusPS;
    public ParticleSystem repairPS;

    private RespawnUsesDisplayManager usesCounter;

    #region InternalFunctions
    void Start()
    {
        if(telescopeButton == null || abacusButton == null || repairButton == null)
        {
            Debug.LogError("Buttons images of the respawn Uses not set");
        }
        telescopeButton.sprite = telescopeColor;
        abacusButton.sprite = abacusColor;
        repairButton.sprite = repairColor;

        telescopePS.Stop();
        abacusPS.Stop();
        repairPS.Stop();

        usesCounter = GetComponent<RespawnUsesDisplayManager>();
    }

    Sprite TelescopeSprite()
    {
        if (telescopeButton.sprite == telescopeColor)
        {
            if(usesCounter.HasUses())
                telescopePS.Play();
            return telescopeGray;
        }
        else
        {
            telescopePS.Stop();
            return telescopeColor;
        }
    }

    Sprite AbacusSprite()
    {
        if (abacusButton.sprite == abacusColor)
        {
            if (usesCounter.HasUses())
                abacusPS.Play();
            return abacusGray;
        }
        else
        {
            abacusPS.Stop();
            return abacusColor;
        }
    }

    Sprite RepairSprite()
    {
        if (repairButton.sprite == repairColor)
        {
            if (usesCounter.HasUses())
                repairPS.Play();
            return repairGray;
        }
        else
        {
            repairPS.Stop();
            return repairColor;
        }
    }
    #endregion

    public bool isDestroyed(int index)
    {
        switch(index)
        {
            case 0:
                return !(repairButton.sprite == repairColor);
            case 1:
                return !(abacusButton.sprite == abacusColor);
            case 2:
                return !(telescopeButton.sprite == telescopeColor);
            default:
                return true;
        }
    }
    /// <summary>
    /// Optional way of switching sprites using directly the "ObjectHandler" array index of the object. (Use with precaution if "ObjectHandler" objects array gets modified.)
    /// </summary>
    /// <param name="index"> Index of the object to swich sprites </param>
    public void SwapSwitch(int index)
    {
        switch(index)
        {
            case 0:
                SwapRepair();
                break;

            case 1:
                SwapAbacus();
                break;

            case 2:
                SwapTelescope();
                break;

            default:
                return;
        }
    }

    /// <summary>
    /// Optional way of switching sprites using directly the "ObjectHandler" object is getting removed. (Use with precaution if "ObjectHandler" objects array gets modified.)
    /// </summary>
    /// <param name="index"> Index of the object to swich sprites </param>
    public void SwapByObject(GameObject objectDestroyed)
    {
        if (objectDestroyed.GetComponent<RepairObject>() != null)
            SwapRepair();

        if(objectDestroyed.GetComponent<Abacus>() != null)
            SwapAbacus();

        if (objectDestroyed.GetComponent<Telescope>() != null)
            SwapTelescope();

    }

    /// <summary>
    /// Method to call to swap the telescope sprite on the HUD button
    /// </summary>
    public void SwapTelescope()
    {
        telescopeButton.sprite = TelescopeSprite();
    }

    /// <summary>
    /// Method to call to swap the abacus sprite on the HUD button
    /// </summary>
    public void SwapAbacus()
    {
        abacusButton.sprite = AbacusSprite();
    }

    /// <summary>
    /// Method to call to swap the gyroscope sprite on the HUD button
    /// </summary>
    public void SwapRepair()
    {
        repairButton.sprite = RepairSprite();
    }
}
