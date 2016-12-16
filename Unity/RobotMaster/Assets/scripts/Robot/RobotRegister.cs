using UnityEngine;
using System.Collections;
using Networking;
using Communication;
using System;
using System.Collections.Generic;
using System.Threading;
using Communication.Messages;

public class RobotRegister : MonoBehaviour, IMessageReceiver, IIncomingDataLinkSubscriber
{
    public GameObject RobotRefModel_Default;
    public GameObject RobotRefModel_Nao;
    public GameObject RobotRefModel_NXT;

    public GameObject robotObjectContainer;

    public RobotList robotList;
    private ShapesUpdater _shapesUpdater;

    private TCPDataLinkListener<ProtoBufPresentation> listener;

    private List<Communicator> communicators;

    public string HostAddress = "127.0.0.1";
    public short HostPort = 1234;
    public bool Host = false;

    private bool wasHosting;

    private Message _newMessage;
    private Communicator _connection;
    private bool _continueRegistration;

    private ManualResetEvent _ev;

    /*
        robotIdentityID contains the highest unused unique ID of a robot.
        This method ensures the robot will get a unique id.
    */
    static private uint robotIdentityID = 0;

    private bool continueRegistrationRoutine;

    void OnDestroy()
    {
        continueRegistrationRoutine = false;

        if (listener != null)
        {
            StopHosting();

            listener.Dispose();
            listener = null;
        }
    }

    void Awake()
    {
        _ev = new ManualResetEvent(false);
    }

    void Start()
    {
        continueRegistrationRoutine = true;
        wasHosting = false;

        if (RobotRefModel_Default == null || RobotRefModel_Default.GetComponent<Robot>() == null)
        {
            throw new Exception("Default model object must reference to a prefab with robot script!");
        }

        if (RobotRefModel_Nao == null || RobotRefModel_Nao.GetComponent<Robot>() == null)
        {
            throw new Exception("Nao model object reference to a prefab with robot script!");
        }

        if (RobotRefModel_NXT == null || RobotRefModel_NXT.GetComponent<Robot>() == null)
        {
            throw new Exception("NXT model object reference to a prefab with robot script!");
        }

        communicators = new List<Communicator>();
        listener = new TCPDataLinkListener<ProtoBufPresentation>(this);
        _ev = new ManualResetEvent(true);

        StartCoroutine("hostingCheck");
        StartCoroutine("handleRegistrations");
    }

    IEnumerator CheckIfStillConnected()
    {
        while (true)
        {
            for (int i = (communicators.Count - 1); i >= 0; i--)
            {
                Communicator r = communicators[i];

                if (!r.GetDataLink().Connected())
                {
                    communicators.RemoveAt(i);

                    Debug.Log("[communicators]: connection lost");
                }
            }

            yield return new WaitForSeconds(0.5F);
        }
    }

    /*
        coroutine hostingCheck is a very cheap function as it waits most of the time.
        The function reads the status of togglable boolean Host. According to the value, the robot registerer will listen for connections.
        When started, the robotregisterer will start listening on public string value HostAddress and short HostPort.
        If Host is false, the listener is stopped if active.
    */
    private IEnumerator hostingCheck()
    {
        while (true)
        {
            if (wasHosting != Host)
            {
                if (Host)
                {
                    Host = StartHosting(HostAddress, HostPort);
                    if (Host)
                    {
                        Debug.Log("Hosting started on " + HostAddress + ":" + HostPort);
                    }
                }
                else
                {
                    StopHosting();
                    Debug.Log("Hosting stopped");
                }

                wasHosting = Host;
            }

            yield return new WaitForSeconds(0.1F);
        }
    }

    public bool StartHosting(string hostAddress, short port)
    {
        Host = listener.Start(hostAddress, port);

        if (Host)
        {
            HostAddress = hostAddress;
            HostPort = port;
        }
        else
        {
            Debug.LogError("Tried to start host but hosting failed for given arguments HostAddress:HostPort, namely " + hostAddress + ":" + port);
        }

        return Host;
    }

    public void StopHosting()
    {
        listener.Stop();
        Host = false;

        foreach (Communicator i in communicators)
        {
            i.GetDataLink().Dispose();
        }

        communicators.Clear();
    }

    /*
        IncomingMessage is used to receive the information from connections.
        It is expected that the message contains identification information.
        From the identification we'll know whether this connection represents a robot and what type.
    */
    public void IncomingMessage(Message newMessage, IDataLink dataLink)
    {

        // Debug.Log("[RobotRegister] incoming message!");

        /*
            Check if dataLink is a client connected through this listener
        */

        Communicator connection = GetCommunicatorFromDataLink(dataLink);

        if (connection == null)//datalink is not a client from this listener
            return;

        //TODO: is identification message and message is that of a robot
        if (true)
        {
            /*
              First wait for the other identification messages to be processed to prevent race conditions
            */
            if (_ev.WaitOne(10000))
            {
                _ev.Reset();

                _connection = connection;
                _newMessage = newMessage;
                _continueRegistration = true;

                _ev.WaitOne(1000);
            }
        }

    }

