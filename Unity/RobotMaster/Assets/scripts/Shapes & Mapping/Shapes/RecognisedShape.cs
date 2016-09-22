using UnityEngine;
using System.Collections;
using UnityEditorInternal;

[RequireComponent(typeof(MeshFilter))]
public class RecognisedShape : MonoBehaviour
{
    public float maxWidth;
    public float maxHeight;

    public RecognisedShape(ShapeData picture)
    {
        MeshFilter _mr = GetComponent<MeshFilter>();
        if (_mr == null) { throw new MissingComponentException("cannot find mesh renderer");}
        _mr.mesh = picture._mesh;
    }

    public Quaternion GetAxisRotation()
    {
        return this.transform.rotation;
    }

    public Vector3 GetPosition()
    {
        return this.transform.position;
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
        this.transform.position = pos;
    }

    public void SetAxisRotation(Quaternion rot)
    {
        this.transform.rotation = rot;
    }
}
