using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroySection : MonoBehaviour, ITutorialSection
{

    public string locale = "ES";

    bool setUp = false;
    bool enabledSection = false;
    bool userInteraction = false;
    bool secondUserInteraction = false;

    bool finishingFade = false;

    public ArrowMovPointer pointArrow;
    public GridObject playerGrid;
    public GridObject abacus;
    public GridObject telescope;
    public GridObject repair;

    public GameObject recoverButton;

    public GameObject focusPanel;

    public Text tutText;

    public MeteorSpawner spawner;


    float initialHolePtg = 0.0f;
    public void enableSection()
    {
        enabledSection = true;
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

    }

    public void setUpSection()
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
            }
            element.setParentCell(null);
        }

        gameObject.GetComponent<GameController>().setInitialObjects(true);

        foreach (GridObject element in objects)
        {
            if (element.gameObject.GetComponent<BasicObject>() != null)
            {
                element.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                element.gameObject.GetComponent<BasicObject>().setDestroyed(true);
                element.setParentCell(null);
            }
        }

        focusPanel.SetActive(true);

        telescope.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        telescope.gameObject.GetComponent<BasicObject>().setDestroyed(false);
        telescope.attachToPreviousCell();
        abacus.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        abacus.gameObject.GetComponent<BasicObject>().setDestroyed(false);
        abacus.attachToPreviousCell();

        focusPanel.GetComponent<FocusPanel>().InitializeFocus(telescope.GetComponent<GridObject>().getParentCell().getScreenPosition());

        //pointArrow.initiateAnimation(Camera.main.ScreenToWorldPoint(telescope.GetComponent<GridObject>().getParentCell().getScreenPosition()));
        if (locale == "ES")
            tutText.text = "Algunos meteoritos dañan el suelo. ¡Ten cuidado donde pisas!";
        else
            tutText.text = "Some meteors can destroy the floor. Mind your step!";
        initialHolePtg = gameObject.GetComponent<GameController>().holePercentageChance;
        gameObject.GetComponent<GameController>().setHolePercentage(1.1f);

        MeteorShapeGenerator shapeGenerator = new MeteorShapeGenerator();
        MeteorPattern pattern = new MeteorPattern();
        pattern.initialPos = new List<Vector2>() { telescope.getPosition() , new Vector2(4.0f,0.0f), new Vector2(5.0f,1.0f), new Vector2(4.0f, 1.0f) };
        pattern.initTime = 0.0f;
        pattern.timeToStart = new List<float>() { 0.0f, 0.0f, 0.0f, 0.0f};
        pattern.timeToFall = new List<float>() { 6.0f, 4.0f, 4.0f, 4.0f};
        pattern.warningTime = 1.0f;
        pattern.endTime = 0.0f;
        pattern.meteorShape = new List<Shape>() { shapeGenerator.generateSquareShape(1), shapeGenerator.generateSquareShape(1), shapeGenerator.generateSquareShape(1), shapeGenerator.generateSquareShape(1) };
        loadMeteorMap(new MeteorPattern[1] { pattern });
    }

    public void loadMeteorMap(MeteorPattern[] newPatternList)
    {
        spawner.setPattern(newPatternList);
    }

    public bool waitForUserSecondInteraction()
    {
        if (playerGrid.getPosition() == repair.getPosition() && repair.gameObject.GetComponent<RepairObject>().isActive())
        {
            gameObject.GetComponent<GameController>().setHolePercentage(initialHolePtg);
            pointArrow.finishAnimation();
            if (locale == "ES")
                tutText.text = "Usa magia para recuperar objetos (max 5 usos)";
            else
                tutText.text = "Use magic to recover your objects (max 5 uses)";
            pointArrow.setHorizontal();
            pointArrow.initiateAnimation( recoverButton.transform.position);
            return true;
        }
        return false;
    }

    public bool waitForUserInteraction()
    {
        if (!telescope.GetComponent<BasicObject>().isDestroyed())
        {
            return false;
        }
        else
        {
            if (!finishingFade)
            {
                finishingFade = true;
                focusPanel.GetComponent<FocusPanel>().FinishFocus();
                return false;
            }
            else
            {
                if (!focusPanel.GetComponent<FocusPanel>().isFinished())
                {
                    return false;
                }
                focusPanel.SetActive(false);
                if (locale == "ES")
                    tutText.text = "Otros objetos permiten reparar el suelo... ¡Ten cuidado donde pisas!";
                else
                    tutText.text = "Other objects can be used to repair the floor... Mind your step!";
                pointArrow.initiateAnimation(Camera.main.ScreenToWorldPoint(gameObject.GetComponent<GameController>().getScreenPositionFromGrid(repair.getPosition())));
                repair.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                repair.gameObject.GetComponent<BasicObject>().setDestroyed(false);
                repair.attachToPreviousCell();
                return true;
            }
        }
    }

    public bool waitForCondition()
    {
        if (telescope.GetComponent<BasicObject>().isDestroyed())
            return false;
        else
        {
            resetMeteors();
            pointArrow.finishAnimation();
            return true;
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
                if (!userInteraction || !finishingFade)
                {
                    userInteraction = waitForUserInteraction();
                }
                else
                {
                    if (!secondUserInteraction)
                    {
                        secondUserInteraction = waitForUserSecondInteraction();
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
}
