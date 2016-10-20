using UnityEngine;
using System.Collections;
using Networking;
using Communication;
using System;
using System.Collections.Generic;

public class RobotRegister : MonoBehaviour, IMessageReceiver, IIncomingDataLinkSubscriber
{
    public GameObject robotPrefab;

    public RobotList robotList;
    private ShapesUpdater _shapesUpdater;

    private TCPDataLinkListener<ProtoBufPresentation> listener;

    private List<Communicator> connections;

    public string HostAddress = "127.0.0.1";
    public short HostPort = 1234;
    public bool Host = false;

    private bool wasHosting;

    private int identity;

    void start()
    {
        if(robotPrefab == null)
        {
            Debug.LogError("robotPrefab should be an object with a robot script and a mesh");
        }

        identity = 0;

        wasHosting = false;

        connections = new List<Communicator>();
        listener = new TCPDataLinkListener<ProtoBufPresentation>(this);

        StartCoroutine("hostingCheck");
    }

    IEnumerator hostingCheck()
    {
        while(true)
        {
            if(wasHosting != Host)
            {
                if(Host)
                {
                    if(!listener.Start(HostAddress, HostPort))
                    {
                        Debug.Log("Tried to start host but hosting failed for given arguments HostAddress:HostPort, namely " + HostPort + ":" + HostPort);
                        Host = false;
                    }
                }
                else
                {
                    listener.Stop();
                }

                wasHosting = Host;
            }

            yield return new WaitForSeconds(0.1F);
        }
    }

    public void IncomingMessage(Message newMessage, IDataLink dataLink)
    {
        Communicator connection = GetCommunicatorFromDataLink(dataLink);

        if (connection == null)
            return;

        //identification message and message is accepted
        if(true)
        {

            connections.Remove(connection);

            GameObject robotGameObject = (GameObject)GameObject.Instantiate(robotPrefab, robotPrefab.transform);

            Robot robot = robotGameObject.GetComponent<Robot>();

            ++identity;
            robot.Init(connection, identity, "I, Roboto", "NAO");

            robotList.Add(robot);
        }
            
    }

    public void IncomingNewDataLink(IDataLink dataLink, IPresentationProtocol usedProtocol)
    {
        usedProtocol.SetReceiver(this);
        Communicator communicator = new Communicator(dataLink, usedProtocol);

        connections.Add(communicator);

        Message id_request = MessageBuilder.CreateMessage(MessageTarget_.Robot, MessageType_.IdentificationRequest);

        communicator.SendCommand(id_request);
    }

    Communicator GetCommunicatorFromDataLink(IDataLink dataLink)
    {
        Communicator result = null;

        foreach (Communicator i in connections)
        {
            if(i.GetDataLink() == dataLink)
            {
                result = i;
                break;
            }
        }

            return result;
    }
}
