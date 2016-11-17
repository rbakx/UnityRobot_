using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject MenuButton;

    public GameObject ManualPanel;
    public GameObject canvas;

    public List<GameObject> panels;

    public bool MenuOpen = false;

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

    public void ClosePanels(List<GameObject> panels)
    {
        foreach (GameObject panel in panels)
        {
            if (panel.activeSelf != false && panel.name != "MovePanel(Clone)")
            {
                panel.SetActive(false);
                MenuButton.GetComponentInChildren<Text>().text = "Open Menu";
                Debug.Log("Heil Satan");
            }
        }
    }

    //TODO: Send cancellation msg to robot
    public void CancelAction()
    {
        
    }

    public void XBtnAction(GameObject thisMenu)
    {
        panels.Remove(thisMenu);
        Destroy(thisMenu,0.1f);
        Debug.Log("Your mother was a hamster anther smelt of elderberries!");

    }
                                               
    public void MenuBtnAction()
    {
        if (MenuOpen)
        {
            ClosePanels(panels);
            CancelAction();
            MenuOpen = false;
        }
        else
        {
            TogglePanel(panels[0]);
            MenuOpen = true;
        }
    }             
                                               
    public void BackBtnAction()                
    {                                          
        ClosePanels(panels);
        TogglePanel(panels[0]);
        MenuOpen = true;
    }

    public void MoveBtnAction()
    {
        //save panel states & close main panels
        bool[] panelStates = new bool[panels.Count];
        for (int i = 0; i < panels.Count; i++)
        {
            if (i < 2)
            {
                panelStates[i] = false;
            }
            else
            {
                panelStates[i] = panels[i].activeSelf;
            }
        }


        //close current panel
        ClosePanels(panels);
        // add popup: choose robot

        //handle chosen robot

        //instantiate the manual robot control menu
        GameObject manualPanel = (GameObject)GameObject.Instantiate(ManualPanel);

        Transform childTransform = manualPanel.transform;
        childTransform.SetParent(canvas.transform,false);

        //add button functions
        addBtnFunctions(manualPanel);

        //return the panels to their original states
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(panelStates[i]);
        }

        panels.Add(manualPanel);
        //TogglePanel(panels[2]);
        MenuOpen = false;
    }


    public void addBtnFunctions(GameObject panel)
    {
        for(int i = 0; i < panel.transform.childCount; i++)
        {
            //check names and add functionality accordingly
            Transform currentChild = panel.transform.GetChild(i);
            if (currentChild.name == "XBtn")
            {
                currentChild.gameObject.GetComponent<Button>().onClick.AddListener(()=> XBtnAction(panel));
            }
            else if(currentChild.name == "MinimizeBtn")
            {
                currentChild.gameObject.GetComponent<Button>().onClick.AddListener(() => MinimizeBtnAction(panel));
            }
        }
    }

    public void AIModeBtnAction()
    {
        //close current panel
        ClosePanels(panels);
        //open the info pop up menu
        TogglePanel(panels[2]);
        MenuOpen = true;
    }

    public void MinimizeBtnAction(GameObject currentMenu)
    {
        Debug.Log(currentMenu.name + ": I am a banana");
    }
}
