using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : InventoryMenu, IShop {


    


    public InventorySlot InventorySlotPrefab;

    private List<ShopInventory.SellableCollectible> CollectiblesInShop;
    private List<ShopInventory.SellableHat> HatsToSell;

    public UnityEngine.UI.Button CloseButton;

    


    private List<InventorySlot> m_ShopSlots;

   
    public Image CollectibleArea = null;
    public Image HatArea = null;

    private ItemViewer m_itemViewer;



    private ShopKeeper m_myVendor;

    private PlayerCreature m_playerInstance;

    public ShopDisplayOption ShopOption = ShopDisplayOption.Hats;

    public List<InventorySlot> HatSlotList;
  


    protected override void Start()
    {
        base.Start(); // Base start call required for all menu classes.
        m_playerInstance = PlayerBrain.s_MasterBrainRef.p_PlayerRef;
        
    }

    protected override void Awake()
    {
        
        m_ShopSlots = new List<InventorySlot>();
        HatsToSell = new List<ShopInventory.SellableHat>();
        CollectiblesInShop = new List<ShopInventory.SellableCollectible>(); // Init item list. Sellable collectible is a wrapper struct hat includes an object and a price.


        HatSlotList = new List<InventorySlot>();

        if (CloseButton)
        {
            CloseButton.onClick.AddListener(Close); // Setup button listeners.
        }

    

       if(!m_itemViewer)
        {
            m_itemViewer = GetComponentInChildren<ItemViewer>();
        }
        

        


    }

    public override void Update()
    {

        if (IsOpen)
        {
            if (Input.GetButtonDown("Dash")) // Check if we press the dash button. (Left or right bumper)
            {
                ++ShopOption; // Increment the shop option.

                if (ShopOption >= ShopDisplayOption.Max)
                {
                    ShopOption = 0;
                }
                
                UpdateSlots(ShopOption); // Update out slots based on the option.
            }
        }
        {
            // Return if the shop isn't open.
            return;
        }


        
        
        
    }

    public void UpdateShop()
    {


        m_myVendor = GetActiveShopKeeper(); // Function to find an active shop keeper.
     
        if (m_myVendor)
        {

            foreach (ShopInventory.SellableCollectible ID in m_myVendor.SI_ShopInventory.CollectiblesToSell)
            {
                CollectiblesInShop.Add(ID); // Loop through the shop keepers inventory and add it to the respective lists.
            }


            foreach (ShopInventory.SellableHat ID in m_myVendor.SI_ShopInventory.Hats)
            {
                HatsToSell.Add(ID);
            }

            UpdateSlots(ShopOption); // Update the slots with the newly filled items.
        }
    }

    private ShopKeeper GetActiveShopKeeper()
    {

       // ShopKeeper holds a static list to keep track of active shop keepers in the scene.

        foreach (ShopKeeper sk in ShopKeeper.s_List_ShopKeepers)
        {
            if (sk != null && sk.gameObject.activeInHierarchy == true) // If it isn't null and its active in the hierachy.
            {
            
                if (sk.B_IsShopOpen == true) // Check if the shop is opened, meaning someone has interacted with it.
                {
                    
                    return sk; // Return it.
                    
                
                }
            }
        }
        return null;

        //Observer based event system made it so we couldn't know the sender of events. This was my solution.
    }

    private bool UpdateSlots(ShopDisplayOption option)
    {

        ClearSlots(); // Clear the old slots.
        EnableDifferentArea(option); // Enable a new portion of the UI to include a different image mask.


        switch (option) // Check the display option.
        {
           
            case ShopDisplayOption.Hats:
 
                
                foreach (ShopInventory.SellableHat _Hat in HatsToSell )
                {
                    if (!m_myVendor.LIST_ID_SoldItems.Contains(_Hat.HatToSell.p_ID_ItemDescription))
                    {

                        InventorySlot HatSlot = CreateSlot(InventorySlotPrefab, Slot.SlotType.Hat); // Generate a slot based on the type and the prefab.
                        HatHolder Holder = HatSlot.GetComponent<HatHolder>(); // Component which purposes is to keep an object. HatHolder inherits from Item holder which is a template Monobehaviour class.
                        Holder.SetObject(_Hat.HatToSell); // Store the object in the holder.
                        HatSlot.ID_ItemInSlot = Holder.GetObject().p_ID_ItemDescription;
                        
                        if (!HatSlot.GetComponent<Sellable>())
                        {
                            Sellable sellable =  HatSlot.gameObject.AddComponent<Sellable>(); // store component at the start
                            sellable.I_Price = _Hat.price;
                            sellable.priceText.text = _Hat.price.ToString(); // Set the price and price text.
                        }
                        else
                        {
                            HatSlot.GetComponent<Sellable>().I_Price = _Hat.price;
                            HatSlot.GetComponent<Sellable>().priceText.text = _Hat.price.ToString();
                        }
                        if (HatSlot.ItemInSlot == null)
                        {
                            HatSlot.ItemInSlot = HatSlot.ID_ItemInSlot;
                        }


                        if (HatSlotList.Count <= HatsToSell.Count)
                        {
                            m_ShopSlots.Add(HatSlot);
                            HatSlotList.Add(HatSlot);
                        }
                        
                     
                    }
                }

                break;

            case ShopDisplayOption.Collectibles:
                foreach (ShopInventory.SellableCollectible SC in CollectiblesInShop) // Same concept as the hat except instead of working with a hat prefab, its just a scriptable object description.
                {

                    if (!m_myVendor.LIST_ID_SoldItems.Contains(SC.ItemToSell))
                    {


                        InventorySlot CollectibleSlot = CreateSlot(InventorySlotPrefab, Slot.SlotType.Collectible);
                        CollectibleSlot.ID_ItemInSlot = SC.ItemToSell;
                        if (!CollectibleSlot.GetComponent<Sellable>())
                        {
                            Sellable sellable = CollectibleSlot.gameObject.AddComponent<Sellable>();
                            sellable.I_Price = SC.price;
                            sellable.priceText.text = SC.price.ToString();
                        }
                        else
                        {
                            CollectibleSlot.GetComponent<Sellable>().I_Price = SC.price;
                            CollectibleSlot.GetComponent<Sellable>().priceText.text = SC.price.ToString();

                        }
                        if (!CollectibleSlot.ItemInSlot)
                        {
                            CollectibleSlot.ItemInSlot = CollectibleSlot.ID_ItemInSlot; // Store the sellable component
                        }

                
                       
                        m_ShopSlots.Add(CollectibleSlot);
                        return true;
                    }
                }
                break;
        }

        return false;
    }

    private void EnableDifferentArea(ShopDisplayOption s)
    {
        // Check the option.
        switch(s)
        {
            case ShopDisplayOption.Collectibles:

                CollectibleArea.gameObject.SetActive(true); // Enable our collectible area
                HatArea.gameObject.SetActive(false); // Disable hat area.
                break;
            case ShopDisplayOption.Hats:
                CollectibleArea.gameObject.SetActive(false); // Vice versa.
                HatArea.gameObject.SetActive(true);
                break;
        }
    }

    private void ClearSlots()
    {
        m_ShopSlots.Clear(); // Clears our slot list.
        foreach (InventorySlot IS in HatArea.GetComponentsInChildren<InventorySlot>(true))
        {
            Destroy(IS.gameObject); // Destroy the hat slots.
        }
        foreach (InventorySlot IS in CollectibleArea.GetComponentsInChildren<InventorySlot>(true)) // true because we want to destroy disabled objects.
        {
            Destroy(IS.gameObject); // destroy the collectible slots.
        }

        HatSlotList.Clear();

        
    }

    private InventorySlot CreateSlot(InventorySlot prefab, Slot.SlotType type)
    {
        InventorySlot ReturningSlot = null;
        switch(type)
        {
            case Slot.SlotType.Collectible:
                InventorySlot CollectibleShopSlot = Instantiate(InventorySlotPrefab); // Instantiate the slot prefab
                CollectibleShopSlot.ST_SlotType = Slot.SlotType.Collectible; // Set its type.
                CollectibleShopSlot.B_IsFilled = true; // Set it to filled.
            
                if (CollectibleArea)
                {
                    CollectibleShopSlot.gameObject.transform.SetParent(CollectibleArea.transform, false); // Set the appropriate transform area.
                }
                CollectibleShopSlot.name = "Collectible Shop Slot ";
                ReturningSlot = CollectibleShopSlot;
                break;
            case Slot.SlotType.Hat:
                InventorySlot HatShopSlot = Instantiate(InventorySlotPrefab);               
                HatShopSlot.ST_SlotType = Slot.SlotType.Hat;
                HatShopSlot.gameObject.AddComponent<HatHolder>();
                HatShopSlot.B_IsFilled = true;
               
                if (HatArea)
                {
                    HatShopSlot.gameObject.transform.SetParent(HatArea.transform, false);
                }
                HatShopSlot.name = "Hat Shop Slot ";
                ReturningSlot = HatShopSlot;
                break;


        }
        return ReturningSlot;
    }

    public void Open()
    {
      // Wrapper for the push function.
        if (IsOpen == false )
        {
            base.Push();
            IsOpen = true;
            UpdateShop();
            PlayerBrain.s_MasterBrainRef.PauseGame();
        }
    }

  public void Close()
    {
        if (IsOpen == true)
        {
            if (m_myVendor)
            {
                m_myVendor.CloseShop();
                EmptyShop();
                m_myVendor = null;
                HatsToSell.Clear();
                CollectiblesInShop.Clear();
            }
            Pop();
            PlayerBrain.s_MasterBrainRef.UnPauseGame();
            IsOpen = false;
        }
       
    }



    public void EmptyShop()
    {
        ClearSlots();
        foreach(InventorySlot s in m_ShopSlots)
        {
            Destroy(s.gameObject);
        }
        m_ShopSlots.Clear();
        HatSlotList.Clear();
    }

    public override void SetSelectedSlot(InventorySlot selectedslot)
    {
        m_SelectedSlot = selectedslot;
         
        BuyItem();

    }

    public void AddToPlayerInventory(InventorySlot slot)
    {
        m_playerInstance.PlayerInventory.IV_ScrapWallet._runtimeValue -= slot.GetComponent<Sellable>().I_Price;
        if (slot.ST_SlotType == Slot.SlotType.Collectible)
        {
            m_playerInstance.PlayerInventory.AddCollectible(slot.ItemInSlot);

            Destroy(slot.gameObject);
        }
        else if(slot.ST_SlotType == Slot.SlotType.Hat)
        {
            m_playerInstance.PlayerInventory.AddHat(slot.GetComponent<HatHolder>().GetObject());
            Destroy(slot.gameObject);
        }
     
    }

    public bool CanPlayerAffordItem(int price)
    {            
        return price <= m_playerInstance.PlayerInventory.IV_ScrapWallet._runtimeValue;
    }


    void BuyItem()
    {
        if (m_SelectedSlot)
        {

            InventorySlot slot = m_SelectedSlot; // Selected slot is the one thats been clicked.

                    if (slot.GetComponent<Sellable>().B_IsSold == false && CanPlayerAffordItem(slot.GetComponent<Sellable>().I_Price)) //If the item is not sold and it can be afforded.
                    {
                        slot.GetComponent<Sellable>().B_IsSold = true; // Set is sold to true.

                        
                            AddToPlayerInventory(slot);
                            m_myVendor.AddToSoldList(slot.ID_ItemInSlot);
                            _ES_eventSystem.SetSelectedGameObject(CloseButton.gameObject);
                            



                    }
         
        }
    }


    public enum ShopDisplayOption
    {
        Hats = 0,
        Collectibles = 1,
        Max = 2
    }
}
