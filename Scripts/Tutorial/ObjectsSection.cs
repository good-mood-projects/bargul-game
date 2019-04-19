using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsSection : MonoBehaviour, ITutorialSection
{
    bool alreadyPlayed = false;
    int currentMeteors = 1;
    public string locale = "ES";
    bool setUp = false;
    bool enabledSection = false;
    bool userInteraction = false;

    int initialMeteors = 0;
    public ArrowMovPointer pointArrow;
    public GridObject playerGrid;
    public GridObject abacus;
    public GridObject telescope;

    public RespawnUsesSpriteManager usesSprite;


    public Text tutText;

    public MeteorSpawner spawner;

    public void enableSection()
    {
        enabledSection = true;
        setUp = false;
    }

    public void setLanguage(string value)
    {
        locale = value;
    }


    public void Skip()
    {
        return;
    }

    void resetMeteors()
    {
        gameObject.GetComponent<LevelManager>().resetMeteorMap();
        Meteor[] meteors = FindObjectsOfType<Meteor>();
        foreach (Meteor met in meteors)
        {
            Destroy(met.gameObject);
        }
        gameObject.GetComponent<GameController>().resetGridColor();
        currentMeteors = gameObject.GetComponent<LevelManager>().getMeteorsHit();

    }

    public void setUpSection()
    {
        if (alreadyPlayed)
        {
            userInteraction = false;
            resetMeteors();
        }
        alreadyPlayed = true;
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
        abacus.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        abacus.gameObject.GetComponent<BasicObject>().setDestroyed(false);
        abacus.attachToPreviousCell();
        pointArrow.initiateAnimation(Camera.main.ScreenToWorldPoint(abacus.GetComponent<GridObject>().getParentCell().getScreenPosition()));
        if (locale == "ES")
        {
            tutText.text = "Cada objeto propociona un tipo de información distinto";
        }
        else
        {
            tutText.text = "Each object provides a different kind of information";
        }
    }

    public void loadMeteorMap(MeteorPattern[] newPatternList)
    {
        spawner.setPattern(newPatternList);
    }

    public bool waitForCondition()
    {
        if (telescope.GetComponent<BasicObject>().isDestroyed() && usesSprite.isDestroyed(2))
        {
            usesSprite.SwapTelescope();
        }
        if (abacus.GetComponent<BasicObject>().isDestroyed() && usesSprite.isDestroyed(1))
        {
            usesSprite.SwapAbacus();
        }
        if (gameObject.GetComponent<LevelManager>().getMeteorsHit() - currentMeteors >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool waitForUserInteraction()
    {
        if (playerGrid.getPosition() == abacus.getPosition())
        {
            if (locale == "ES")
                tutText.text = "¡Evita perder todos tus objetos!";
            else
                tutText.text = "Avoid losing all your objects!";
            pointArrow.finishAnimation();
            MeteorShapeGenerator shapeGenerator = new MeteorShapeGenerator();
            MeteorPattern pattern = new MeteorPattern();
            pattern.initialPos = new List<Vector2>() { new Vector2(0.0f, 0.0f), new Vector2(3.0f, 3.0f), telescope.getPosition() };
            pattern.initTime = 0.0f;
            pattern.timeToStart = new List<float>() { 0.0f, 0.0f, 0.0f };
            pattern.timeToFall = new List<float>() { 5.0f, 8.0f, 6.0f };
            pattern.warningTime = 3.0f;
            pattern.endTime = 0.0f;
            pattern.meteorShape = new List<Shape>() { shapeGenerator.generateSquareShape(2), shapeGenerator.generateSquareShape(1), shapeGenerator.generateSquareShape(1) };
            loadMeteorMap(new MeteorPattern[1] { pattern });
            return true;
        }
        else
        {
            return false;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
}
