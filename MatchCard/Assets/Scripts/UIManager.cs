using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI matchCountText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private TMP_Dropdown difficultyDropdown;// = 2;

    
    /// <summary>
    /// This Function is called when start the game
    /// </summary>
    public void OnContinueButtonClick()
    {
        GameData savedGameData = SaveLoadSystem.Instance.LoadGame();
        if (savedGameData != null)
        {
           
            CardGameManager.Instance.StartCardGame(true); // Load the saved game
        }
        else
        {
            CardGameManager.Instance.StartCardGame(false);
          
        }
    }
    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateMatchCount(int matchCount)
    {
        matchCountText.text = "Matches: " + matchCount;
    }

    public void ShowGameOverScreen()
    {
        SoundManager.Instance.PlayGameOverSound();
        gameOverScreen.SetActive(true);
    }
    /// <summary>
    /// Called this function on UI drop down.
    /// </summary>
    /// <param name="difficultyDropdown"></param>
    public void OnLevelSelect(TMP_Dropdown difficultyDropdown)
    {
        SaveLoadSystem.Instance.ResetSaveData();
        CardGameManager.Instance.OnDifficultyChange(difficultyDropdown);
    }
    public void ShowWinScreen()
    {
        SoundManager.Instance.PlayWinSound();
        winScreen.SetActive(true);
    }
    public void RestartGame()
    {
        SaveLoadSystem.Instance.ResetSaveData();
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ResetUI()
    {
        scoreText.text = "Score: 0";
        matchCountText.text = "Matches: 0";
        gameOverScreen.SetActive(false);
        winScreen.SetActive(false);
    }

   

}
