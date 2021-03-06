﻿using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private ShotSpawnerController player;

    [Header("GUI")]
    [SerializeField]
    private Color scoreColor = Color.red;
    [SerializeField]
    private GameObject startScreen;
    [SerializeField]
    private GameObject ingameGUI;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject lostScreen;
    [SerializeField]
    private Text lostScoreText;
    [SerializeField]
    private GameObject winScreen;
    [SerializeField]
    private Text winLevelText;
    [SerializeField]
    private Text winScoreText;
    [SerializeField]
    private Toggle soundToggle;
    [SerializeField]
    private CameraShake cameraShake;
    [SerializeField]
    [Range(0, 1)]
    private float heathBarReductionSpeed = 1f;
    [SerializeField]
    [Range(0, 1)]
    private float startPulseTime = 0.5f;

    private bool isPaused = false;
    private bool gameWon = false;
    private int score = 0;
    private float anchorDelta = 0.25f;
    private float nextAnchorMin;
    private float nextAnchorMax;
    private int currentLevel = 1;
    private int maxLevelsCount = 2;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("music"))
        {
            PlayerPrefs.SetInt("music", 1);
            soundToggle.isOn = true;
            AudioListener.volume = 1;
            PlayerPrefs.Save();
        }
        else
        {
            if (PlayerPrefs.GetInt("music") == 0)
            {
                soundToggle.isOn = false;
                AudioListener.volume = 0;
            }
            else
            {
                soundToggle.isOn = true;
                AudioListener.volume = 1;
            }
        }

        if (!PlayerPrefs.HasKey("restart"))
        {
            PlayerPrefs.SetInt("restart", 0);
        }
        else
        {
            if (PlayerPrefs.GetInt("restart") == 1)
            {
                startScreen.SetActive(false);
                ingameGUI.SetActive(true);
                player.SetActive();
                PlayerPrefs.SetInt("restart", 0);
            }
        }
    }

    private void Start()
    {
        scoreText.text = score.ToString();
        nextAnchorMax = healthBar.rectTransform.anchorMax.x;
        nextAnchorMin = healthBar.rectTransform.anchorMin.x;
    }

    private void Update()
    {
        healthBar.rectTransform.anchorMin = new Vector2(
            Mathf.Lerp(healthBar.rectTransform.anchorMin.x, nextAnchorMin, heathBarReductionSpeed),
            healthBar.rectTransform.anchorMin.y);

        healthBar.rectTransform.anchorMax = new Vector2(
            Mathf.Lerp(healthBar.rectTransform.anchorMax.x, nextAnchorMax, heathBarReductionSpeed),
            healthBar.rectTransform.anchorMin.y);

        if (startScreen.activeInHierarchy)
        {
            if (Input.GetButton("Fire1"))
            {
                StartCoroutine(StartGame());
            }
        }
    }

    IEnumerator StartGame()
    {
        var animCtrl = startScreen.GetComponentInChildren<Animator>();
        animCtrl.Play("FastPulse");

        yield return new WaitForSeconds(startPulseTime);

        animCtrl.Play("BasicPulse");
        startScreen.SetActive(false);
        ingameGUI.SetActive(true);
        player.SetActive();
    }

    public void Pause()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
            ingameGUI.SetActive(true);
            isPaused = false;
        }
        else
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            ingameGUI.SetActive(false);
            isPaused = true;
        }
    }

    public void ModifyHealthBar(float ratio)
    {
        nextAnchorMax = 0.5f + anchorDelta * ratio;
        nextAnchorMin = 0.5f - anchorDelta * ratio;
    }

    public void IncreaseScore(int add)
    {
        score += add;
        scoreText.text = score.ToString();
    }

    public void ToggleSound()
    {
        if (soundToggle.isOn)
        {
            PlayerPrefs.SetInt("music", 1);
            AudioListener.volume = 1;
        }
        else
        {
            PlayerPrefs.SetInt("music", 0);
            AudioListener.volume = 0;
        }
        PlayerPrefs.Save();
    }

    public void Won()
    {
        gameWon = true;
        winScreen.SetActive(true);
        winScoreText.text = $"SCORE: <color=#{ColorUtility.ToHtmlStringRGB(scoreColor)}>{score}</color>";
        winLevelText.text = $"LEVEL <color=#{ColorUtility.ToHtmlStringRGB(scoreColor)}>{currentLevel}</color>\nCLEARED";
        ingameGUI.SetActive(false);
        player.SetInactive();
    }

    public void Lost()
    {
        if (!gameWon)
        {
            lostScreen.SetActive(true);
            lostScoreText.text = $"SCORE: <color=#{ColorUtility.ToHtmlStringRGB(scoreColor)}>{score}</color>";
            ingameGUI.SetActive(false);
            player.SetInactive();
        }
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("restart", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CameraShake()
    {
        cameraShake.StartShaking();
    }

    public void NextLevel()
    {
        //TODO: real implementation
        MainMenu();
    }
}
