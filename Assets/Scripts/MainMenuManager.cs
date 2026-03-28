using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string _firstLevel;

    void Start()
    {
    }

    public void StartGame()
    {
        SceneManager.LoadScene(_firstLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
