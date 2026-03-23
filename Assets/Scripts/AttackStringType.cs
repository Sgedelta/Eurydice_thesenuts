using UnityEngine;

public class AttackStringType : MonoBehaviour
{
    //Literally all this does is hold onto the type of string this is
    public TargetType StringType { get; set; } = TargetType.None;
}
