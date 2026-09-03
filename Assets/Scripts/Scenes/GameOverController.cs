using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    #region SCORE

    [Header("Score")]
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public static int score;
    public static int highScore;

    #endregion SCORE

    //----------------------------------------------------------------------------------------

    #region SCENES

    [Header("Scenes")]
    public SceneAsset menuScene;
    public SceneAsset gameScene;

    public void GoToMenu() => SceneManager.LoadScene(menuScene.name);
    public void GoToGame() => SceneManager.LoadScene(gameScene.name);

    #endregion SCENES

    //----------------------------------------------------------------------------------------

    #region EVENTS

    private void Start()
    {
        scoreText.SetText("SCORE: " + score);
        highScoreText.SetText("HIGH SCORE: " + highScore);
    }

    #endregion EVENTS
}