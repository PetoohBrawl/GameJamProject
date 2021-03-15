using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJson;
using System;

public class NodeBasedEditor : BaseCustomEditor
{
    private Connection _newConnection;
    private readonly List<Connection> _connections = new List<Connection>();

    private readonly List<Node> _nodes = new List<Node>();

    private Vector2 _drag;
    private Vector2 _offset;

    private readonly Vector2 _stageNodeSize = new Vector2(300, 200);
    private readonly Vector2 _choiceNodeSize = new Vector2(300, 300);

    private static NodeBasedEditor _instance;

    private string _sequenceName;
    private string _finalStageName;

    private readonly List<string> _parsedNodes = new List<string>();

    private List<string> _characterNames;

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

    public static void OpenWindow(string startStageName, string finalStageName, string sequenceName, List<string> characterNames)
    {
        _instance = GetWindow<NodeBasedEditor>();
        _instance.titleContent = new GUIContent("Node Based Editor");

        _instance._sequenceName = sequenceName;
        _instance._finalStageName = finalStageName;

        _instance._characterNames = characterNames;

        if (string.IsNullOrEmpty(startStageName))
        {
            return;
        }
        
        JsonObject startStageJson = _instance.GetStageJson(startStageName);

        _instance.ParseNextStage(startStageJson, Vector2.zero, null);
    }

    #region Parsing

    private void ParseNextStage(JsonObject stageObj, Vector2 lastNodePos, Node parentNode)
    {
        if (stageObj == null)
        {
            return;
        }

        string stageName = (string) stageObj["Name"];

        if (_parsedNodes.Contains(stageName))
        {
            Node searchNode = _nodes.Find(delegate(Node node)
            {
                if (node is StageNode searchStageNode)
                {
                    if (searchStageNode.StageName.Equals(stageName))
                    {
                        searchStageNode.NodeRect.y = (searchStageNode.NodeRect.y + parentNode.NodeRect.y) / 2; 
                        return true;
                    }
                }
                
                return false;
            });

            if (searchNode == null)
            {
                Debug.LogError("Error with double stageNode connection");
                return;
            }
            
            CreateConnection(parentNode, searchNode);
            return;
        }

        StageNode stageNode = ScriptableObject.CreateInstance<StageNode>();
        stageNode.Init(stageObj);
        
        _parsedNodes.Add(stageNode.StageName);

        if (_finalStageName != null && stageNode.StageName.Equals(_finalStageName))
        {
            stageNode.IsFinal = true;
            _finalStageName = null;
        }

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

        JsonArray choicesNamesArray = stageObj.Get<JsonArray>("Choices");

        if (choicesNamesArray == null || choicesNamesArray.Count == 0)
        {
            return;
        }
        
        JsonObject[] choicesJsons = new JsonObject[choicesNamesArray.Count];
        
        for (int i = 0; i < choicesJsons.Length; i++)
        {
            choicesJsons[i] = GetChoiceJson(choicesNamesArray.GetAt<string>(i));
        }

        ParseChoices(choicesJsons, lastNodePos, stageNode);
    }

    private void ParseChoices(JsonObject[] choiceObjs, Vector2 lastNodePos, Node parentNode)
    {
        int choicesCount = choiceObjs.Length;
        float choiceNodeYPlace = _choiceNodeSize.y + 200;
        float topNodePosY = (choicesCount - 1) * choiceNodeYPlace / 2;

        for (int i = 0; i < choicesCount; i++)
        {
            JsonObject choiceObj = choiceObjs[i];

            if (choiceObj == null)
            {
                continue;
            }

            ChoiceNode choiceNode = ScriptableObject.CreateInstance<ChoiceNode>();
            choiceNode.Init(choiceObj, _characterNames);

            choiceNode.Title = "Choice";

            Vector2 choicePosition = new Vector2(lastNodePos.x + 400, lastNodePos.y - (topNodePosY - i * choiceNodeYPlace));
            choiceNode.NodeRect = new Rect(choicePosition.x, choicePosition.y, _choiceNodeSize.x, _choiceNodeSize.y);

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

            Vector2 stagePos = new Vector2(choicePosition.x + 400, choicePosition.y);
            ParseNextStage(nextStageJson, stagePos, choiceNode);
        }
    }

    private JsonObject GetStageJson(string stageName)
    {
        foreach (JsonObject stageJson in GameDataHelper._stagesData)
        {
            if (stageName.Equals(stageJson["Name"]))
            {
                return stageJson;
            }
        }

        return null;
    }

    private JsonObject GetChoiceJson(string choiceName)
    {
        foreach (JsonObject choiceJson in GameDataHelper._choicesData)
        {
            if (choiceName.Equals(choiceJson["Name"]))
            {
                return choiceJson;
            }
        }

        return null;
    }
    #endregion

