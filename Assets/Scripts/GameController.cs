using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static bool GameStarted { get; private set; }

    public BallController ball;
    public PaddleController paddle;
    public BlockGenerator blockGenerator;

    //----------------------------------------------------------------------------------------

    #region SCORE

    [Header("Score")]
    public TMP_Text highScoreText;
    public TMP_Text scoreText;
    private int score;

    //----------------------------------------------------------------------------------------

    public void AddScore(int points)
    {
        score += points;
        scoreText.SetText(score.ToString());
    }

    private void UpdateHighScore()
    {
        if (HighScoreManager.TrySave(score)) highScoreText.SetText("NEW HIGH SCORE");
        else highScoreText.SetText($"HIGH SCORE: {HighScoreManager.Load()}");
    }

    #endregion SCORE

    //----------------------------------------------------------------------------------------

    #region START/STOP

    [Header("Start")]
    public TMP_Text startText;

    private bool waitingToStart;

    //----------------------------------------------------------------------------------------

    private void WaitForStart()
    {
        GameStarted = false;
        waitingToStart = true;
        startText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
    }

    private void StartGame()
    {
        waitingToStart = false;
        GameStarted = true;
        startText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        ball.Launch();
    }

    private void StopGame()
    {
        GameStarted = false;
        waitingToStart = false;

        ball.Reset();
        paddle.Reset();
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void ExitGame() => SceneManager.LoadScene("Menu");

    public void Defeat()
    {
        StopGame();
        UpdateHighScore();
        blockGenerator.ClearBlocks();
        defeatPanel.SetActive(true);
    }

    #endregion START/STOP

    //----------------------------------------------------------------------------------------

    #region LEVEL

    [Header("Level")]
    public GameObject clearedPanel;
    public GameObject defeatPanel;
    private int remainingBlocks;
    private int level = 1;

    //----------------------------------------------------------------------------------------

    private void StartLevel()
    {
        clearedPanel.SetActive(false);
        defeatPanel.SetActive(false);

        ball.Reset();
        paddle.Reset();

        remainingBlocks = blockGenerator.GenerateBlocks(level);
        WaitForStart();
    }

    private void OnBlockDestroyed(int points)
    {
        AddScore(points);

        remainingBlocks--;
        if (remainingBlocks <= 0) ClearLevel();
    }

    private void ClearLevel()
    {
        StopGame();
        clearedPanel.SetActive(true);
    }

    public void NextLevel()
    {
        level++;
        StartLevel();
    }

    #endregion LEVEL

    //----------------------------------------------------------------------------------------

    #region EVENTS

    private void OnEnable() => BlockController.Destroyed += OnBlockDestroyed;
    private void OnDisable() => BlockController.Destroyed -= OnBlockDestroyed;

    private void Start()
    {
        startText.SetText("PRESS ANY KEY TO START");
        scoreText.SetText(score.ToString());
        StartLevel();
    }

    private void Update()
    {
        if (waitingToStart
            && Keyboard.current != null
            && Keyboard.current.anyKey.wasPressedThisFrame
            ) StartGame();
    }

    #endregion EVENTS
}