using UnityEngine;

public class LevelUpUIManager : MonoBehaviour
{
    [SerializeField] private GameObject LevelUpPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayLevelUpPanel()
    {
        LevelUpPanel.SetActive(true);
    }

    public void HideLevelUpPanel()
    {
        LevelUpPanel.SetActive(false);
    }
}
