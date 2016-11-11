using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject MenuButton;

    public GameObject[] panels;

    public bool MenuOpen = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
        if (panel.activeSelf)
        {
            MenuButton.GetComponentInChildren<Text>().text = "Close Menu";
        }
        else
        {
            MenuButton.GetComponentInChildren<Text>().text = "Open Menu";
        }
    }

    public void ClosePanels(GameObject[] panels)
    {
        foreach (GameObject panel in panels)
        {
            if(panel.activeSelf != false)
            panel.SetActive(false);
            MenuButton.GetComponentInChildren<Text>().text = "Open Menu";
        }
    }

    public void MenuBtnAction()
    {
        if (MenuOpen)
        {
            ClosePanels(panels);
            MenuOpen = false;
        }
        else
        {
            TogglePanel(panels[0]);
            MenuOpen = true;
        }
    }

    public void MoveBtnAction()
    {
        //close current panel
        ClosePanels(panels);
        //open the move pop up menu
        TogglePanel(panels[2]);
    }
}
