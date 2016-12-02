using Communication;
using Communication.Messages;
using Networking;
using UnityEngine;

public class Robot : MonoBehaviour, IMessageSender, IMessageReceiver
{
    public string _name;
    private string _type;
    private Vector3 _velocity;
    private Vector3 _rotationVelocity;

    private bool _moving;

    public RecognisedShape shape;
    private Communicator _communicator;

    void Awake()
    {
        // TODO: Robots should be initialized somewhere else, initialise dummy objects
        ProtoBufPresentation pp = new ProtoBufPresentation();
        LocalDataLink dl = new LocalDataLink();
        Communicator c = new Communicator(dl, pp);
        shape = null;

        _communicator = c;
    }

    void Update()
    {
        //if (_moving && transform.position != _destination)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, _destination, 2f * Time.deltaTime);
        //    transform.position = transform.position;
        //    Debug.Log("hans is een steen");
        //}

        transform.Rotate(_rotationVelocity * Time.deltaTime);
        transform.Translate(_velocity * Time.deltaTime);
    }

    public void Init(Communicator communicator, uint id, string name = "", string type = "")
    {
        _communicator = communicator;
        _name = name;
        _type = type;

        _velocity = Vector3.zero;
        _rotationVelocity = Vector3.zero;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public void SetType(string type)
    {
        _type = type;
    }

    public string GetRobotName()
    {
        return _name;
    }

    public string GetRobotType()
    {
        return _type;
    }

    public void SetLinearVelocity(Vector3 linearVelocity)
    {
        _moving = true;
        this._velocity = linearVelocity;

        Message moveMessage = MessageBuilder.CreateMessage(MessageTarget_.Robot,
            MessageType_.VelocityChange);

        moveMessage.SetVelocity(linearVelocity, null);

        SendCommand(moveMessage);
    }

    public bool IsMoving()
    {
        return _moving;
    }

    public void StopMoving()
    {
        _moving = false;
        _velocity = Vector3.zero;

        Message stopMessage = MessageBuilder.CreateMessage(MessageTarget_.Robot,
            MessageType_.VelocityChange);

        stopMessage.SetVelocity(Vector3.zero, Vector3.zero);

        SendCommand(stopMessage);
    }

    public void SetAngularVelocity(Vector3 angularVelocity)
    {
        _rotationVelocity = angularVelocity;

        Message rotateMessage = MessageBuilder.CreateMessage(MessageTarget_.Robot,
            MessageType_.VelocityChange);

        rotateMessage.SetVelocity(null, angularVelocity);

        SendCommand(rotateMessage);
    }

    public void Indicate()
    {
        Message indicateMessage = MessageBuilder.CreateMessage(MessageTarget_.Robot,
            MessageType_.Indicate);

        SendCommand(indicateMessage);
    }

    public bool SendCommand(Message message)
    {
        bool result = _communicator.SendCommand(message);

        if (result == false)
        {
            Debug.LogError("Robot " + _name + ": SendCommand failed!");
        }

        return result;
    }

    public void IncomingMessage(Message newMessage, IDataLink dataLink)
    {
        if (newMessage.messageTarget != MessageTarget_.Unity)
        {
            Debug.LogError("Robot " + _name + ": received message with a physical robot target!");
            return;
        }

        switch (newMessage.messageType)
        {
            case MessageType_.IdentificationResponse:
                Debug.Log("Identification for robot " + _name + ": " + newMessage.identificationResponse.robotType);
                break;

            case MessageType_.LogError:
                Debug.LogError(newMessage.error.message);
                break;

            case MessageType_.CustomMessage:
                Debug.Log("Custom event for robot " + _name + ": (" +
                    newMessage.customMessage.key + ", " + newMessage.customMessage.data + ")");
                break;
        }
    }

    public bool IsConnected()
    {
        return _communicator.GetDataLink().Connected();
    }
}
