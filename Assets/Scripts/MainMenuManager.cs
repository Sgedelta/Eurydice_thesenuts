using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string _firstLevel;

    void Start()
    {
        //if(PauseMenu.instance)
        //{
        //    PauseMenu.instance.gameObject.SetActive(false);
        //    PauseMenu.instance.IsEnabled = false;
        //}
    }

    public void StartGame()
    {
        //PauseMenu.instance.IsEnabled = true;
        SceneManager.LoadScene(_firstLevel);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
