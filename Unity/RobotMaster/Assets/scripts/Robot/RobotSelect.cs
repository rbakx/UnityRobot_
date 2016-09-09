using UnityEngine;
using System.Collections;

public class RobotSelect : MonoBehaviour {

    public bool isSelected;
    public bool particleActive;

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
	        Debug.Log("kaasstengel");
	        
	        {
	            particleObject = (GameObject)Instantiate(particle, this.transform.position, this.transform.rotation);
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
