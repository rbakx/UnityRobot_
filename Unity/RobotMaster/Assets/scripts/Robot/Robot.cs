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

    public void SetVelocity(Vector3 velocity)
    {
        _moving = true;
        this._velocity = velocity;

        Message moveMessage = new Message
        {
            messageTarget = MessageTarget.Robot,
            messageType = MessageType.VelocityChange,
            robotVelocity = new SetVelocity
            {
                velocity = new Communication.Transform.Vector3
                {
                    x = velocity.x,
                    y = velocity.y,
                    z = velocity.z,
                }
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
            messageTarget = MessageTarget.Robot,
            messageType = MessageType.StopMoving,
        };

        SendCommand(stopMessage);
    }

    public void SetRotation(Vector3 rotation)
    {
        _rotationVelocity = rotation;

        Message rotateMessage = new Message
        {
            messageTarget = MessageTarget.Robot,
            messageType = MessageType.RotationChange,
            robotRotation = new SetRotation
            {
                rotation = new Communication.Transform.Vector3
                {
                    x = rotation.x,
                    y = rotation.y,
                    z = rotation.z,
                }
            }
        };

        SendCommand(rotateMessage);
    }

    public void Indicate()
    {
        Message indicateMessage = new Message
        {
            messageTarget = MessageTarget.Robot,
            messageType = MessageType.Indicate,
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
        if (newMessage.messageTarget != MessageTarget.Unity)
        {
            Debug.LogError("Robot " + _name + ": received message with a physical robot target!");
            return;
        }

        switch (newMessage.messageType)
        {
            case Communication.MessageType.Identification:
                Debug.Log("Identification for robot " + _name + ": " + newMessage.identificationResponse.robotType);
                break;

            case Communication.MessageType.LogError:
                Debug.LogError(newMessage.error.message);
                break;

            case Communication.MessageType.CustomEvent:
                Debug.Log("Custom event for robot " + _name + ": (" +
                    newMessage.customMessage.key + ", " + newMessage.customMessage.data + ")");
                break;
        }
    }
}
