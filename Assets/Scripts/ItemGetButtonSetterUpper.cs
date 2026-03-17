using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemGetButtonSetterUpper : MonoBehaviour
{
    [SerializeField] private Button b;
    [SerializeField] private GameObject itemToEquip;


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

        //we have to setup callbacks here because... gamemanager. womp womp.

        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(() => { 
            DontDestroyOnLoad(itemToEquip);
            GameManager.instance.AutoEquip(itemToEquip);
            gameObject.SetActive(false);
            GameManager.instance.LastVisitedRoomManager.SetCompleted(true);

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
