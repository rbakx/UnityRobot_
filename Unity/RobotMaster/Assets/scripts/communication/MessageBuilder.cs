using Communication.Messages;
using Communication.Transform;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        /// Create a set velocity message. If one of the vectors is null, it's velocity won't be changed.
        /// </summary>
        /// <param name="linear">Linear velocity</param>
        /// <param name="angular">Angular velocity</param>
        /// <returns>The created message.</returns>
        public static SetVelocity_ CreateSetVelocity_(
            UnityEngine.Vector3? linear, UnityEngine.Vector3? angular)
        {
            SetVelocity_ result = new SetVelocity_();

            result.linearVelocity = CreateVector3_(linear);
            result.angularVelocity = CreateVector3_(angular);

            return result;
        }

        /// <summary>
        /// Create a Vector3 message.
        /// </summary>
        /// <param name="x_"></param>
        /// <param name="y_"></param>
        /// <param name="z_"></param>
        /// <returns>The created message.</returns>
        public static Vector3_ CreateVector3_(float x_, float y_, float z_)
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
        public static Vector3_ CreateVector3_(UnityEngine.Vector3? vec)
        {
            if (vec == null)
            {
                return null;
            }

            return CreateVector3_(vec.Value.x, vec.Value.y, vec.Value.z);
        }

        public static Quaternion_ CreateQuaternion_(UnityEngine.Quaternion quat)
        {
            if (quat == null)
            {
                throw new ArgumentNullException("quat");
            }

            Quaternion_ result = new Quaternion_
            {
                x = quat.x,
                y = quat.y,
                z = quat.z,
                w = quat.w,
            };

            return result;
        }

        public static Transform_ CreateTransform_(UnityEngine.Transform transform)
        {
            if (transform == null)
            {
                throw new ArgumentNullException("transform");
            }

            Transform_ result = new Transform_();

            result.position = CreateVector3_(transform.position);
            result.rotation = CreateQuaternion_(transform.rotation);

            return result;
        }


        /// <summary>
        /// Create a shape message.
        /// </summary>
        /// <param name="id_">Shape id</param>
        /// <param name="vertices">Vertices that make up the shape</param>
        /// <param name="indices">Indices indicating in which order to draw the vertices</param>
        /// <returns>The created shape</returns>
        public static Shape_ CreateShape_(Int32 id_, ICollection<UnityEngine.Vector3> vertices,
            ICollection<UInt32> indices, UnityEngine.Transform transform = null)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException("vertices");
            }

            if (indices == null)
            {
                throw new ArgumentNullException("indices");
            }

            if (vertices.Count < 3)
            {
                Debug.LogWarning("Creating a shape with less than 3 vertices.");
            }

            if (indices.Count < 3)
            {
                Debug.LogWarning("Creating a shape with less than 3 indices.");
            }

            Shape_ result = new Shape_
            {
                id = id_,
            };

            foreach (var uVertex in vertices)
            {
                result.vertices.Add(CreateVector3_(uVertex));
            }

            foreach (UInt32 index in indices)
            {
                result.indices.Add(index);
            }

            if (transform != null)
            {
                result.transform = CreateTransform_(transform);
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
                Debug.LogWarning("Creating a shape update without any changed or new shapes.");
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

            message.robotVelocity = CreateSetVelocity_(linear, angular);
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

        /// <summary>
        /// Transforms a proto vector into a unityengine vector
        /// </summary>
        /// <param name="vec"></param>
        /// <returns>Unity vector</returns>
        public static UnityEngine.Vector3 ToUnityVector(this Vector3_ vec)
        {
            return new UnityEngine.Vector3(vec.x, vec.y, vec.z);
        }
    }
}