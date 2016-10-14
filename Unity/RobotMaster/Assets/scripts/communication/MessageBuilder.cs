using Communication.Messages;
using Communication.Transform;
using System;
using System.Collections.Generic;

namespace Communication
{
    public static class MessageBuilder
    {
        /// <summary>
        /// Create a message with a target and type.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <returns>The created message.</returns>
        public static Message CreateMessage(MessageTarget_ target, MessageType_ type)
        {
            Message result = new Message
            {
                messageTarget = target,
                messageType = type,
            };

            return result;
        }

        /// <summary>
        /// Create an identification response message.
        /// </summary>
        /// <param name="robotType_"></param>
        /// <returns>The created message</returns>
        public static IdentificationResponse_ CreateIdentificationResponse(string robotType_)
        {
            IdentificationResponse_ result = new IdentificationResponse_
            {
                robotType = robotType_,
            };

            return result;
        }

        /// <summary>
        /// Create an error message.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns>The created message</returns>
        public static Error_ CreateError(string errorMsg)
        {
            Error_ result = new Error_
            {
                message = errorMsg,
            };

            return result;
        }

        /// <summary>
        /// Create a custom message.
        /// </summary>
        /// <param name="customKey">Message key</param>
        /// <param name="customData">Message data</param>
        /// <returns>The created message</returns>
        public static CustomMessage_ CreateCustomMessage(string customKey, string customData)
        {
            CustomMessage_ result = new CustomMessage_
            {
                key = customKey,
                data = customData,
            };

            return result;
        }

        /// <summary>
        /// Create a set velocity message. If one of the vectors is null, it's velocity won't be changed.
        /// </summary>
        /// <param name="linear">Linear velocity</param>
        /// <param name="angular">Angular velocity</param>
        /// <returns>The created message.</returns>
        public static SetVelocity_ CreateSetVelocity(
            UnityEngine.Vector3? linear, UnityEngine.Vector3? angular)
        {
            SetVelocity_ result = new SetVelocity_();

            result.linearVelocity = CreateVector3(linear);
            result.angularVelocity = CreateVector3(angular);

            return result;
        }

        /// <summary>
        /// Create a Vector3 message.
        /// </summary>
        /// <param name="x_"></param>
        /// <param name="y_"></param>
        /// <param name="z_"></param>
        /// <returns>The created message.</returns>
        public static Vector3_ CreateVector3(float x_, float y_, float z_)
        {
            Vector3_ result = new Vector3_
            {
                x = x_,
                y = y_,
                z = z_,
            };

            return result;
        }

        /// <summary>
        /// Create a Vector3 message.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns>The created message.</returns>
        public static Vector3_ CreateVector3(UnityEngine.Vector3? vec)
        {
            if (vec == null)
            {
                return null;
            }

            return CreateVector3(vec.Value.x, vec.Value.y, vec.Value.z);
        }

        /// <summary>
        /// Create a shape message.
        /// </summary>
        /// <param name="id_">Shape id</param>
        /// <param name="vertices">Vertices that make up the shape</param>
        /// <returns>The created message</returns>
        public static Shape_ CreateShape(Int32 id_, IEnumerable<UnityEngine.Vector3> vertices)
        {
            Shape_ result = new Shape_
            {
                id = id_,
            };

            foreach (var uVertex in vertices)
            {
                result.vertices.Add(CreateVector3(uVertex));
            }

            return result;
        }

        /// <summary>
        /// Create a shapeupdateinfo message.
        /// </summary>
        /// <param name="changedShapes">Collection with changed shapes.</param>
        /// <param name="newShapes">Collection with new shapes.</param>
        /// <returns>The created message.</returns>
        public static ShapeUpdateInfo_ CreateShapeUpdateInfo(IEnumerable<Shape_> changedShapes,
            IEnumerable<Shape_> newShapes)
        {
            ShapeUpdateInfo_ result = new ShapeUpdateInfo_();

            foreach (Shape_ shape in changedShapes)
            {
                result.changedShapes.Add(shape);
            }

            foreach (Shape_ shape in newShapes)
            {
                result.newShapes.Add(shape);
            }

            return result;
        }

        /// <summary>
        /// Set the identification response of a message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="robotType"></param>
        public static void SetIdentificationResponse(this Message message, string robotType)
        {
            if (message == null)
            {
                throw new ArgumentException("message");
            }

            if (robotType == null)
            {
                throw new ArgumentNullException("robotType");
            }

            message.identificationResponse = CreateIdentificationResponse(robotType);
        }

        /// <summary>
        /// Set the logerror of a message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorMsg"></param>
        public static void SetLogError(this Message message, string errorMsg)
        {
            if (message == null)
            {
                throw new ArgumentException("message");
            }

            if (errorMsg == null)
            {
                throw new ArgumentNullException("errorMsg");
            }

            message.error = CreateError(errorMsg);
        }

        /// <summary>
        /// Set the customMessage of a message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void SetCustomMessage(this Message message, string key, string data)
        {
            if (message == null)
            {
                throw new ArgumentException("message");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            message.customMessage = CreateCustomMessage(key, data);
        }

        /// <summary>
        /// Set the velocity of a message. If linear or angular are null, the respective velocity won't be changed.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="linear"></param>
        /// <param name="angular"></param>
        public static void SetVelocity(this Message message,
            UnityEngine.Vector3? linear, UnityEngine.Vector3? angular)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            message.robotVelocity = CreateSetVelocity(linear, angular);
        }

        /// <summary>
        /// Set the shape update info of a message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="changedShapes">Collection with changed shapes.</param>
        /// <param name="newShapes">Collection with new shapes.</param>
        public static void SetShapeUpdateInfo(this Message message,
            IEnumerable<Shape_> changedShapes, IEnumerable<Shape_> newShapes)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            message.shapeUpdateInfo = CreateShapeUpdateInfo(changedShapes, newShapes);
        }
    }
}