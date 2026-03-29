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

        // Do not show the Get Item UI if this item room has already been visited
        if (GameManager.instance.LastVisitedRoomManager.RoomData.Completed)
        {
            gameObject.SetActive(false);
        }

        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(() => {
            if (itemToEquip)
            {
                DontDestroyOnLoad(itemToEquip);
                UIManager.instance.AutoEquip(itemToEquip, this.gameObject);
            }
            gameObject.transform.Find("ItemPanel").Find("ItemGet").gameObject.SetActive(false);
            GameManager.instance.LastVisitedRoomManager.SetCompleted(true);

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideItemCanvas()
    {
        this.gameObject.SetActive(false);
    }
}
