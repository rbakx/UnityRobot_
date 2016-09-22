using UnityEngine;
using System.Collections;

public class RobotSelect : MonoBehaviour {

    public bool isSelected;
    private bool particleActive;

    public GameObject particle;
    private GameObject particleObject;

    void Start ()
    {
        isSelected = false;
        particleActive = false;
    }
	
	// Update is called once per frame
	void Update () {
	    if (isSelected && !particleActive)
	    {
	        {
	            particleObject = (GameObject)Instantiate(particle, this.transform.position, this.transform.rotation);
	            particleObject.transform.parent = this.gameObject.transform;
	            particleActive = true;
	        }
	    }
	    if (!isSelected)
	    {
	        particleActive = false;
            Destroy(particleObject);
	    }
	}
}
