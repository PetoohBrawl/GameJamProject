using System;
using UnityEditor;
using UnityEngine;

public class Connection
{
    public readonly Node ParentNode;
    public Node ChildNode;

    public static event Action<Connection> OnClickRemoveConnection;

    public Connection(Node parentNode)
    {
        ParentNode = parentNode;
    }

    public void Draw()
    {
        Vector2 startPoint = new Vector2(ParentNode.NodeRect.xMax, ParentNode.NodeRect.center.y);
        Vector2 endPoint = new Vector2(ChildNode.NodeRect.xMin, ChildNode.NodeRect.center.y);

        Handles.DrawLine(startPoint, endPoint);

        if (Handles.Button((ParentNode.NodeRect.center + ChildNode.NodeRect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            OnClickRemoveConnection?.Invoke(this);
        }
    }
}
