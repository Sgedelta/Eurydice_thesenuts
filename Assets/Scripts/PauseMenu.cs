using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuUI;
    private bool isGamePaused = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused) ResumeGame();
            else PauseGame();
        }
    }

    public void ResumeGame()
    {
        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume normal game time
        isGamePaused = false;
    }

    public void PauseGame()
    {
        _pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Stop all time-based operations (movement, physics, yap)
        isGamePaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; // Unfreeze time before loading a new scene
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
