﻿using System;
using SimpleJson;
using UnityEditor;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public Rect NodeRect;
    public string Title;

    private bool _isDragged;
    private bool _isSelected;

    public static event Action<Node> OnRemoveNode;
    public static event Action<Node> OnConnectionCreateBegin;
    public static event Action<Node> OnConnectionCreateEnd;

    public abstract JsonObject SerializeToJson();

    public void Drag(Vector2 delta)
    {
        NodeRect.position += delta;
    }

    public abstract void Draw();

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (NodeRect.Contains(e.mousePosition))
                    {
                        _isDragged = true;
                        GUI.changed = true;
                        _isSelected = true;

                        OnConnectionCreateEnd?.Invoke(this);
                    }
                    else
                    {
                        GUI.changed = true;
                        _isSelected = true;
                    }
                }

                if (e.button == 1 && _isSelected && NodeRect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                _isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && _isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.AddItem(new GUIContent("Add Connection"), false, OnClickAddConnection);
        genericMenu.ShowAsContext();
    }

    private void OnClickAddConnection()
    {
        OnConnectionCreateBegin?.Invoke(this);
    }

    private void OnClickRemoveNode()
    {
        OnRemoveNode?.Invoke(this);
    }
}
