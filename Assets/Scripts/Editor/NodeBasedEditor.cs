using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJson;
using System;

public class NodeBasedEditor : EditorWindow
{
    private Connection _newConnection;
    private List<Connection> _connections = new List<Connection>();

    private List<Node> _nodes = new List<Node>();

    private Vector2 _drag;
    private Vector2 _offset;

    private JsonArray _dialogSequences;
    private JsonArray _dialogStages;
    private JsonArray _dialogChoices;
    private JsonArray _characters;

    private string _currentSequenceName;

    private Vector2 _stageNodeSize = new Vector2(300, 200);
    private Vector2 _choiceNodeSize = new Vector2(300, 300);
    private float _zoomScale = 1f;

    private static NodeBasedEditor _instance;

    private void OnEnable()
    {
        Node.OnRemoveNode += OnClickRemoveNode;
        Node.OnConnectionCreateBegin += OnNewConnectionCreateBegin;
        Node.OnConnectionCreateEnd += OnNewConnectionCreateEnd;
        Connection.OnClickRemoveConnection += OnClickRemoveConnection;
    }

    private void OnDisable()
    {
        Node.OnRemoveNode -= OnClickRemoveNode;
        Node.OnConnectionCreateBegin -= OnNewConnectionCreateBegin;
        Node.OnConnectionCreateEnd -= OnNewConnectionCreateEnd;
        Connection.OnClickRemoveConnection -= OnClickRemoveConnection;
    }

    [MenuItem("Tools/Node Based Editor")]
    private static void OpenWindow()
    {
        _instance = GetWindow<NodeBasedEditor>();
        _instance.titleContent = new GUIContent("Node Based Editor");

        string sequencesData = AssetDatabase.LoadAssetAtPath<TextAsset>(@"Assets/GameData/JSONS/DialogSequences.json").text;
        string stagesData = AssetDatabase.LoadAssetAtPath<TextAsset>(@"Assets/GameData/JSONS/DialogStage.json").text;
        string choicesData = AssetDatabase.LoadAssetAtPath<TextAsset>(@"Assets/GameData/JSONS/DialogChoice.json").text;
        string charactersData = AssetDatabase.LoadAssetAtPath<TextAsset>(@"Assets/GameData/JSONS/Characters.json").text;

        _instance._dialogSequences = SimpleJson.SimpleJson.DeserializeObject<JsonArray>(sequencesData);
        _instance._dialogStages = SimpleJson.SimpleJson.DeserializeObject<JsonArray>(stagesData);
        _instance._dialogChoices = SimpleJson.SimpleJson.DeserializeObject<JsonArray>(choicesData);
        _instance._characters = SimpleJson.SimpleJson.DeserializeObject<JsonArray>(charactersData);

        _instance._currentSequenceName = "Chapter1_Part1_Bar_Fade_in";

        _instance.ParseSequence();
    }

    private void ParseSequence()
    {
        foreach (JsonObject json in _instance._dialogSequences)
        {
            if (json["Name"].Equals(_instance._currentSequenceName))
            {
                string startStageName = (string)json["StartStage"];

                JsonObject startStageJson = GetStageJson(startStageName);

                ParseNextStage(startStageJson, Vector2.zero, null);

                break;
            }
        }
    }

    private void ParseNextStage(JsonObject stageObj, Vector2 lastNodePos, Node parentNode)
    {
        if (stageObj == null)
        {
            return;
        }

        StageNode stageNode = new StageNode(stageObj);

        stageNode.Title = "Stage";
        stageNode.NodeRect = new Rect(lastNodePos, _stageNodeSize);

        _instance._nodes.Add(stageNode);

        if (parentNode != null)
        {
            CreateConnection(parentNode, stageNode);
        }

        string nextStageName = (string)stageObj["NextStageName"];

        if (string.IsNullOrEmpty(nextStageName) == false)
        {
            JsonObject nextStagejson = GetStageJson(nextStageName);

            Vector2 childNodePos = new Vector2(lastNodePos.x + 400, lastNodePos.y);

            ParseNextStage(nextStagejson, childNodePos, stageNode);

            return;
        }

        string choicesString = (string)stageObj["Choices"];

        if (string.IsNullOrEmpty(choicesString) == false)
        {
            string[] choicesNames = choicesString.Split('\n');

            JsonObject[] choicesJsons = new JsonObject[choicesNames.Length];
            
            for (int i = 0; i < choicesJsons.Length; i++)
            {
                choicesJsons[i] = GetChoiceJson(choicesNames[i]);
            }

            ParseChoices(choicesJsons, lastNodePos, stageNode);
        }
    }

    private void ParseChoices(JsonObject[] choiceObjs, Vector2 lastNodePos, Node parentNode)
    {
        foreach (JsonObject choiceObj in choiceObjs)
        {
            if (choiceObj == null)
            {
                continue;
            }

            ChoiceNode choiceNode = new ChoiceNode(choiceObj);

            choiceNode.Title = "Choice";
            choiceNode.NodeRect = new Rect(lastNodePos.x + 400, lastNodePos.y, _choiceNodeSize.x, _choiceNodeSize.y);

            _instance._nodes.Add(choiceNode);

            if (parentNode != null)
            {
                CreateConnection(parentNode, choiceNode);
            }

            string stageName = (string)choiceObj["StageName"];

            if (string.IsNullOrEmpty(stageName))
            {
                continue;
            }

            JsonObject nextStageJson = GetStageJson(stageName);

            ParseNextStage(nextStageJson, Vector2.zero, choiceNode);
        }
    }

