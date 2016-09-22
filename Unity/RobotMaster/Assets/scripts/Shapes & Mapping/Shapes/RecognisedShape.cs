using UnityEngine;
using System.Collections;
using UnityEditorInternal;

public class RecognisedShape : MonoBehaviour
{

    private Vector3 axisRotation;
    private Vector3 position;
    private float maxWidth;
    private float maxHeight;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public RecognisedShape(ShapeData picture)
    {
        //;
    }

    public Vector3 GetAxisRotation()
    {
        return axisRotation;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public float GetMaxWidth()
    {
        return maxWidth;
    }

    public float GetMaxHeight()
    {
        return maxHeight;
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
    }

    public void SetAxisRotation(Vector3 rot)
    {
        axisRotation = rot;
    }
}
