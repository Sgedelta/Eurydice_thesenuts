using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EquipUIHookerUpper : MonoBehaviour
{
    [SerializeField] private Button closeButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SetupAfterGM());
    }

    public IEnumerator SetupAfterGM()
    {
        while(GameManager.instance == null)
        {
            yield return null;
        }


        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => { GameManager.instance.ToggleInventory(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
