using Communication;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Robot))]
public class CustomRobotInspector : Editor
{
    private Robot _robot;
    private Vector3 _velocity;
    private Vector3 _rotation;
    private Communication.MessageType _messageType;

    private bool _showSimulateMessage = true;

    private string _robotType = "";
    private string _errorMsg = "";
    private string _customKey = "";
    private string _customData = "";

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
        if (_showSimulateMessage = EditorGUILayout.Foldout(_showSimulateMessage,
            "Simulate Message"))
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
                case Communication.MessageType.Identification:
                    validMessage = true;
                    DrawTextBox("Robot type", ref _robotType);
                    message.identificationResponse = new Communication.Messages.IdentificationResponse()
                    {
                        robotType = _robotType,
                    };
                    break;

                case Communication.MessageType.LogError:
                    validMessage = true;

                    DrawTextBox("Error message", ref _errorMsg);

                    message.error = new Communication.Messages.Error()
                    {
                        message = _errorMsg,
                    };
                    break;

                case Communication.MessageType.CustomEvent:
                    validMessage = true;

                    DrawTextBox("Key", ref _customKey);
                    DrawTextBox("Data", ref _customData);

                    message.customMessage = new Communication.Messages.CustomMessage()
                    {
                        key = _customKey,
                        data = _customData,
                    };
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

    private void DrawTextBox(string label, ref string result)
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label(label);
            result = GUILayout.TextField(result);

        }
        EditorGUILayout.EndHorizontal();
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
