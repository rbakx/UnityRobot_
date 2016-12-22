using Communication;
using Networking;
using System.Threading;
using UnityEngine;

public class Robot : MonoBehaviour, IMessageSender, IMessageReceiver
{
    public string _name;
    private string _type;
    private Vector3 _destination;
    private Vector3 _velocity;
    private Vector3 _rotationVelocity;

    private bool _moving;

    private float _rotation;
    private Semaphore _rotationSem = null;

    public RecognisedShape shape;
    private Communicator _communicator;

    public bool isVirtual = false;

    public Communicator Communicator { get { return _communicator; } }

    public bool Alive { get; set; } 

    void Awake()
    {
        // TODO: Robots should be initialized somewhere else, initialise dummy objects
        ProtoBufPresentation pp = new ProtoBufPresentation();
        LocalDataLink dl = new LocalDataLink();
        Communicator c = new Communicator(dl, pp);
        shape = null;

        _rotationSem = new Semaphore(0, 1);

        _communicator = c;

        Alive = true;
    }

    void Update()
    {
        if (_moving && transform.position != _destination)
        {
            //rotate towards goal
            transform.Rotate(_rotationVelocity * Time.deltaTime);

            transform.position = Vector3.MoveTowards(transform.position, _destination, 50f * Time.deltaTime);
        }

        if (!Alive)
        {
            Destroy(this.gameObject);
        }

    }
    
    public void SetDestination(Vector3 dest)
    {
        _destination = dest;

        Message message = MessageBuilder.CreateMessage(MessageTarget_.Robot, MessageType_.VelocityChange);

        CalcAngleToDestination();
        //Send RotateCommand

        double distance = CalcDistanceToDestination();
        //Send MoveCommand

        message.SetVelocity(new Vector3((float)distance, 0, 0), null);
        SendCommand(message);
    }

    //Calculates the angle between the robot and its destination
    public double CalcAngleToDestination()
    {
        float myRot = GetRotation();

        if (myRot > 180)
        {
            myRot -= 360;
        }
        if (myRot == -180)
        {
            myRot = 0;
        }

        Debug.Log("Curr Rot: " + myRot);
        double Angle = 0;
        float Q;

        Vector3 dydx = _destination - transform.position;

        float theta = Mathf.Atan2(dydx.z,dydx.x) * Mathf.Rad2Deg;

        //step 1: look for quardrant angle, find rotation till q
        if (_destination.x > transform.position.x && _destination.z > transform.position.z)
        {
            //quadrant 1
         //   Debug.Log("Quadrant 1");
            Q = 90 - theta;
        }
        else if (_destination.x < transform.position.x && _destination.z > transform.position.z)
        {
            //quadrant 2
          //  Debug.Log("Quadrant 2");
            Q = (-90 + theta) *-1;
        }
        else if (_destination.x < transform.position.x && _destination.z < transform.position.z)
        {
            //quadrant 3
           // Debug.Log("Quadrant 3");
            Q = -270 - theta;
            if (Q == -180)
            {
                Q = 180;
            }
        }
        else
        {
            //quadrant 4
          //  Debug.Log("Quadrant 4");
            Q = 90 - theta;
        }

        Debug.Log("Q: " + Q);

        Angle = Q - myRot;
        
        Angle = Angle % 360;
        
        Debug.Log("Angle Before Recalc: " + Angle);
        if (Angle > 180)
        {
            Angle -= 360;
        }
        if (Angle < -180)
        {
            Angle += 360;
        }
        Debug.Log("AngleToDestination: " + Angle);
        return Angle;
    }

    //Calculates the distance between robot and destination
    public double CalcDistanceToDestination()
    {
        double Dist;

        Vector3 distV3 = _destination - transform.position;

        Dist = Mathf.Sqrt(Mathf.Pow(distV3.z,2) + Mathf.Pow(distV3.x,2));

      //  Debug.Log("DistanceToDestination: " + Dist);
        return Dist;
    }

    public Vector3 GetDestination()
    {
        return _destination;
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

    public float GetRotation()
    {

        //if (_communicator.GetDataLink().Connected())
        //{
        //    Message message = MessageBuilder.CreateMessage(MessageTarget_.Robot,
        //        MessageType_.RotationRequest);

        //    SendCommand(message);

        //    _rotationSem.WaitOne();

        //    return _rotation;
        //}

        return transform.eulerAngles.y;
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
                // First identification should be handled in the register,
                // other identifications are ignored for now.
                Debug.Log("(Re?)Identification for robot " + _name + ": " + newMessage.identificationResponse.robotType);
                break;

            case MessageType_.LogError:
                Debug.LogError(newMessage.error.message);
                break;

            case MessageType_.CustomMessage:
                Debug.Log("Custom event for robot " + _name + ": (" +
                    newMessage.customMessage.key + ", " + newMessage.customMessage.data + ")");
                break;

            case MessageType_.Disconnect:
                Debug.Log("Robot disconnecting");
                _communicator.Dispose();
               break;


            case MessageType_.RotationResponse:
                _rotation = newMessage.rotationResponse;
                _rotationSem.Release();
                break;

        }
    }

    public bool IsConnected()
    {
        return _communicator.GetDataLink().Connected();
    }
}
