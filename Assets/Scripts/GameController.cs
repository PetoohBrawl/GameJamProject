using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public DialogWindow dialogWindow;

    [SerializeField]
    private GameDataContainer dataContainer;

    [SerializeField]
    private Character char_1;
    [SerializeField]
    private Character char_2;

    private void Awake()
    {
        Instance = this;
        GameDataStorage.Instance.InitStorage(dataContainer);
    }

    public void StartDialogSequence(DialogSequenceInfo dialogSequence)
    {
        dialogWindow.ShowDialog(dialogSequence);
    }
}
