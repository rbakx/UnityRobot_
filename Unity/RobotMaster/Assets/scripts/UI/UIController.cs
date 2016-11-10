using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject MenuButton;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.active);
        if (panel.active)
        {
            MenuButton.GetComponentInChildren<Text>().text = "Close Command Menu";
        }
        else
        {
            MenuButton.GetComponentInChildren<Text>().text = "Open Command Menu";
        }
    }

}
