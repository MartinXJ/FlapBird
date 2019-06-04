using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager GameInstance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    enum PageState {
        None,
        FirstStart,
        GameOver,
        Countdown
    }

    int score = 0;
    bool gameOver = true;

    public bool GameOver { get {return gameOver; } }

    void Awake() {
        GameInstance = this;
    }

    void OnEnable() {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
    }

    void OnDisable() {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
    }

    void OnCountdownFinished() {
        SetPageState(PageState.None);
        OnGameStarted(); // event will be sent to TapController
        score = 0;
        gameOver = false;
    }

    void OnPlayerDied() {
        gameOver = true;
        // needs to be changed to high score of 10 highest score
        int savedScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > savedScore) {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);

    }

    void OnPlayerScored() {
        score += 1;
        scoreText.text = score.ToString();

    }

    void SetPageState(PageState state){
        // Debug.Log("SetPageState is running");
        switch (state) {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.FirstStart:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                // Debug.Log("Inside Countdown is running");
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
        }
    }

    public void ConfirmGameOver () {
        // Activated when replay button is hit
        OnGameOverConfirmed(); // activate this event
        // Event will be sent to TapController

        // reset the score to zero
        scoreText.text = "0";
        SetPageState(PageState.FirstStart);
    }

    public void StartGame (){
        // activated when play button is hit
        SetPageState(PageState.Countdown);

    }

}
