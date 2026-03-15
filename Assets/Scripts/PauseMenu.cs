using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    private CanvasGroup _pauseMenuCanvasGroup;
    private bool isGamePaused = false;

    private void Awake()
    {
        // If an instance already exists and it's not this, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Otherwise, set this as the instance
        instance = this;

        // Keep the object alive when loading new scenes
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        _pauseMenuCanvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        _pauseMenuCanvasGroup.blocksRaycasts = false;
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (isGamePaused) ResumeGame();
        else PauseGame();
    }

    public void ResumeGame()
    {
        this.gameObject.SetActive(false);
        _pauseMenuCanvasGroup.blocksRaycasts = false;
        Time.timeScale = 1f; // Resume normal game time
        isGamePaused = false;
    }

    public void PauseGame()
    {
        this.gameObject.SetActive(true);
        _pauseMenuCanvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f; // Stop all time-based operations (movement, physics, yap)
        isGamePaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; // Unfreeze time before loading a new scene
        SceneManager.LoadScene("MainMenu");
        Destroy(this.gameObject);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
