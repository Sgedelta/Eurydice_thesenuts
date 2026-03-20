using UnityEngine;

public class RestRoom : MonoBehaviour
{

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnRestore()
    {
        GameManager.instance.Orpheus.SetMorale(100.0f);
        DeactivateRestRoomUI();
    }

    public void DeactivateRestRoomUI()
    {
        this.gameObject.SetActive(false);
    }
}
