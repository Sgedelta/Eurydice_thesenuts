using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //References to UIs to enable/disable
    //TODO: all placeholder UIs currently, this may be removed/adjusted
    [SerializeField] private GameObject EquipUI;
    [SerializeField] private GameObject InventoryUI;

    //Description management for inventory ui
    private Transform descObject;
    private TextMeshProUGUI descText;

    //Status popup for general ui
    private Transform recieveObject;
    private TextMeshProUGUI recieveText;

    //Used to temporarily "hold" an item to move between slots
    public Item selectedItem;
    public int selectedItemIndex;

    //Inventory slots
    //TODO: may adjust this setup later
    [SerializeField] private Button[] inventorySlots = new Button[4];

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            //pass on our data on (the old gm will need it as it's loading into this scene)
            //UIManager.instance.InventoryUI = InventoryUI;
            //UIManager.instance.EquipUI = EquipUI;
            //UIManager.instance.inventorySlots = inventorySlots;
            //
            //UIManager.instance.SetupInvButtons();

            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
        SetupInvButtons();
    }

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
        InventoryUI.SetActive(!InventoryUI.activeSelf);
    }

    //Called when an inventory button is pressed
    //Either updates the selected item if null, or moves the selected item to the new spot
    //i refers to the slot of the array to check
    //First pass of this code is gonna be a mess im so sorry, inventories suck
    //There is absolutely a better way to indicate spots I'm just tired and can't think of it rn
    public void MoveItem(int i)
    {
        //Gets the description panel
        descObject = EquipUI.transform.Find("ItemDescription");
        descText = descObject.GetComponent<TextMeshProUGUI>();

        Debug.Log("moving");
        //No selected item, store the one in the corresponding array slot
        if (selectedItem == null)
        {
            //TODO: highlight button
            selectedItem = RetrieveItem(i);
            selectedItemIndex = i;
            Debug.Log("Selected item: " + selectedItem);

            //Adds the current item description if there's a selected item
            if (selectedItem != null)
            {
                descObject.gameObject.SetActive(true);
                descText.text = $"Selected Item: \n {selectedItem.name} \n {selectedItem.description}";
            }


            return;
        }

        //Moving spot

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
        //Empty slot (if the player didn't double click the same spot)
        else if (selectedItemIndex != i)
        {
            UpdateLabel(selectedItemIndex);
        }

        Debug.Log("Eurydice Item 1: " + GameManager.instance.Eurydice.EquippedItems[0] + "Item 2: " + GameManager.instance.Eurydice.EquippedItems[1]);
        Debug.Log("Orpheus Item 1: " + GameManager.instance.Orpheus.EquippedItems[0] + "Item 2: " + GameManager.instance.Orpheus.EquippedItems[1]);

        //Null out selected item and index
        //tbh index doesn't *need* to be wiped but just in case
        selectedItem = null;
        selectedItemIndex = -1;

        //Clearing out and disabling item description
        descObject.gameObject.SetActive(false);
        descText.text = "";

        //Reset button highlight

    }

    public void SetupInvButtons()
    {
        //Set up listeners on buttons
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null)
            {
                Debug.LogError($"Inventory Slot {i} Does not exist!");
                continue;
            }
            //Seems like passing in i directly is causing issues
            int index = i;
            inventorySlots[i].onClick.AddListener(() => MoveItem(index));
        }
    }

    //Helper for MoveItem, gets an item from an array based on button index
    //TODO: update this to remove some number hardcoding
    private Item RetrieveItem(int i)
    {
        //If index is >1, Eurydice array
        if (i > 1)
        {
            return GameManager.instance.Eurydice.EquippedItems[i - 2];
        }

        //Orpheus array
        else
        {
            Debug.Log(GameManager.instance.Orpheus);
            return GameManager.instance.Orpheus.EquippedItems[i];
        }
    }

    //Helper for MoveItem, runs equipItem on determined spot
    //TODO: update this to remove some number hardcoding
    private Item EquipItem(Item item, int i)
    {
        Item previousItem = null;

        //Removes item from any slots beforehand
        //TODO: update unequip to take an index?
        GameManager.instance.Eurydice.UnequipItem(item);
        GameManager.instance.Orpheus.UnequipItem(item);

        //If index is >1, Eurydice array
        if (i > 1)
        {
            previousItem = GameManager.instance.Eurydice.EquipItem(i - 2, item);
        }

        //Orpheus array
        else
        {
            previousItem = GameManager.instance.Orpheus.EquipItem(i, item);
        }

        return previousItem;
    }

    //Equips item in first empty slot
    public void AutoEquip(GameObject item, GameObject ItemCanvas)
    {
        //Merging the two arrays for checking purposes
        //Didn't think to do this with earlier stuff, will go back and clean up later

        Item[] fullInventory = GameManager.instance.Orpheus.EquippedItems.Concat(GameManager.instance.Eurydice.EquippedItems).ToArray();

        //Loops through the inventory slots to find the first empty one
        for (int i = 0; i < fullInventory.Length; i++)
        {
            Debug.Log(fullInventory[i]);
            //Empty spot found
            if (fullInventory[i] == null)
            {
                Debug.Log(i);
                //Place item in the corresponding inventory, need to access og arrays
                if (i <= 1)
                {
                    GameManager.instance.Orpheus.EquipItem(i, item.GetComponent<Item>());
                }
                else
                {
                    GameManager.instance.Eurydice.EquipItem(i - 2, item.GetComponent<Item>());
                }

                //Makes sure general ui is on
                if (!InventoryUI.activeSelf)
                {
                    ToggleInventory();
                }

                //Enables popup
                recieveObject = ItemCanvas.transform.Find("ItemPanel").Find("ItemReceive");

                recieveText = recieveObject.Find("ItemReceiveText").gameObject.GetComponent<TextMeshProUGUI>();
                recieveObject.gameObject.SetActive(true);

                string itemName = item.GetComponent<Item>().name;
                UpdateLabel(i, itemName);
                recieveText.text = $"You received a {itemName}";

                //End the loop here
                break;
            }
        }
    }

    //Updates label for the inventory slot
    //TODO: I assume this'll probably get replaced with sprites down the line?
    private void UpdateLabel(int i, string name = "Empty")
    {
        inventorySlots[i].GetComponentInChildren<TextMeshProUGUI>().text = name;
    }

    public void ResetInventoryText()
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            UpdateLabel(i);
        }
    }
    
    // Enables clicking of inventory items
    public void EnableInventoryItems()
    {
        EquipUI.transform.Find("OItem1").gameObject.GetComponent<Button>().enabled = true;
        EquipUI.transform.Find("OItem2").gameObject.GetComponent<Button>().enabled = true;
        EquipUI.transform.Find("EItem1").gameObject.GetComponent<Button>().enabled = true;
        EquipUI.transform.Find("EItem1").gameObject.GetComponent<Button>().enabled = true;
    }

    // Disables clicking of inventory items
    public void DisableInventoryItems()
    {
        EquipUI.transform.Find("OItem1").gameObject.GetComponent<Button>().enabled = false;
        EquipUI.transform.Find("OItem2").gameObject.GetComponent<Button>().enabled = false;
        EquipUI.transform.Find("EItem1").gameObject.GetComponent<Button>().enabled = false;
        EquipUI.transform.Find("EItem1").gameObject.GetComponent<Button>().enabled = false;
    }
}
