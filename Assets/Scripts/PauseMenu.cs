using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    [SerializeField] private GameObject _pauseMenu;
    private bool isGamePaused = false;
    //public bool IsEnabled { get; set; } = true;

    private void Awake()
    {
        // If an instance already exists and it's not this, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(_pauseMenuCanvasGroup.gameObject);
            return;
        }

        // Otherwise, set this as the instance
        instance = this;

        // Keep the object alive when loading new scenes
        DontDestroyOnLoad(_pauseMenuCanvasGroup.gameObject);
        _pauseMenuCanvasGroup.gameObject.SetActive(false);
    }

    void Start()
    {
        _pauseMenuCanvasGroup.blocksRaycasts = false;
        _pauseMenuCanvasGroup.gameObject.SetActive(false);
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
            _pauseMenuCanvasGroup.gameObject.SetActive(false);
            _pauseMenuCanvasGroup.blocksRaycasts = false;
            Time.timeScale = 1f; // Resume normal game time
            isGamePaused = false;
        //}
    }

    public void PauseGame()
    {
        //if (IsEnabled)
        //{
            _pauseMenuCanvasGroup.gameObject.SetActive(true);
            _pauseMenuCanvasGroup.blocksRaycasts = true;
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
