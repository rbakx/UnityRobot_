using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RealWorldMapping
{
    private RecognisedShapesList _shapesList;

    RealWorldMapping(RecognisedShapesList shapeList)
    {
        if (shapeList == null)
        {
            throw new ArgumentNullException("shapeList");
        }

        _shapesList = shapeList;
    }

    RecognisedShape[] GetShapesNearPositionWithinRadius(float radius, Vector3 position)
    {
        List<RecognisedShape> shapes = new List<RecognisedShape>();

        foreach (RecognisedShape shape in _shapesList)
        {
            if (Vector3.Distance(position, shape.Position) <= radius)
            {
                shapes.Add(shape);
            }
        }

        return shapes.ToArray(); ;
    }
}
