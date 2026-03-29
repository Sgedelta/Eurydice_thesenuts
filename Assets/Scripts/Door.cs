using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Door : MonoBehaviour, IPointerClickHandler
{
    [Header("Door Type")]
    [SerializeField] private RoomType _doorType;

    [Tooltip("The next room (scene) this door leads to")]
    [Header("Door Load Scene")]
    [SerializeField] private string _doorScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // On click event
    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetComponent<TransitionCanvas>().FadeInOut(_doorScene);
        if (GameManager.instance.LastVisitedRoomManager.RoomData.Type == RoomType.Item) 
        {
            UIManager.instance.EnableInventoryItems();
        }
        else 
        {
            UIManager.instance.DisableInventoryItems();
        }
    }

 
}
