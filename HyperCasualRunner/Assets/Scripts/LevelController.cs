using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [Header("All Menus in Game")]
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject finishMenu;
    [Header("All Text Objects in Menus")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text finishScoreText;
    [SerializeField] private Text currentLevelText;
    [SerializeField] private Text nextLevelText;
    [SerializeField] private Slider levelProgressBar;
    [SerializeField] private GameObject finishLine;

    public float maxDistance;

    public static LevelController Current;
    [Header("Controller Parameters")]
    public bool gameActive = false;
    private int currentLevel;
    private int score;
    private void Start()
    {


        Current = this;
        currentLevel = PlayerPrefs.GetInt("currentLevel");

        if (SceneManager.GetActiveScene().name != "Level " + currentLevel)
        {
            SceneManager.LoadScene("Level " + currentLevel);
        }
        else
        {
            currentLevelText.text = (currentLevel + 1).ToString();
            nextLevelText.text = (currentLevel + 2).ToString();
        }
    }
    private void Update()
    {
        if (gameActive)
        {
            PlayerController player = PlayerController.CurrentPlayer;
            float distance = finishLine.transform.position.z - PlayerController.CurrentPlayer.transform.position.z;
            levelProgressBar.value = 1 - (distance / maxDistance);
        }
    }
    public void StartLevel()
    {
        maxDistance = finishLine.transform.position.z - PlayerController.CurrentPlayer.transform.position.z;

        PlayerController.CurrentPlayer.ChangeSpeed(PlayerController.CurrentPlayer.runningSpeed);
        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        PlayerController.CurrentPlayer.anim.SetBool("Runing", true);
        gameActive = true;
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene("Level " + (currentLevel + 1).ToString());
    }

    public void GameOver()
    {
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;
    }

    public void FinishGame()
    {
        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);
        finishScoreText.text = score.ToString();
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;
    }
    public void ChangeScore(int increment)
    {
        score += increment;
        scoreText.text = score.ToString();
    }
}
