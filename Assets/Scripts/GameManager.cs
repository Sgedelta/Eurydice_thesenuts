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
    //First pass of this code is gonna be a mess im so sorry, inventories suck
    //There is absolutely a better way to indicate spots I'm just tired and can't think of it rn
    public void MoveItem(int i)
    {

        //No selected item, store the one in the corresponding array slot
        if (selectedItem == null)
        {
            ////TODO: highlight button?
            RetrieveItem(i);
            selectedItemIndex = i;
            return;
        }

        //Item already being moved, update existing info
        Item previousItem = EquipItem(i);

        //If there's an item passed back, set it to the first item's spot
        if (previousItem != null)
        {
            EquipItem(selectedItemIndex);
        }

        ////Null out selected item and index
        selectedItem = null;
        selectedItemIndex = -1;

        //TODO: update text of buttons -- array of buttons to reference?
        //In that case, may not need to pass in int and can just set listener on startup,,,,getting everything working first though

    }

    //Helper for MoveItem, gets an item from an array based on button index
    private void RetrieveItem(int i)
    {
        //If index is >1, Eurydice array
        if (i > 1)
        {
            i -= 2;
            selectedItem = Eurydice.EquippedItems[i];
        }

        //Orpheus array
        else
        {
            selectedItem = Orpheus.EquippedItems[i];
        }
    }

    //Helper for MoveItem, runs equipItem on determined spot
    private Item EquipItem(int i)
    {
        Item previousItem = null;
        //If index is >1, Eurydice array
        if (i > 1)
        {
            i -= 2;
            previousItem = Eurydice.EquipItem(i, selectedItem);
        }

        //Orpheus array
        else
        {
            previousItem = Orpheus.EquipItem(i, selectedItem);
        }

        //fallback, shouldn't get hit
        return previousItem;
    }
}
