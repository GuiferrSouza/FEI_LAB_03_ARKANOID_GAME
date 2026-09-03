using System;
using System.Collections;
using TMPro;
using UnityEditor;
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

    #region HELPERS

    private void Wait(float seconds, Action action) => StartCoroutine(WaitCoroutine(seconds, action));
    private IEnumerator WaitCoroutine(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }

    #endregion HELPERS

    //----------------------------------------------------------------------------------------

    #region AUDIO

    public static AudioSource audioSource;

    //----------------------------------------------------------------------------------------

    public static void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    #endregion AUDIO

    //----------------------------------------------------------------------------------------

    #region SCORE

    [Header("Score")]
    public TMP_Text scoreText;
    public int maxScore = 999;
    private int score;

    //----------------------------------------------------------------------------------------

    public void AddScore(int points)
    {
        score = Mathf.Min(score + points, maxScore);
        scoreText.SetText(score.ToString());
    }

    public int UpdateHighScore()
    {
        var hightScore = HighScoreManager.Load();
        if (hightScore < score)
        {
            HighScoreManager.TrySave(score);
            return score;
        }
        return hightScore;
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

    #endregion START/STOP

    //----------------------------------------------------------------------------------------

    #region OVER/WIN

    [Header("Game Over/Win")]
    public SceneAsset gameOverScene;
    public SceneAsset gameWinScene;

    //----------------------------------------------------------------------------------------

    public void GameOver()
    {
        GameOverController.score = score;
        GameOverController.highScore = UpdateHighScore();
        SceneManager.LoadScene(gameOverScene.name);
    }

    public void GameWin()
    {
        GameWinController.score = score;
        SceneManager.LoadScene(gameWinScene.name);
    }

    #endregion OVER/WIN

    //----------------------------------------------------------------------------------------

    #region LEVEL

    [Header("Level")]
    public GameObject clearedPanel;
    private int remainingBlocks;
    private int level = 1;

    //----------------------------------------------------------------------------------------

    private void StartLevel()
    {
        clearedPanel.SetActive(false);

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
        if (score >= maxScore) Wait(2f, GameWin);
        else clearedPanel.SetActive(true);
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
        audioSource = GetComponent<AudioSource>();

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