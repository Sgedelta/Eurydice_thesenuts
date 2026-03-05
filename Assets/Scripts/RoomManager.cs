using System;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    [SerializeField] RoomData roomData;

    public void Awake()
    {
        //first, check if the game manager has data at this location

        if (GameManager.instance == null)
        {
            new WaitForEndOfFrame();
        }

        if (roomData == null)
        {
            Debug.LogError("[RM] Room Manager's room Data was null!");
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

    }

}

[Serializable]
public class RoomData
{
    public RoomType Type = RoomType.NONE;

    public bool Completed = false;

    public Vector2Int Position;
}
