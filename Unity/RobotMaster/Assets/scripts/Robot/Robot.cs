using Communication;
using Communication.Messages;
using Networking;
using UnityEngine;

public class Robot : MonoBehaviour, IMessageSender, IMessageReceiver
{
    private string _name;
    private string _type;
    private Vector3 _velocity;
    private Vector3 _rotationVelocity;

    private bool _moving;

    private RecognisedShape _shape;
    private Communicator _communicator;

    void Awake()
    {
        // TODO: Robots should be initialized somewhere else
        ProtoBufPresentation pp = new ProtoBufPresentation();
        LocalDataLink dl = new LocalDataLink();
        Communicator c = new Communicator(dl, pp);

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

    public void Init(Communicator communicator, int id, string name = "", string type = "")
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

        Message moveMessage = new Message
        {
            messageTarget = MessageTarget_.Robot,
            messageType = MessageType_.VelocityChange,
            robotVelocity = new SetVelocity_
            {
                linearVelocity = new Communication.Transform.Vector3_
                {
                    x = linearVelocity.x,
                    y = linearVelocity.y,
                    z = linearVelocity.z,
                },
                angularVelocity = null,
            }
        };

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

        Message stopMessage = new Message
        {
            messageTarget = MessageTarget_.Robot,
            messageType = MessageType_.VelocityChange,
            robotVelocity = new SetVelocity_
            {
                linearVelocity = new Communication.Transform.Vector3_ { x = 0, y = 0, z = 0 },
                angularVelocity = new Communication.Transform.Vector3_ { x = 0, y = 0, z = 0 },
            },
        };

        SendCommand(stopMessage);
    }

    public void SetAngularVelocity(Vector3 angularVelocity)
    {
        _rotationVelocity = angularVelocity;

        Message rotateMessage = new Message
        {
            messageTarget = MessageTarget_.Robot,
            messageType = MessageType_.VelocityChange,
            robotVelocity = new SetVelocity_
            {
                angularVelocity = new Communication.Transform.Vector3_
                {
                    x = angularVelocity.x,
                    y = angularVelocity.y,
                    z = angularVelocity.z,
                },
                linearVelocity = null,
            }
        };

        SendCommand(rotateMessage);
    }

    public void Indicate()
    {
        Message indicateMessage = new Message
        {
            messageTarget = MessageTarget_.Robot,
            messageType = MessageType_.Indicate,
        };

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
            case Communication.MessageType_.IdentificationResponse:
                Debug.Log("Identification for robot " + _name + ": " + newMessage.identificationResponse.robotType);
                break;

            case Communication.MessageType_.LogError:
                Debug.LogError(newMessage.error.message);
                break;

            case Communication.MessageType_.CustomEvent:
                Debug.Log("Custom event for robot " + _name + ": (" +
                    newMessage.customMessage.key + ", " + newMessage.customMessage.data + ")");
                break;
        }
    }
}
