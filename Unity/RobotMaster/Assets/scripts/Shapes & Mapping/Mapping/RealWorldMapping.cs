using UnityEngine;
using System.Collections;

public class RealWorldMapping : MonoBehaviour
{
    private RecognisedShapesList ShapesList;
	// Use this for initialization
	void Start () {
	ShapesList = new RecognisedShapesList();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    RealWorldMapping(RecognisedShapesList shapeList)
    {
        this.ShapesList = shapeList;
    }

    RecognisedShape[] GetShapesNearPositionWithinRadius(float radius)
    {
        RecognisedShape[] shapes = null;
        return shapes;
    }
}
