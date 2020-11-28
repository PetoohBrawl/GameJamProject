using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private DialogWindow _dialogWindow;

    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _exitGameButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ShowExitGameButton()
    {
        _exitGameButton.gameObject.SetActive(true);
    }

    public void HideStartGameButton()
    {
        _startGameButton.gameObject.SetActive(false);
    }

    public void OnClickExitGame()
    {
        Application.Quit();
    }

    public void OnClickStartGame()
    {
        GameProgressController.Instance.StartGame(0);
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
