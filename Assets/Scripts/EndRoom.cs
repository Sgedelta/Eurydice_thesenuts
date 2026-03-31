using UnityEngine;
using UnityEngine.SceneManagement;

public class EndRoom : MonoBehaviour
{
    [SerializeField] private string _roomName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(_roomName);
    }
}
