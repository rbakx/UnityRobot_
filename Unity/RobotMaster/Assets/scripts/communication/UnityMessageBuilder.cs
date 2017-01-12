using Communication.Messages;
using Communication.Transform;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Communication
{
    public static class UnityMessageBuilder
    {
        /// <summary>
        /// Create a set velocity message. If one of the vectors is null, it's velocity won't be changed.
        /// </summary>
        /// <param name="linear">Linear velocity</param>
        /// <param name="angular">Angular velocity</param>
        /// <returns>The created message.</returns>
        public static SetVelocity_ CreateSetVelocity_(
            Vector3? linear, Vector3? angular)
        {
            SetVelocity_ result = new SetVelocity_();

            result.linearVelocity = CreateVector(linear);
            result.angularVelocity = CreateVector(angular);

            return result;
        }

        /// <summary>
        /// Create a Vector3 message.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns>The created message.</returns>
        public static Vector3_ CreateVector(Vector3? vec)
        {
            if (vec == null)
            {
                return null;
            }

            return MessageBuilder.CreateVector(vec.Value.x, vec.Value.y, vec.Value.z);
        }

        public static Quaternion_ CreateQuaternion_(Quaternion quat)
        {
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

            result.position = CreateVector(transform.position);
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
        public static Shape_ CreateShape_(Int32 id_, ICollection<Vector3> vertices,
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
                result.vertices.Add(UnityMessageBuilder.CreateVector(uVertex));
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
        /// Set the velocity of a message. If linear or angular are null, the respective velocity won't be changed.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="linear"></param>
        /// <param name="angular"></param>
        public static void SetVelocity(this Message message,
            Vector3? linear, Vector3? angular)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            message.robotVelocity = CreateSetVelocity_(linear, angular);
        }

        /// <summary>
        /// Transforms a proto vector into a unityengine vector
        /// </summary>
        /// <param name="vec"></param>
        /// <returns>Unity vector</returns>
        public static Vector3 ToUnityVector(this Vector3_ vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }
    }
}