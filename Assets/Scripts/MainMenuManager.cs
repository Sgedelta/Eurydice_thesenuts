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
        if (UIManager.instance)
        {
            UIManager.instance.gameObject.transform.Find("PauseMenuPanel").gameObject.SetActive(false);
            UIManager.instance.GetComponent<PauseMenu>().IsEnabled = false;
        }
    }

    public void StartGame()
    {
        //Resetting these in case it'd cause issues later
        IntroView.SetActive(false);
        MainView.SetActive(true);

        if (UIManager.instance)
        {
            UIManager.instance.GetComponent<PauseMenu>().IsEnabled = true;
        }
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
