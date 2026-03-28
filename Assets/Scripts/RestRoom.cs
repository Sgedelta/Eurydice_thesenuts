using UnityEngine;

public class RestRoom : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnRestore()
    {
        GameManager.instance.Orpheus.SetMorale(100.0f);
        GameManager.instance.LastVisitedRoomManager.SetCompleted(true);
        DeactivateRestRoomUI();
    }

    public void DeactivateRestRoomUI()
    {
        this.gameObject.SetActive(false);
    }
}
