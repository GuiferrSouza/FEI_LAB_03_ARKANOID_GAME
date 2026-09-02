using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    #region HIGH SCORE

    [Header("High Score")]
    public TMP_Text highScoreText;

    //----------------------------------------------------------------------------------------

    private void LoadHighScore()
    {
        highScoreText.SetText($"HIGH SCORE: {HighScoreManager.Load()}");
    }

    #endregion HIGH SCORE

    //----------------------------------------------------------------------------------------

    #region GAME

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    #endregion GAME

    //----------------------------------------------------------------------------------------

    #region EVENTS

    private void Start()
    {
        LoadHighScore();
    }

    #endregion EVENTS
}