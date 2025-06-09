using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game States")]
    public bool gameStarted = false;
    private bool isPaused = false;

    [Header("UI Panels")]
    public GameObject platformSpawner;
    public GameObject gameplayUI;
    public GameObject menuUI;         // Home screen UI
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;

    [Header("Score UI")]
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverBestScoreText;

    [Header("Audio Sources & Clips")]
    public AudioSource audioSource;
    public AudioClip buttonClickClip;   // Play this on any button press
    public AudioClip gameOverClip;      // Play this when GameOver() is called
    public AudioClip[] gameMusic;       // [0] = menu music, [1] = gameplay music, [2] = point sound etc.

    private int score = 0;
    private int highScore;
    private Coroutine scoreCoroutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Time.timeScale = 0f;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "Best Score : " + highScore;

        menuUI.SetActive(true);
        gameplayUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    void Update()
    {
        if (!gameStarted && Input.GetMouseButtonDown(0))
        {
            GameStart();
        }
    }

    public void GameStart()
    {
        PlayButtonClick();

        gameStarted = true;
        Time.timeScale = 1f;

        platformSpawner.SetActive(true);
        menuUI.SetActive(false);
        gameplayUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        gameOverUI.SetActive(false);

        audioSource.clip = gameMusic[1]; // gameplay music
        audioSource.Play();

        score = 0;
        scoreTxt.text = score.ToString();
        scoreCoroutine = StartCoroutine(UpdateScore());
    }

    public void PauseGame()
    {
        PlayButtonClick();

        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0f;
            pauseMenuUI.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        PlayButtonClick();

        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
        }
    }

    public void ExitGame()
    {
        PlayButtonClick();
        Debug.Log("ExitGame() called");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void RestartGame()
    {
        PlayButtonClick();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        // Play game over sound first
        PlayGameOverSound();

        gameStarted = false;
        platformSpawner.SetActive(false);

        if (scoreCoroutine != null)
            StopCoroutine(scoreCoroutine);

        SaveHighScore();

        gameOverUI.SetActive(true);
        gameplayUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 0f;

        gameOverScoreText.text = "Score: " + score;
        gameOverBestScoreText.text = "Best: " + PlayerPrefs.GetInt("HighScore", 0);
    }

    public void GoToHome()
    {
        PlayButtonClick();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator UpdateScore()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            score++;
            scoreTxt.text = score.ToString();
        }
    }

    public void IncrementScore()
    {
        score += 2;
        scoreTxt.text = score.ToString();
        audioSource.PlayOneShot(gameMusic[2], 0.2f);
    }

    private void SaveHighScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────
    // Helper methods to play audio clips:

    private void PlayButtonClick()
    {
        if (buttonClickClip != null)
            audioSource.PlayOneShot(buttonClickClip);
    }

    private void PlayGameOverSound()
    {
        if (gameOverClip != null)
            audioSource.PlayOneShot(gameOverClip);
    }
}
