using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
public class RoomData : ScriptableObject
{
    public RoomType Type = RoomType.NONE;

    public bool Completed = false;

    public Vector2Int Position;
}
