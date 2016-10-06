using UnityEngine;
using System.Collections;
using Networking;
using Communication;
using System;

public class Robot : MonoBehaviour, IMessageSender, IMessageReceiver
{
    private int _id;
    private string _name;
    private string _type;
    private Vector3 _velocity;
    private Vector3 _rotationVelocity;

    private bool _moving;

    private RecognisedShape _shape;
    private Communicator _communicator;

    public int Id { get { return _id; } set { _id = value; } }

    void Update()
    {
        //if (_moving && transform.position != _destination)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, _destination, 2f * Time.deltaTime);
        //    transform.position = transform.position;
        //    Debug.Log("hans is een steen");
        //}

        transform.position += _velocity;
        transform.Rotate(_rotationVelocity);
    }

    public void Init(Communicator communicator, int id, string name = "", string type = "")
    {
        _communicator = communicator;
        _id = id;
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
            messageType = MessageType.SetVelocity,
            robotID = _id,
            robotVelocity = new RobotVelocity
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
            robotID = _id,
        };

        SendCommand(stopMessage);
    }

    public void SetRotation(Vector3 rotation)
    {
        _rotationVelocity = rotation;

        Message rotateMessage = new Message
        {
            messageTarget = MessageTarget.Robot,
            messageType = MessageType.SetRotation,
            robotID = _id,
            robotRotation = new RobotRotation
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
            robotID = _id,
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
            case MessageType.RobotHeartbeat:
                // TODO: Handle heartbeat, or the lack thereof
                break;

            case MessageType.RobotTypeNotification:
                // TODO: Is a robot allowed to change it's type when it already is an 
                // instance of this class (Robot)?
                if (string.IsNullOrEmpty(newMessage.robotType.type) == false)
                {
                    _type = newMessage.robotType.type;
                }
                break;
        }
    }
}
