using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public struct MeteorPattern
{
    public List<Vector2> initialPos;
    public List<float> timeToFall;
    public List<float> timeToStart;
    public List<Shape> meteorShape;
    public float warningTime;
    public float initTime;
    public float endTime;
}

public class PatternGenerator {

    public MeteorPattern[] getDefaultPattern()
    {
        MeteorPattern[] pattern = new MeteorPattern[0] { };
        return pattern;
    }

    public MeteorPattern[] getRandomPattern()
    {
        int type = (int)Random.Range(0.0f, 2.0f);
        string typeString = "";
        if (type == 0)
            typeString = "ALLRED";
        else
            typeString = "SYNC";
        int list = (int)(Random.Range(0.0f, 3.0f) + 1.0f);
        typeString = typeString + list.ToString();
        Debug.Log("Loading " + typeString);
        return new MeteorPattern[1]{ getPatternFromCSV(GetStreamingAssetsPath()+"/Patterns/" + typeString + ".csv",typeString)};
    }


    string GetStreamingAssetsPath()
    {
     if (Application.platform == RuntimePlatform.Android)
       return "jar:file://" + Application.dataPath + "!/assets";
else
       return Application.streamingAssetsPath;

    }

    MeteorPattern getPatternFromCSV(string path, string typeString)
    {
        MeteorPattern newPattern = new MeteorPattern();
        string[] metaLines = { };
        if (!path.Contains("jar"))
        {
            metaLines = File.ReadAllLines(GetStreamingAssetsPath() + "/Patterns/METADATA.csv");
        }
        else
        {
            WWW request = new WWW(GetStreamingAssetsPath() + "/Patterns/METADATA.csv");
            while (!request.isDone)
            {
               
            }
            metaLines = request.text.Split('\n');           
        }
        foreach (string line in metaLines)
        {
            string[] fields = line.Split(';');
            if (fields[0] == typeString)
            {
                newPattern.warningTime = float.Parse(fields[1]);
                newPattern.initTime = float.Parse(fields[2]);
                newPattern.endTime = float.Parse(fields[3]);
            }
        }
        MeteorShapeGenerator shapeGenerator = new MeteorShapeGenerator();
        string[] lines = { };
        if (!path.Contains("jar"))
        {
            lines = File.ReadAllLines(path);
        }
        else
        {
            WWW request = new WWW(path);
            while (!request.isDone)
            {
                
            }
            lines = request.text.Split('\n');
        }
        newPattern.initialPos = new List<Vector2>();
        newPattern.timeToFall = new List<float>();
        newPattern.timeToStart = new List<float>();
        newPattern.meteorShape = new List<Shape>();
        foreach (string line in lines)
        {
            string[] fields = line.Split(';');
            if (fields[0].Length > 1)
                fields[0] = fields[0].Substring(fields[0].Length - 1);
            newPattern.initialPos.Add(new Vector2(int.Parse(fields[0]), int.Parse(fields[1])));
            newPattern.timeToFall.Add(float.Parse(fields[2]));
            newPattern.timeToStart.Add(0.0f);
            newPattern.meteorShape.Add(shapeGenerator.generateSquareShape(int.Parse(fields[3])));
        }
        return newPattern;
    }

}
