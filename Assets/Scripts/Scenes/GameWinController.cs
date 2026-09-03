using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWinController : MonoBehaviour
{
    #region SCORE

    [Header("Score")]
    public TMP_Text scoreText;
    public static int score;

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

    private void Start() => scoreText.SetText("SCORE: " + score);

    #endregion EVENTS
}