using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI matchCountText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;

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

    public void ShowWinScreen()
    {
        SoundManager.Instance.PlayWinSound();
        winScreen.SetActive(true);
    }
    public void RestartGame()
    {
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
