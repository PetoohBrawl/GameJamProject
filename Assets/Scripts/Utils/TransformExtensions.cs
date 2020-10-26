using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// Calls GameObject.Destroy on all children of transform. and immediately detaches the children
    /// from transform so after this call tranform.childCount is zero.
    /// </summary>
    public static void DestroyChildren(this Transform transform) 
    {
        for (int i = transform.childCount - 1; i >= 0; --i) 
        {
            GameObject go = transform.GetChild(i).gameObject;
            Object.Destroy(go);
        }
            
        transform.DetachChildren();
    }
}