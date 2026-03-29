using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string _firstLevel;
    [SerializeField] private GameObject MainView;
    [SerializeField] private GameObject ControlView;
    [SerializeField] private GameObject IntroView;

    void Start()
    {
    }

    public void StartGame()
    {
        //Resetting these in case it'd cause issues later
        IntroView.SetActive(false);
        MainView.SetActive(true);
        SceneManager.LoadScene(_firstLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleIntro()
    {
        MainView.SetActive(!MainView.activeSelf);
        IntroView.SetActive(!IntroView.activeSelf);
    }

    public void ToggleControls()
    {
        MainView.SetActive(!MainView.activeSelf);
        ControlView.SetActive(!ControlView.activeSelf);
    }
}
