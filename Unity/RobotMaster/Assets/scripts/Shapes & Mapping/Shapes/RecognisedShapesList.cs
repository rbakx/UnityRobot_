using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecognisedShapesList : MonoBehaviour
{
    public List<RecognisedShape> Shapelist; 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

   public RecognisedShapesList()
    {
        Shapelist = new List<RecognisedShape>();
    }

   public bool AddShape(RecognisedShape newShape)
    {
        return false;
    }

    public bool RemoveShape(RecognisedShape Shape)
    {
        return false;
    }

    public RecognisedShape GetShape(int index)
    {
        return Shapelist[index];
    }
}