    private JsonObject GetStageJson(string name)
    {
        foreach (JsonObject stageJson in _instance._dialogStages)
        {
            if (name.Equals(stageJson["Name"]))
            {
                return stageJson;
            }
        }

        return null;
    }

    private JsonObject GetChoiceJson(string name)
    {
        foreach (JsonObject choiceJson in _instance._dialogChoices)
        {
            if (name.Equals(choiceJson["Name"]))
            {
                return choiceJson;
            }
        }

        return null;
    }

    private void CreateConnection(Node parentNode, Node childNode)
    {
        Connection connection = new Connection(parentNode);
        connection.ChildNode = childNode;

        _connections.Add(connection);
    }

    private void OnGUI()
    {
        ScaleWindow();
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void ScaleWindow()
    {
        Matrix4x4 before = GUI.matrix;
        //Scale my gui matrix
        Matrix4x4 Translation = Matrix4x4.TRS(new Vector3(0, 25, 0), Quaternion.identity, Vector3.one);
        Matrix4x4 Scale = Matrix4x4.Scale(new Vector3(_zoomScale, _zoomScale, _zoomScale));
        GUI.matrix = Translation * Scale * Translation.inverse;
    }

    private void DrawNodes()
    {
        BeginWindows();

        if (_nodes != null)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].NodeRect = GUI.Window(i, _nodes[i].NodeRect, DrawNodeWindow, _nodes[i].Title);
            }
        }

        EndWindows();
    }

    private void DrawNodeWindow(int id)
    {
        _nodes[id].Draw();
    }

    private void DrawConnections()
    {
        if (_connections != null)
        {
            for (int i = 0; i < _connections.Count; i++)
            {
                _connections[i].Draw();
            }
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (_nodes != null)
        {
            for (int i = _nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = _nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void ProcessEvents(Event processEvent)
    {
        _drag = Vector2.zero;

        switch (processEvent.type)
        {
            case EventType.MouseDown:
                if (processEvent.button == 1)
                {
                    ProcessContextMenu(processEvent.mousePosition);
                }
                else if (processEvent.button == 0)
                {
                    _newConnection = null;
                }
                break;

            case EventType.MouseDrag:
                if (processEvent.button == 0)
                {
                    OnDrag(processEvent.delta);
                }
                break;

            case EventType.ScrollWheel:
                {
                    _zoomScale += processEvent.delta.y / 60f;
                }
                break;
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Add Stage Node"), false, () => OnClickAddStageNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Add Choice Node"), false, () => OnClickAddChoiceNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        Node node = new Node();
        node.NodeRect = new Rect(mousePosition.x, mousePosition.y, 200, 300);
        node.Title = "Node";

        _nodes.Add(node);
    }

    private void OnClickAddStageNode(Vector2 mousePosition)
    {
        StageNode stageNode = new StageNode(null);
        stageNode.NodeRect = new Rect(mousePosition.x, mousePosition.y, _stageNodeSize.x, _stageNodeSize.y);
        stageNode.Title = "Stage";

        _nodes.Add(stageNode);
    }

    private void OnClickAddChoiceNode(Vector2 mousePosition)
    {
        ChoiceNode choiceNode = new ChoiceNode(null);
        choiceNode.NodeRect = new Rect(mousePosition.x, mousePosition.y, _choiceNodeSize.x, _choiceNodeSize.y);
        choiceNode.Title = "Choice";

        _nodes.Add(choiceNode);
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        _connections.Remove(connection);
    }

    private void OnClickRemoveNode(Node node)
    {
        if (_connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < _connections.Count; i++)
            {
                if (_connections[i].ParentNode == node || _connections[i].ChildNode == node)
                {
                    connectionsToRemove.Add(_connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                _connections.Remove(connectionsToRemove[i]);
            }
        }

        _nodes.Remove(node);
    }

    private void OnNewConnectionCreateBegin(Node node)
    {
        if (_newConnection != null)
        {
            return;
        }

        _newConnection = new Connection(node);
    }

    private void OnNewConnectionCreateEnd(Node node)
    {
        if (_newConnection == null)
        {
            return;
        }

        _newConnection.ChildNode = node;

        _connections.Add(_newConnection);

        _newConnection = null;
    }

    private void OnDrag(Vector2 delta)
    {
        _drag = delta;

        if (_nodes != null)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void DrawConnectionLine(Event e)
    {
        if (_newConnection != null)
        {
            Vector2 startPoint = new Vector2(_newConnection.ParentNode.NodeRect.xMax, _newConnection.ParentNode.NodeRect.center.y);

            Handles.DrawBezier(
                startPoint,
                e.mousePosition,
                _newConnection.ParentNode.NodeRect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        _offset += _drag * 0.5f;
        Vector3 newOffset = new Vector3(_offset.x % gridSpacing, _offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }
}
