using System;
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
    public List<GameObject> ManualPanels;

    public GameObject MinionControl;
    public Robot SelectedRobot;

    public bool MenuOpen = false;
    public bool robotFound = false;


    private bool[] panelStates;


    GameObject manualPanel;
    Transform childTransform;


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
            if (panel.activeSelf != false && panel.name != "MovePanel(Clone)" && panel != null)
            {
                panel.SetActive(false);
                MenuButton.GetComponentInChildren<Text>().text = "Open Menu";
            }
        }
    }

    private RobotList activeRobots = null;

    //TODO: Send cancellation msg to robot
    public void CancelAction()
    {

    }

    public void XBtnAction(GameObject thisMenu)
    {
        panels.Remove(thisMenu);
        ManualPanels.Remove(thisMenu);
        Destroy(thisMenu, 0.1f);
        Debug.Log("Your mother was a hamster father smelt of elderberries!");

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
        MinionControl.transform.GetComponent<UnitSelecter>().SelectedUnit = null;
        SelectedRobot = null;
        panelStates = null;

        activeRobots = MinionControl.GetComponent<RobotList>();

        //if a robot is available
        if (activeRobots.Count < 1)
        {
            Debug.Log("Not enough robots to open manual command");
            return;
        }
        else
        {
            panelStates = new bool[panels.Count];

            //Save panel states, states will be reloaded after instantiation of new panel
            for (int i = 0; i < panels.Count - 1; i++)
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


            //close currently open panels
            ClosePanels(panels);
            MenuOpen = false;

            if (activeRobots.Count == 1 && !CheckManualPanelExists(activeRobots.Get(0)))
            {
                //instantiate the manual robot control menu
                manualPanel = (GameObject)GameObject.Instantiate(ManualPanel);

                SelectedRobot = activeRobots.Get(0);

                manualPanel.gameObject.transform.Find("RobotName").GetComponent<Text>().text =
                    SelectedRobot.GetComponent<Robot>().GetRobotName();

                childTransform = manualPanel.transform;
                childTransform.SetParent(canvas.transform, false);

                //add button functions
                addBtnFunctions(manualPanel);
                Debug.Log("Selected robot: ");
                Debug.Log(SelectedRobot);
                ManualMoveCommander commander = manualPanel.GetComponent<ManualMoveCommander>();

                commander.SetRobot(SelectedRobot.gameObject);
                Debug.Log("set robot: ");
                Debug.Log(commander.GetRobot());

                //return the panels to their original states
                for (int i = 0; i < panels.Count; i++)
                {
                    panels[i].SetActive(panelStates[i]);
                }

                panels.Add(manualPanel);
                ManualPanels.Add(manualPanel);
                //TogglePanel(panels[2]);


            }

            //if more than one robot is available
            else
            {
                // add popup: choose robot
                Debug.Log("Select your player or RMB to cancel");

                //set selectingRobot flag to true;
                MinionControl.transform.GetComponent<UnitSelecter>().selectingRobot = true;
                
                StartCoroutine(SelectingTarget());
            }

        }
    }

    public bool CheckManualPanelExists(Robot newRobot)
    {
        foreach (GameObject p in ManualPanels)
        {
            if (p.GetComponent<ManualMoveCommander>().GetRobotScript() == newRobot)
            {
                Debug.Log("Yes dis is dog");
                return true;
            }
        }
        Debug.Log("No, This is Patrick");
        return false;
    }


    public IEnumerator SelectingTarget()
    {
        MinionControl.transform.GetComponent<UnitSelecter>().selectingRobot = true;
        MinionControl.transform.GetComponent<UnitSelecter>().RobotSelected = false;
        while (MinionControl.transform.GetComponent<UnitSelecter>().SelectedUnit == null)
        {
            yield return new WaitForSeconds(0.1f);
            if (MinionControl.transform.GetComponent<UnitSelecter>().SelectedUnit != null)
            {

                SelectedRobot = MinionControl.transform.GetComponent<UnitSelecter>().SelectedUnit.GetComponent<Robot>();
                Debug.Log("WHOOHOOO");

                //instantiate the manual robot control menu
                if (!CheckManualPanelExists(SelectedRobot))
                {
                    manualPanel = (GameObject) GameObject.Instantiate(ManualPanel);

                    manualPanel.transform.FindChild("RobotName").GetComponent<Text>().text =
                        SelectedRobot.GetComponent<Robot>().GetRobotName();

                    childTransform = manualPanel.transform;
                    childTransform.SetParent(canvas.transform, false);

                    //add button functions
                    addBtnFunctions(manualPanel);

                    manualPanel.GetComponent<ManualMoveCommander>().SetRobot(SelectedRobot.gameObject);
                    ManualPanels.Add(manualPanel);
                }
            }
        }
    }

    public void addBtnFunctions(GameObject panel)
    {
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            //check names and add functionality accordingly
            Transform currentChild = panel.transform.GetChild(i);
            if (currentChild.name == "XBtn")
            {
                currentChild.gameObject.GetComponent<Button>().onClick.AddListener(() => XBtnAction(panel));
            }
            else if (currentChild.name == "MinimizeBtn")
            {
                currentChild.gameObject.GetComponent<Button>().onClick.AddListener(() => MinimizeBtnAction(panel));
            }
            else if (currentChild.name == "SelectDestinationBtn")
            {
                currentChild.gameObject.GetComponent<Button>().onClick.AddListener(() => panel.GetComponent<ManualMoveCommander>().DestinationBtnPressed());
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

        //Debug.Log(currentMenu.name + ": I am a banana");
        //TODO: set panel inactive, spawn minimized icon

    }
}
