using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseSection : MonoBehaviour, ITutorialSection {

    public string locale = "ES";

    bool setUp = false;
    bool enabledSection = false;
    bool userInteraction = false;

    public ArrowMovPointer pointArrow;
    public ArrowMovSlider slideArrow;
    public GridObject playerGrid;
    public GridObject telescope;

    public RespawnUsesSpriteManager usesSprite;

    public Text tutText;

    public MeteorSpawner spawner;

    public void enableSection()
    {
        enabledSection = true;
    }


    public void resetEverything()
    {
        gameObject.GetComponent<LevelManager>().resetMeteorMap();
        Meteor[] meteors = FindObjectsOfType<Meteor>();
        foreach (Meteor met in meteors)
        {
            Destroy(met.gameObject);
        }
        gameObject.GetComponent<GameController>().resetGridColor();

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

        gameObject.GetComponent<GameController>().setInitialObjects(true);
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

        telescope.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        telescope.gameObject.GetComponent<BasicObject>().setDestroyed(false);
        telescope.attachToPreviousCell();
        pointArrow.initiateAnimation(Camera.main.ScreenToWorldPoint(telescope.GetComponent<GridObject>().getParentCell().getScreenPosition()));
        if (locale == "ES")
            tutText.text = "Selecciona un objeto para acercarte y usar su poder";
        else
            tutText.text = "Select an object to move closer and use its power";
    }

    public void loadMeteorMap(MeteorPattern[] newPatternList)
    {
        spawner.setPattern(newPatternList);
    }

    public bool waitForCondition()
    {
        if (telescope.getPosition() == new Vector2(3, 2))
        {
            slideArrow.finishAnimation();
            return true;
        }
        else if (telescope.GetComponent<BasicObject>().isDestroyed())
        {
            resetEverything();
            setUpSection();
            usesSprite.SwapTelescope();
            gameObject.GetComponent<LevelManager>().decreaseMeteorsHit(1);
            userInteraction = false;
            return false;
        }
        else if (telescope.getPosition() != new Vector2(3, 2))
        {
            slideArrow.updateInitialPos(Camera.main.ScreenToWorldPoint(gameObject.GetComponent<GameController>().getScreenPositionFromGrid(telescope.getPosition())));
            return false;
        }
        else
        {
            return false;
        }
    }

    public bool waitForUserInteraction()
    {
        if (playerGrid.getPosition() == telescope.getPosition())
        {
            if (locale == "ES")
                tutText.text = "Salva tus objetos. ¡Cuidado, un meteorito! (Sálvalo arrastrando)";
            else
                tutText.text = "Keep your objects away from danger. Look out! \nA meteor! (Swipe to move) ";
            pointArrow.finishAnimation();
            MeteorShapeGenerator shapeGenerator = new MeteorShapeGenerator();
            MeteorPattern pattern = new MeteorPattern();
            pattern.initialPos = new List<Vector2>() { telescope.getPosition() };
            pattern.initTime = 0.0f;
            pattern.timeToStart = new List<float>() { 0.0f };
            pattern.timeToFall = new List<float>() { 5.0f };
            pattern.warningTime = 5.0f;
            pattern.endTime = 0.0f;
            pattern.meteorShape = new List<Shape>() { shapeGenerator.generateSquareShape(1) };
            loadMeteorMap(new MeteorPattern[1] { pattern });
            slideArrow.initiateAnimation(Camera.main.ScreenToWorldPoint(gameObject.GetComponent<GameController>().getScreenPositionFromGrid(telescope.getPosition())), Camera.main.ScreenToWorldPoint(gameObject.GetComponent<GameController>().getScreenPositionFromGrid(new Vector2(3.0f, 2.0f))));
            return true;
        }
        else
        {
            return false;
        }
    }

    // Use this for initialization
    void Start () {
		
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
                if (!userInteraction)
                {
                    userInteraction = waitForUserInteraction();
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

    public void setLanguage(string value)
    {
        locale = value;
    }
}
