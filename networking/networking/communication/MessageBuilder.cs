using Communication.Messages;
using Communication.Transform;
using System;
using System.Collections.Generic;

namespace Communication
{
    public static partial class MessageBuilder
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
        public static IdentificationResponse_ CreateIdentificationResponse_(string robotType_)
        {
            if (robotType_ == null)
            {
                throw new ArgumentNullException("robotType_");
            }

            // NOTE: Allow emtpy robot type?

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
        public static Error_ CreateError_(string errorMsg)
        {
            if (errorMsg == null)
            {
                throw new ArgumentNullException("errorMsg");
            }

            // NOTE: allow empty error message?

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
        public static CustomMessage_ CreateCustomMessage_(string customKey, string customData)
        {
            if (customKey == null)
            {
                throw new ArgumentNullException("customKey");
            }
            if (customKey.Length == 0)
            {
                throw new ArgumentException("customKey is not allow to be an empty string");
            }

            // NOTE: allow null/empty customData?

            CustomMessage_ result = new CustomMessage_
            {
                key = customKey,
            };

            if (!String.IsNullOrEmpty(customData))
            {
                result.data = customData;
            }

            return result;
        }

        /// <summary>
        /// Create a shapeupdateinfo message.
        /// </summary>
        /// <param name="changedShapes">Collection with changed shapes.</param>
        /// <param name="newShapes">Collection with new shapes.</param>
        /// <returns>The created message.</returns>
        public static ShapeUpdateInfo_ CreateShapeUpdateInfo_(ICollection<Shape_> changedShapes,
            ICollection<Shape_> newShapes)
        {
            if (changedShapes == null)
            {
                throw new ArgumentNullException("changedShapes");
            }

            if (newShapes == null)
            {
                throw new ArgumentNullException("newShapes");
            }

            if (changedShapes.Count == 0 && newShapes.Count == 0)
            {
                throw new Exception("Creating a shape update without any changed or new shapes.");
            }

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

            // NOTE: allow null/empty robot type?

            message.identificationResponse = CreateIdentificationResponse_(robotType);
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

            // NOTE: allow null/empty error message?

            message.error = CreateError_(errorMsg);
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

            if (key.Length == 0)
            {
                throw new ArgumentException("Empty keys are not allowed.");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            // NOTE: allow null/empty data?

            message.customMessage = CreateCustomMessage_(key, data);
        }

        /// <summary>
        /// Set the shape update info of a message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="changedShapes">Collection with changed shapes.</param>
        /// <param name="newShapes">Collection with new shapes.</param>
        public static void SetShapeUpdateInfo(this Message message,
            ICollection<Shape_> changedShapes, ICollection<Shape_> newShapes)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (changedShapes == null)
            {
                throw new ArgumentNullException("changedShapes");
            }

            if (newShapes == null)
            {
                throw new ArgumentNullException("newShapes");
            }

            message.shapeUpdateInfo = CreateShapeUpdateInfo_(changedShapes, newShapes);
        }
    }
}