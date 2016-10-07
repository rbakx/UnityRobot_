using UnityEngine;
using UnityEditor;
using System;
using Communication;

[CustomEditor(typeof(Robot))]
public class CustomRobotInspector : Editor
{
    private Robot _robot;
    private Vector3 _velocity;
    private Vector3 _rotation;
    private Communication.MessageType _messageType;

    private bool _showSimulateMessage = true;

    private string _robotType = "";

    void Awake()
    {
        _velocity = Vector3.zero;
        _rotation = Vector3.zero;

        _robot = target as Robot;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        if (Application.isPlaying)
        {
            DrawVector("Velocity", ref _velocity, _robot.SetVelocity);
            DrawVector("Rotation", ref _rotation, _robot.SetRotation);

            if (GUILayout.Button("Indicate"))
            {
                _robot.Indicate();
            }

            EditorGUILayout.Space();

            DrawSendMessageGUI();
        }
    }

    private void DrawSendMessageGUI()
    {
        if (_showSimulateMessage =
            EditorGUILayout.Foldout(_showSimulateMessage, "Simulate Message"))
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Message Type:");
                _messageType = (Communication.MessageType)EditorGUILayout.EnumPopup(_messageType);
            }
            EditorGUILayout.EndHorizontal();

            bool validMessage = false;

            Message message = new Message
            {
                messageTarget = Communication.MessageTarget.Unity,
                messageType = _messageType,
            };


            switch (_messageType)
            {
                case Communication.MessageType.RobotHeartbeat:
                    validMessage = true;
                    break;

                case Communication.MessageType.RobotTypeNotification:
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Robot Type");
                        _robotType = GUILayout.TextField(_robotType);
                        message.robotType = new RobotType();
                        message.robotType.type = _robotType;
                    }
                    EditorGUILayout.EndHorizontal();
                    validMessage = true;
                    break;
            }

            if (validMessage)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Press send to simulate message");
                if (GUILayout.Button("Send"))
                {
                    _robot.IncomingMessage(message, null);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("Unsupported message type");
            }
        }
    }

    private void DrawVector(string name, ref Vector3 vec, Action<Vector3> robotMethod)
    {
        EditorGUILayout.BeginHorizontal();
        {
            Vector3 newVec = EditorGUILayout.Vector3Field(name, vec);

            DrawResetButton(ref newVec);

            if (newVec != vec)
            {
                vec = newVec;
                robotMethod(vec);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawResetButton(ref Vector3 vec)
    {
        if (GUILayout.Button("Reset"))
        {
            vec = Vector3.zero;
        }
    }
}
