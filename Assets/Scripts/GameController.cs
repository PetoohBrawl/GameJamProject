using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public DialogWindow DialogWindow;

    [SerializeField]
    private GameDataContainer _dataContainer;

    [SerializeField]
    private Character _char_1;
    [SerializeField]
    private Character _char_2;

    private void Awake()
    {
        Instance = this;
        GameDataStorage.Instance.InitStorage(_dataContainer);
    }

    public void StartDialogSequence(DialogSequenceInfo dialogSequence)
    {
        DialogWindow.ShowDialog(dialogSequence);
    }
}
