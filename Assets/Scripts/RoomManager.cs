using System;
using System.Collections;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    [SerializeField] RoomData roomData;


    [SerializeField] GameObject[] doorObjects;

    public void OnEnable() //OnEnable runs between Awake and Start... have to do it here so that GameManager can make sure its here.
    {

        //first, check if the game manager and it's data exists
        if (GameManager.instance == null)
        {
            Debug.LogWarning("[RM] Game Manager Instance not Found! waiting one frame...");
            StartCoroutine(WaitOneFrameAndRestart());
            return;
        }
        
        if (roomData == null)
        {
            Debug.LogError("[RM] Room Manager's Room Data Array was null!");
            return;
        }

        //if the data in the GM exists or isn't matching, overwrite our data with that data
        if(GameManager.instance.GetRoomDataAtLoc(roomData.Position.x, roomData.Position.y) != null)
        {
            roomData = GameManager.instance.GetRoomDataAtLoc(roomData.Position.x, roomData.Position.y);
            Debug.Log($"[RM] Overwrote room data at {roomData.Position} with cached room data");
        } else //otherwise overwrite the data with this data (we are setting for the first time)
        {
            Debug.Log($"[RM] Room Manager setting data at {roomData.Position} to its roomData");
            GameManager.instance.SetRoomDataAtLoc(roomData.Position.x, roomData.Position.y, roomData);
        }

        GameManager.instance.LastVisitedRoom = roomData.Position;
        GameManager.instance.LastVisitedRoomManager = this;
    }

    public void Start()
    {
        //roomData is correct now, because we've already run OnEnable
        //now we need to update the state of the game based on that information
        SetDoorsToCompletion();
    }

    public void SetCompleted(bool completed)
    {
        roomData.Completed = completed;
        SetDoorsToCompletion();
    }

    public void SetDoorsToCompletion()
    {
        foreach(GameObject door in doorObjects)
        {
            door.SetActive(roomData.Completed);
        }
    }


    public IEnumerator WaitOneFrameAndRestart()
    {
        yield return null;
        OnEnable();
        Start();
    }
}

[Serializable]
public class RoomData
{
    public RoomType Type = RoomType.NONE;

    public bool Completed = false;

    public Vector2Int Position;
}
