using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //public static PauseMenu instance;

    [SerializeField] private GameObject _pauseMenuUI;
    private bool isGamePaused = false;

    /*private void Awake()
    {
        // If an instance already exists and it's not this, destroy this one
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Otherwise, set this as the instance
        instance = this;

        // Keep the object alive when loading new scenes
        DontDestroyOnLoad(this.gameObject);
    }*/

    void Start()
    {
        //_pauseMenuUI = _pauseMenuCanvas.transform.Find("PauseMenuUI").gameObject;
        //_pauseMenuUI.GetComponent<CanvasGroup>().alpha = 0f;
    }

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
        //_pauseMenuUI.GetComponent<CanvasGroup>().alpha = 0f;
        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume normal game time
        isGamePaused = false;
    }

    public void PauseGame()
    {
        //_pauseMenuUI.GetComponent<CanvasGroup>().alpha = 1f;
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
