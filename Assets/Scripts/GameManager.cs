using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    //Inventory slots
    //TODO: may adjust this setup later
    [SerializeField] private Button[] inventorySlots = new Button[4];

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
        //Set up listeners on buttons
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            //Seems like passing in i directly is causing issues
            int index = i;
            inventorySlots[i].onClick.AddListener(() => MoveItem(index));
        }
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
            //TODO: highlight button
            selectedItem = RetrieveItem(i);
            selectedItemIndex = i;
            Debug.Log("Selected item: " + selectedItem);
            return;
        }

        //Moving spots

        //Equips selected item, get previous item in slot if applicable
        Item previousItem = EquipItem(selectedItem, i);

        UpdateLabel(i, selectedItem.name);


        //If there's an item passed back, set it to the first item's spot
        if (previousItem != null)
        {
            //Don't really need the return here
            EquipItem(previousItem, selectedItemIndex);
            UpdateLabel(selectedItemIndex, previousItem.name);
        }
        //Empty slot
        else
        {
            UpdateLabel(selectedItemIndex);
        }

        Debug.Log("Eurydice Item 1: " + Eurydice.EquippedItems[0] + "Item 2: " + Eurydice.EquippedItems[1]);
        Debug.Log("Orpheus Item 1: " + Orpheus.EquippedItems[0] + "Item 2: " + Orpheus.EquippedItems[1]);

        //Null out selected item and index
        //tbh index doesn't *need* to be wiped but just in case
        selectedItem = null;
        selectedItemIndex = -1;

        //Reset button highlight

    }

    //Helper for MoveItem, gets an item from an array based on button index
    //TODO: update this to remove some number hardcoding
    private Item RetrieveItem(int i)
    {
        //If index is >1, Eurydice array
        if (i > 1)
        {
            return Eurydice.EquippedItems[i - 2];
        }

        //Orpheus array
        else
        {
            return Orpheus.EquippedItems[i];
        }
    }

    //Helper for MoveItem, runs equipItem on determined spot
    //TODO: update this to remove some number hardcoding
    private Item EquipItem(Item item, int i)
    {
        Item previousItem = null;

        //Removes item from any slots beforehand
        //TODO: update unequip to take an index?
        Eurydice.UnequipItem(item);
        Orpheus.UnequipItem(item);

        //If index is >1, Eurydice array
        if (i > 1)
        {
            previousItem = Eurydice.EquipItem(i - 2, item);
        }

        //Orpheus array
        else
        {
            previousItem = Orpheus.EquipItem(i, item);
        }

        
        return previousItem;
    }

    //Updates label for the inventory slot
    //TODO: I assume this'll probably get replaced with sprites down the line?
    private void UpdateLabel(int i, string name = "Empty")
    {
        inventorySlots[i].GetComponentInChildren<TextMeshProUGUI>().text = name;
    }
}
