using System;

namespace broker
{
    public abstract class RobotServiceListener
    {
        protected IRobotServiceEventsReceiver _eventReceiver;

        public RobotServiceListener(IRobotServiceEventsReceiver eventReceiver)
        {
            if (eventReceiver == null)
            {
                throw new ArgumentNullException("eventReceiver");
            }

            _eventReceiver = eventReceiver;
        }

        public abstract string GetServiceName();

        public abstract bool StartListening();

        public abstract void StopListening();

        public abstract bool Connected();

        protected void HandleNewRobot(GeneralTypeRobot newRobot)
        {
            _eventReceiver.OnRobotConnect(newRobot);
        }
    }
}