    private IEnumerator handleRegistrations()
    {
        while (continueRegistrationRoutine)
        {
            if (_continueRegistration)
            {
                // Debug.Log("Handling new registration request on main thread");

                _continueRegistration = false;

                Communicator connection = _connection;
                Message newMessage = _newMessage;

                _ev.Set();

                //TODO: Replace nao with the variable from the message that indicates the type
                // Debug.Log("newMessage: " + newMessage.identificationResponse);
                string robotType = newMessage.identificationResponse.robotType.ToLower();

                //Get the robot object which contains a Robot component with predefined shape data (this is a reference object)
                GameObject robotPrefab = null;

                switch (robotType)
                {
                    case "nao":
                        {
                            robotPrefab = RobotRefModel_Nao;
                            break;
                        }

                    case "nxt":
                        {
                            robotPrefab = RobotRefModel_NXT;
                            break;
                        }

                    default:
                        {
                            robotType = "default";
                            robotPrefab = RobotRefModel_Default;
                            break;
                        }
                }

                // Clone the reference object
                GameObject robotGameObject = (GameObject)GameObject.Instantiate(robotPrefab, robotPrefab.transform.position, Quaternion.identity, robotObjectContainer.transform);

                // Check if the object actually has the oh-so-important robot component
                Robot robot = robotGameObject.GetComponent<Robot>();

                // An idiot removed the component from the prefab or had gone to another dimension
                if (robot == null)
                {
                    throw new ArgumentNullException("Expected gameObject to contain a robot component: robot_prefab_" + robotType);
                }

                // Reset and (re-)initialise the robot script
                robot.Init(connection, ++robotIdentityID, "Unknown harry", robotType);

                robotList.Add(robot);

                robot.Indicate();

                //Accept connection not as a general client but as a robot
                communicators.Remove(connection);
            }
            else
            {
                yield return new WaitForSeconds(0.01F);
            }
        }

        yield return null;
    }

    /*
        IncomingNewDataLink is used to accept new connections from the listener.
    */
    public void IncomingNewDataLink(IDataLink dataLink, IPresentationProtocol usedProtocol)
    {
        Debug.Log("[RobotRegister] incoming new connection!");

        // Change the receiver of the incoming messages for given client (connection) to this class.
        // The information is used to check the identity of the connection.
        usedProtocol.SetReceiver(this);

        // Communicator is a management class for protocol and datalink
        Communicator communicator = new Communicator(dataLink, usedProtocol);

        // Ask for identification of the connection; might be a robot!
        Message id_request = MessageBuilder.CreateMessage(MessageTarget_.Robot, MessageType_.IdentificationRequest);

        Thread.Sleep(1);

        if (!communicator.SendCommand(id_request))
        {
            Debug.LogError("[RobotRegister] Sending identity request command fails, how could this happen!?");
            // Actually, it could happen if the connection is immediately closed after making it.
        }
        else
        {
            // Keep a list of the active connections
            communicators.Add(communicator);
        }
    }

    private Communicator GetCommunicatorFromDataLink(IDataLink dataLink)
    {
        Communicator result = null;

        foreach (Communicator i in communicators)
        {
            if (i.GetDataLink() == dataLink)
            {
                result = i;
                break;
            }
        }

        return result;
    }

    public void AddDummyBot()
    {
        // Debug.Log("Spoofing prereqs...");

        ProtoBufPresentation pp = new ProtoBufPresentation();
        DummyReceiver dummyReceiver = new DummyReceiver();

        pp.SetReceiver(dummyReceiver);

        DummyDataLink datalink = new DummyDataLink(pp);

        Communicator com = new Communicator(datalink, pp);

        //create message, set response

        MessageTarget_ target = MessageTarget_.Robot;
        MessageType_ type = MessageType_.CustomMessage;

        Message DummyMessage = MessageBuilder.CreateMessage(target, type);
        DummyMessage.SetIdentificationResponse("dummyBot");

        /*
              First wait for the other identification messages to be processed to prevent race conditions
            */
        if (_ev.WaitOne(10000))
        {
            _ev.Reset();

            _connection = com;
            _newMessage = DummyMessage;
            _continueRegistration = true;

            _ev.WaitOne(1000);
        }
    }

}
