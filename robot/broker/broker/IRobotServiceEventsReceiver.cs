using System;

namespace broker
{
    public interface IRobotServiceEventsReceiver
    {
        void OnRobotConnect(GeneralTypeRobot robot);
    }
}
