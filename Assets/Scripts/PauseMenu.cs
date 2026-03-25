using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuPanel;
    private bool isGamePaused = false;
    //public bool IsEnabled { get; set; } = true;

    void Start()
    {
        _pauseMenuPanel = this.gameObject.transform.Find("PauseMenuPanel").gameObject;
        _pauseMenuPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        //if (IsEnabled)
        //{
            if (isGamePaused) ResumeGame();
            else PauseGame();
        //}
    }

    public void ResumeGame()
    {
        //if (IsEnabled)
        //{
            _pauseMenuPanel.gameObject.SetActive(false);
            Time.timeScale = 1f; // Resume normal game time
            isGamePaused = false;
        //}
    }

    public void PauseGame()
    {
        //if (IsEnabled)
        //{
            _pauseMenuPanel.gameObject.SetActive(true);
            Time.timeScale = 0f; // Stop all time-based operations (movement, physics, yap)
            isGamePaused = true;
        //}
    }

    public void LoadMenu()
    {
        //if(IsEnabled)
        //{
            Time.timeScale = 1f; // Unfreeze time before loading a new scene
            SceneManager.LoadScene("MainMenu");
        //}
    }

    public void QuitGame()
    {
        //if (IsEnabled)
        //{
            Application.Quit();
        //}
    }
}
