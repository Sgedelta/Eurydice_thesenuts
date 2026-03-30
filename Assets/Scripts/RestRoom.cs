using UnityEngine;

public class RestRoom : MonoBehaviour
{

    
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
