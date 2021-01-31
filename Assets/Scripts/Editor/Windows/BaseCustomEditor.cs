using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BaseCustomEditor : EditorWindow
{
    private void OnDestroy()
    {
        GameDataHelper.TryShowSavaDataDialog(BeforeWriteData);
    }

    protected virtual void BeforeWriteData() { }
}
