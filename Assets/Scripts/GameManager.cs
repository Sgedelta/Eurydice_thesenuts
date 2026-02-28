using NUnit.Framework.Interfaces;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //References to UIs to enable/disable
    //TODO: all placeholder UIs currently, this may be removed/adjusted
    [SerializeField] private GameObject EquipUI;
    [SerializeField] private GameObject GeneralUI;

    //Used to temporarily "hold" an item to move between slots
    public Item selectedItem;
    public int selectedItemIndex;

    private void Awake()
    {
        //standard singleton
        if(instance == null)
        {
            instance = this;
        }
        //awake should never be called twice but. just in case!
        else if (instance != this)
        {
            Debug.LogWarning($"Destroying {name} due to the static instance of GameManager already being on {GameManager.instance.name}");
            Destroy(this.gameObject);
        }
    }


    //this will be how Orpheus and Eurydice talk
    public OrpheusController Orpheus;
    public EurydiceController Eurydice;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Toggles inventory UI
    public void ToggleInventory()
    {
        EquipUI.SetActive(!EquipUI.activeSelf);
        GeneralUI.SetActive(!GeneralUI.activeSelf);
    }


    //Called when an inventory button is pressed
    //Either updates the selected item if null, or moves the selected item to the new spot
    //i refers to the slot of the array to check
    public void MoveItem(GameObject button)
    {
        //No selected item, store the one in the corresponding array slot
        if (selectedItem == null)
        {
            ////TODO: highlight button?
            //RetrieveItem(i);
            //selectedItemIndex = i;
            //return;

            //Get the child item of the button that called this
            selectedItem = button.GetComponentInChildren<Item>();
            Debug.Log(selectedItem);
        }

        //Item already being moved, update existing info
        //Move item in selected spot if needed

        

        ////Null out selected item and index
        //selectedItem = null;
        //selectedItemIndex = -1;

    }

    //Helper for MoveItem, gets an item from an array based on button index
    //public void RetrieveItem(int i)
    //{
    //    //If index is >1, Eurydice array
    //    if (i > 1)
    //    {
    //        i -= 2;
    //        selectedItem = Eurydice.EquippedItems[i];
    //    }

    //    //Orpheus array
    //    else
    //    {
    //        selectedItem = Orpheus.EquippedItems[i];
    //    }
    //}
}
