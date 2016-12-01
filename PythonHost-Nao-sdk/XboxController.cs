using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicalControllers
{
    class XboxController
    {
        private int _conId;

        public event EventHandler<buttonStateUpdate> OnButtonChange = delegate { };

        public enum UpdateType
        {
            DigitalStateUpdate,
            AnalogStateUpdate
        }

        public class buttonStateUpdate : EventArgs
        {
            public XboxController parent;
            public string buttonName;
            public int buttonValue;
            public UpdateType UpdateType { get; set; }
        }

        public XboxController(int controllerID)
        {
            if(controllerID < 0)
            {
                throw new Exception("[XboxController] controllerId may not be lower than zero!");
            }

            _conId = controllerID;

            Program._controller_register(this);
        }

        ~XboxController()
        {
            Program._controller_unregister(this);
        }

        public int GetId()
         { return _conId; }

        public void UpdateDigitalState(string buttonName, bool value)
        {
            OnButtonChange(this, new buttonStateUpdate { parent = this, buttonName = buttonName, buttonValue = value ? 1 : 0, UpdateType = UpdateType.DigitalStateUpdate });
        }

        public void UpdateAnalogState(string buttonName, int value)
        {
            OnButtonChange(this, new buttonStateUpdate { parent = this, buttonName = buttonName, buttonValue = value, UpdateType = UpdateType.AnalogStateUpdate });
        }
    }
}
