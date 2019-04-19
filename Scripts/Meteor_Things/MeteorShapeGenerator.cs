using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorShapeGenerator
{
    public Shape generateRandomShape()
    {
        return null;
    }

    public Shape generateSquareShape(int size)
    {
        Shape shape = new Shape(generateSquareForm(size), size);

        return shape;
    }

    #region obsolete

    public Vector2[] generateRandomForm(int randomShape, int randomSize)
    {
        switch(randomShape)
        {
            case 0:
                return generateSquareForm(randomSize);

            case 1:
                return generateLForm(randomSize, 1);

            case 2:
                return generateTriangleForm(randomSize);

            case 3:
                return generateLForm(1, randomSize);

            default:
                return generateSquareForm(0);
        }
    }


    public Vector2 [] generateLForm (int width, int height)
    {
        List<Vector2> values = new List<Vector2>();
        values.Add(new Vector2(0.0f,0.0f));
        for (int i = 0; i<width; i++)
        {
            values.Add(new Vector2((float)i,0.0f));
        }
        for (int j = 0; j<height; j++)
        {
            values.Add(new Vector2(0.0f, (float)j));
        }
        return values.ToArray();
    }

    public Vector2[] generateSquareForm(int size)
    {
        List<Vector2> values = new List<Vector2>();
        if (size == 3) // Para que la casilla central sea el 0,0
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    values.Add(new Vector2((float)i, (float)j));
                }
            }
        }
        else
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    values.Add(new Vector2((float)i, (float)j));
                }
            }
        }
        
        return values.ToArray();
    }

    public Vector2[] generateTriangleForm(int size)
    {
        List<Vector2> values = new List<Vector2>();
        values.Add(new Vector2(0.0f, 0.0f));
        int size2 = size;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size2; j++)
            {
                values.Add(new Vector2((float)i, (float)j));
            }
            size2--;
        }
        return values.ToArray();
    }

    #endregion

}