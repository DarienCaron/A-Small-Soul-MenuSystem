using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatMenu : InventoryMenu {

    private InventorySlot m_SlotSelected;
    private InventorySlot m_SlotToChange;

    private PlayerCreature m_PlayerReference;

    

    public Button ConfirmButton;
    public Button CancelButton;

    public InventorySlot HatSlotPrefab;

    private List<InventorySlot> m_HatHoldingSlotList;

    //private ItemViewer _it_Hatviewer;

    
  

    public Image SlotArea;
    public RectTransform SlotAreaTransform;
    public Vector2 V2_SlotAreaDefaultBottomMin;
    public Vector2 V2_DefaultSize;

    [Tooltip("The amount to move the bottom of the rectTransform down in the list")]
    public float SlotAreaBottomOffset;
    [Tooltip("A margin for the bottom of the rectTransform")]
    public float SlotAreaBottomMargin;

    float TopLocation;

    public Scrollbar HatScrollBar;

    protected override void Start()
    {
        base.Start();

        TopLocation = SlotAreaTransform.offsetMax.y;

        m_HatHoldingSlotList = new List<InventorySlot>();

        if (ConfirmButton)
        {
            ConfirmButton.onClick.AddListener(OnConfirm);
        }
        if(CancelButton)
        {
            CancelButton.onClick.AddListener(OnCancel);
        }

        //_it_Hatviewer = GetComponentInChildren<ItemViewer>();

        V2_DefaultSize = SlotAreaTransform.sizeDelta;
    }
    protected override void Awake()
    {
        m_PlayerReference = PlayerBrain.s_MasterBrainRef.p_PlayerRef;
    }

    public override void Update()
    {
        base.Update();

        //UpdateItemViewer(_it_Hatviewer, HoveredSlot);

        CheckForCancelMenu();


    }

    void UpdateSlots()
    {
        //The current number of hats
        float currentHatCount = 0;
        //The total number of hats avaiable to the player
        float totalHatCount = 0;
        //SlotAreaTransform.offsetMin = V2_SlotAreaDefaultBottomMin;
        SlotAreaTransform.sizeDelta = V2_DefaultSize;

        foreach (Hat _hat in m_PlayerReference.PlayerInventory.PlayerHats) // Loop through all the player hats.
        {
            //Increment the current hats and total hats by one
            currentHatCount += 1;
            totalHatCount += 1;

            InventorySlot slot = Instantiate(HatSlotPrefab); // Instantiate a slot prefab
            slot.GetComponent<HatHolder>().SetObject(_hat); // Get the hat holder component.
            slot.ID_ItemInSlot = _hat.p_ID_ItemDescription; // Set the slots item description.
            if (SlotArea) // If our slot area is not null
            {
                m_HatHoldingSlotList.Add(slot); // Add the slot to the list
                slot.transform.SetParent(SlotArea.transform, false); // Set the slots parent.

                //Are you currently at more than 3 hats (the number per row)
                if(currentHatCount > 3)
                {
                    //Move donw the bottom of the area for the hats by 350 (the space between each row)
                    SlotAreaTransform.offsetMin -= new Vector2(0, SlotAreaBottomOffset);
                    //Reset the current hat count
                    currentHatCount = 0;

                }
                //Have you reached the maximum number of hats
                if (totalHatCount >= m_PlayerReference.PlayerInventory.PlayerHats.Count)
                {
                    //Offset the bottom of the slot area by an extra 100 units
                    SlotAreaTransform.offsetMin -= new Vector2(0, SlotAreaBottomOffset);
                }


            }
        }

        GO_FirstSelectedObject = m_HatHoldingSlotList[0].gameObject;
    }

    public override void SetSelectedSlot(InventorySlot slot)
    {
        base.SetSelectedSlot(slot); // Overriden method which is called when a slot is clicked.

        m_SlotSelected = slot;



        ConfirmButton.Select();

    }
    public override void Push()
    {
        PlayerBrain.s_MasterBrainRef.PauseGame(); // Added
        base.Push();

        //V2_SlotAreaDefaultBottomMin = SlotAreaTransform.offsetMin;
        SlotAreaTransform.offsetMax = new Vector2(SlotAreaTransform.offsetMax.x, TopLocation);

        UpdateSlots();

        HatScrollBar.value = 0;
    }

    public override void Pop()
    {
        ClearSlots();
        base.Pop();
      
    }

    public void OnConfirm()
    {       

        if(m_SlotSelected) // check if the slot we clicked on isn't null
        {
            HatHolder ChosenHat = m_SlotSelected.GetComponent<HatHolder>(); // Get the hat holder object from the slot.

            PlayerBrain.s_MasterBrainRef.p_PlayerRef.ChangeHats(ChosenHat.GetObject()); // Get our player to change hats.
            Pop(); // Pop the menu off
        }

    }

    public void OnCancel()
    {
        Pop();
    }


    private void ClearSlots()
    {
        foreach (InventorySlot HSlot in m_HatHoldingSlotList)
        {
            Destroy(HSlot.gameObject); // destroy the slot objects.
        }
        m_HatHoldingSlotList.Clear(); // clear the list.
    }
}
