using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IPointerClickHandler
{
    private enum enDoorType { Monster, Item};
    [SerializeField] private enDoorType _doorType;
    [SerializeField] private string _doorScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(_doorScene);
    }

    private void PrintType()
    {
        Debug.Log("Enter door: " + _doorType.ToString());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PrintType();
        LoadLevel();
    }
}
