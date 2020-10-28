using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class GameDataContainer : ScriptableObject
{
    public TextAsset[] GameDataAssets;
    public int MaxHistoryStage;
}
