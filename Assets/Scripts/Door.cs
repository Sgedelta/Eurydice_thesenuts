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
        // If entering a combat room, create a new entry in data tracker dictionary
        CheckMonsterRoom();

        // If entering the end room, save data to json file
        CheckEndRoom();

        UIManager.instance.GetComponent<TransitionCanvas>().FadeInOut(_doorScene);
    }

    private void CheckMonsterRoom()
    {
        if (_doorType == RoomType.Monster && !GameManager.instance.DataTracker.ContainsKey(_doorScene))
        {
            GameManager.instance.DataTracker.Add(_doorScene, 0);
        }
    }

    private void CheckEndRoom()
    {
        if (_doorType == RoomType.Ending)
        {
            // Save data to json file
            GameManager.instance.SaveData();
            // Clear the DataTracker dictionary 
            GameManager.instance.DataTracker.Clear();
        }
    }
}