    #region Drawing

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Save Data", GUILayout.Width(100), GUILayout.Height(60)))
        {
            OnClickSaveData();
        }
        
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            Repaint();
        }
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

    private void DrawConnectionLine(Event e)
    {
        if (_newConnection != null)
        {
            Vector2 startPoint = new Vector2(_newConnection.ParentNode.NodeRect.xMax, _newConnection.ParentNode.NodeRect.center.y);

            Handles.DrawLine(startPoint, e.mousePosition);

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
    #endregion

    #region Processing
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
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Stage Node"), false, () => OnClickAddStageNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Add Choice Node"), false, () => OnClickAddChoiceNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnClickAddStageNode(Vector2 mousePosition)
    {
        StageNode stageNode = ScriptableObject.CreateInstance<StageNode>();
        stageNode.Init(null);
        
        stageNode.NodeRect = new Rect(mousePosition.x, mousePosition.y, _stageNodeSize.x, _stageNodeSize.y);
        stageNode.Title = "Stage";

        _nodes.Add(stageNode);
        
        GameDataHelper.SetDirty();
    }

    private void OnClickAddChoiceNode(Vector2 mousePosition)
    {
        ChoiceNode choiceNode = ScriptableObject.CreateInstance<ChoiceNode>();
        choiceNode.Init(null, _characterNames);
        
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

    private void CreateConnection(Node parentNode, Node childNode)
    {
        Connection connection = new Connection(parentNode);
        connection.ChildNode = childNode;

        _connections.Add(connection);
    }

    protected override void BeforeWriteData()
    {
        base.BeforeWriteData();
        
        OnClickSaveData();
    }

    public void OnClickSaveData()
    {
        foreach (Node node in _nodes)
        {
            if (node is StageNode)
            {
                StageNode stageNode = (StageNode)node;

                JsonObject newStageData = stageNode.SerializeToJson();

                GameDataHelper.RemoveStage((string)newStageData["Name"]);
                GameDataHelper.AddStage(newStageData);
            }
            else if (node is ChoiceNode)
            {
                JsonObject newChoiceData = ((ChoiceNode)node).SerializeToJson();
                
                GameDataHelper.RemoveChoice((string)newChoiceData["Name"]);
                GameDataHelper.AddChoice(newChoiceData);
            }
        }

        foreach (Connection connection in _connections)
        {
            if (connection.ChildNode == null || connection.ParentNode == null)
            {
                Debug.LogError("Error in connection parsing. One node is NULL");
                continue;
            }

            if (connection.ParentNode is StageNode parentStage)
            {
                if (connection.ChildNode is StageNode childStage)
                {
                    childStage.ParentNode = parentStage;

                    foreach (JsonObject stageJson in GameDataHelper._stagesData)
                    {
                        if (((string)stageJson["Name"]).Equals(parentStage.StageName))
                        {
                            stageJson["NextStageName"] = childStage.StageName;
                            break;
                        }
                    }
                }
                else if (connection.ChildNode is ChoiceNode childChoice)
                {
                    foreach (JsonObject stageJson in GameDataHelper._stagesData)
                    {
                        if (((string)stageJson["Name"]).Equals(parentStage.StageName))
                        {
                            if (stageJson["Choices"] == null)
                            {
                                stageJson["Choices"] = new JsonArray();
                            }

                            ((JsonArray)stageJson["Choices"]).Add(childChoice.ChoiceName);

                            break;
                        }
                    }
                }
            }
            else if (connection.ParentNode is ChoiceNode parentChoice)
            {
                StageNode childStage = connection.ChildNode as StageNode;

                childStage.ParentNode = parentChoice;

                foreach (JsonObject choiceJson in GameDataHelper._choicesData)
                {
                    if (((string)choiceJson["Name"]).Equals(parentChoice.ChoiceName))
                    {
                        choiceJson["StageName"] = childStage.StageName;
                        break;
                    }
                }
            }
        }

        foreach (JsonObject sequenceObject in GameDataHelper._sequencesData)
        {
            if (!_sequenceName.Equals((string) sequenceObject["Name"]))
            {
                continue;
            }
            
            foreach (var node in _nodes)
            {
                if (node is ChoiceNode)
                {
                    continue;
                }

                StageNode stageNode = (StageNode)node;

                if (stageNode.ParentNode != null)
                {
                    continue;
                }
                
                sequenceObject["StartStage"] = stageNode.StageName;
                break;
            }
                
            sequenceObject["FinalStage"] = null;

            foreach (Node node in _nodes)
            {
                if (node is StageNode stageNode)
                {
                    if (stageNode.IsFinal)
                    {
                        sequenceObject["FinalStage"] = stageNode.StageName;
                        break;
                    }
                }
            }
            break;
        }
        
        GameDataHelper.SaveData();
    }
    #endregion
}
