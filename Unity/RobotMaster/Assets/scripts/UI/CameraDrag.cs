using UnityEngine;
using System.Collections;

public class CameraDrag : MonoBehaviour
{
    private bool rotatable;

    public GameObject staticCam;
    public GameObject dragCam;

    public float DragSpeed = 4;
    private Vector3 dragOrigin;

    public bool CameraDragging = true;

    public float OuterLeft = -10f;
    public float OuterRight = 10f;
    
    void Start ()
	{
	    rotatable = true;
        staticCam.GetComponent<Camera>().orthographic = true;
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
	    if (rotatable)
	    {
            Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            float left = Screen.width * 0.2f;
            float right = Screen.width - (Screen.width * 0.2f);

            if (mousePosition.x < left)
            {
                CameraDragging = true;
            }
            else if (mousePosition.x > right)
            {
                CameraDragging = true;
            }

            if (CameraDragging)
            {

                if (Input.GetMouseButtonDown(1))
                {
                    dragOrigin = Input.mousePosition;
                    return;
                }

                if (!Input.GetMouseButton(1)) return;

                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                Vector3 move = new Vector3(0, pos.x * DragSpeed, 0);

                if (move.x > 0f)
                {
                    if (this.transform.position.x < OuterRight)
                    {
                        transform.Rotate(move, Space.World);
                    }
                }
                else
                {
                    if (this.transform.position.x > OuterLeft)
                    {
                        transform.Rotate(move, Space.World);
                    }
                }
            }
        }
	}

    public void setRotatable(bool rot)
    {
        rotatable = rot;
    }

    public void SwitchView()
    {
        if (rotatable)
        {
            staticCam.SetActive(true);
            dragCam.SetActive(false);
            rotatable = false;
        }
        else
        {
            dragCam.SetActive(true);
            staticCam.SetActive(false);
            rotatable = true;
        }
    }
}
