using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private DialogWindow _dialogWindow;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #region CharactersDialog
    public void StartDialogSequence(DialogSequenceInfo dialogSequence)
    {
        _dialogWindow.ShowDialog(dialogSequence);
    }
    #endregion

    #region PlayerDiary

    #endregion

    #region HeroManequen

    #endregion
}
