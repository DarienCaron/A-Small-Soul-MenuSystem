using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/*
 *  Creators: Darien
 *  
 *  Description: Component that inherits from UIMenu to encapsulate all the pieces of an inventory menu.
 *  
 *  Last Edit: 29 / 10 / 2018 (Darien)
 * 
 */
public class InventoryMenu : UIMenu
{


    // List<InventorySlot> ItemSlots;
    protected static InventoryMenu s_p_InventoryMenuInstance;






    protected InventorySlot m_HoveredSlot;

    protected InventorySlot m_SelectedSlot;




    protected PlayerCreature _pc_PlayerRef;
    

    List<InventorySlot> _list_is_InventorySlots;

    //private ToolChangerSlot _tcs_currentSelectedToolSlot;

    private ItemViewer _iv_ItemViewer;

    public bool IsOpen { get; set; }

    [Header("Important Events")]
    [SerializeField]
    [Tooltip("Event that triggers when the menu needs to closed via cancel means")]
    protected GameEvent p_e_CloseMenuViaCancelEvent;

    public Text Txt_ScrapText;
    public Text Txt_EssenceText;




    // Must call base start for UI
    protected override void Start()
    {
        base.Start();
        //GetComponentsInChildren(false, ItemSlots);

        _pc_PlayerRef = PlayerBrain.s_MasterBrainRef.p_PlayerRef;

        InitializeChildrenElements();

        


    }
    /*
  *  Creators: Darien
  *  
  *  
  *  Desc: Closes shop
  *  
  *  
  *  Last Edit: 30 / 11 / 2018 (Darien,  description)
  */
    protected virtual void Awake()
    {

        
    }
    /*
  *  Creators: Darien
  *  
  *  
  *  Desc: Overriden method, when pushed, it updates the inventory menu,
  *  
  *  
  *  Last Edit: 30 / 11 / 2018 (Darien,  description)
  */
    public override void Push()
    {
        if (IsOpen == false && p_UR_ParentRoot.GetIsMenuOpen("Pause") == false) // Check if we aren't paused.
        {


            base.Push();
            UpdateInventoryMenu();

            PlayerBrain.s_MasterBrainRef.PauseGame(); // Static player brain reference. Calls pause for the player.
            IsOpen = true;
        }
    }

    /*
  *  Creators: Darien
  *  
  *  
  *  Desc: Inits all the children elements of the inventory menu.
  *  
  *  
  *  Last Edit: 30 / 11 / 2018 (Darien,  description)
  */
    private void InitializeChildrenElements()
    {
        _list_is_InventorySlots = new List<InventorySlot>();

        InventorySlot[] slots = GetComponentsInChildren<InventorySlot>(true); // Get all the slots

        foreach (InventorySlot slot in slots)
        {
            _list_is_InventorySlots.Add(slot); // Add them to a slot list.
        }



        if (!s_p_InventoryMenuInstance)
        {
            s_p_InventoryMenuInstance = this; // Set singleton instance.
        }


        if (!_iv_ItemViewer)
        {
            _iv_ItemViewer = GetComponentInChildren<ItemViewer>(true);
        }


        if (_pc_PlayerRef) // If the player ref is not null
        {

            CurrentTools ct = _pc_PlayerRef.gameObject.GetComponent<CreatureUseTool>().CT_CurrentTools; // Get the players current tools


            List<PlayerTool> RandomTools = new List<PlayerTool> // Load them into a list
            {
            ct.PTLIST_ListOfEquippedTools[0],
            ct.PTLIST_ListOfEquippedTools[1],
            ct.PTLIST_ListOfEquippedTools[2],
            ct.PTLIST_ListOfEquippedTools[3]
            };

            List<InventorySlot> tools = new List<InventorySlot>(); // Create lists for every possible item in the inventory+
            List<InventorySlot> _Collectibles = new List<InventorySlot>();
       
            List<InventorySlot> _Hats = new List<InventorySlot>();

            foreach (InventorySlot s in _list_is_InventorySlots) // Store them based on slot type
            {
                if (s.ST_SlotType == Slot.SlotType.Tool)
                {
                    tools.Add(s);

                }
                else if (s.ST_SlotType == Slot.SlotType.Collectible)
                {
                    _Collectibles.Add(s);

                }          
                else if(s.ST_SlotType == Slot.SlotType.Hat)
                {
                    _Hats.Add(s);
                }

            }

            if(_Hats.Count == 1)
            {
                if (_pc_PlayerRef.CurrentHat)
                {
                    _Hats[0].ID_ItemInSlot = _pc_PlayerRef.CurrentHat.p_ID_ItemDescription; // Store the hat in the hat slot
                    _Hats[0].GetComponent<HatHolder>().SetObject(_pc_PlayerRef.CurrentHat);
                }
            }
               
            
      

     


            foreach (InventorySlot _toolSlot in tools) // Fill the tool slots.
            {
                if (_toolSlot.GetComponent<ToolHolder>())
                {
                    if (tools.Count <= 4)
                    {
                        for (int i = 0; i < RandomTools.Count; i++)
                        {
                            ToolHolder holder = tools[i].GetComponent<ToolHolder>();
                            holder.SetObject(RandomTools[i]);
                            if(holder.GetObject() != null)
                                tools[i].ID_ItemInSlot = holder.GetObject().p_ID_ItemDescription;

                        }
                    }
                }
            }

            // Set them to null for the garbage collector.
            tools = null;        
            _Collectibles = null;
            _Hats = null;
        }

    }

    /*
*  Creators: Darien
*  
*  
*  Desc: Called when the inventory menu is open.
*  
*  
*  Last Edit: 30 / 11 / 2018 (Darien,  description)
*/
    public void UpdateInventoryMenu()
    {

        UpdateToolSlots();

        UpdateHats(_pc_PlayerRef.CurrentHat);

        if (Txt_EssenceText && Txt_ScrapText)
        {
            Txt_EssenceText.text = "Essence: " + PlayerBrain.s_MasterBrainRef.p_PlayerRef.PlayerInventory.IV_EssenceCount._runtimeValue;
            Txt_ScrapText.text = "Scrap: " + PlayerBrain.s_MasterBrainRef.p_PlayerRef.PlayerInventory.IV_ScrapWallet._runtimeValue;
        }
    }


    /*
*  Creators: Darien
*  
*  
*  Desc: Called when the inventory menu is pushed, it updates the hat slot.
*  
*  
*  Last Edit: 5 / 12 / 2018 (Darien,  description)
*/
    private void UpdateHats(Hat currentHat)
    {
        foreach (InventorySlot s in _list_is_InventorySlots)
        {
            if(s.ST_SlotType == Slot.SlotType.Hat)
            {
                if (currentHat == s.GetComponent<HatHolder>().GetObject()) // If the current hat hasn't changed
                {
                    break; // Theres no need to update
                    
                }
                else
                {
                    s.ID_ItemInSlot = currentHat.p_ID_ItemDescription; // Else store the item and the physical hat.
                    s.GetComponent<HatHolder>().SetObject(currentHat);
                }
            }
        }

        
    }

    /*
*  Creators: Darien
*  
*  
*  Desc: Called when the inventory menu is opened, updates the tool slots.
*  
*  
*  Last Edit: 5 / 12 / 2018 (Darien,  description)
*/
    private void UpdateToolSlots()
    {
        if (_pc_PlayerRef)
        {
            List<PlayerTool> CurrentCreatureTools = new List<PlayerTool> // Store the current tools.
            {
            _pc_PlayerRef.gameObject.GetComponent<CreatureUseTool>().CT_CurrentTools.PTLIST_ListOfEquippedTools[0],
            _pc_PlayerRef.gameObject.GetComponent<CreatureUseTool>().CT_CurrentTools.PTLIST_ListOfEquippedTools[1],
            _pc_PlayerRef.gameObject.GetComponent<CreatureUseTool>().CT_CurrentTools.PTLIST_ListOfEquippedTools[2],
            _pc_PlayerRef.gameObject.GetComponent<CreatureUseTool>().CT_CurrentTools.PTLIST_ListOfEquippedTools[3]
             };


            foreach (InventorySlot _slot in _list_is_InventorySlots) // Loop through all the tool slots.
            {
                if (_slot.ST_SlotType == Slot.SlotType.Tool)
                {
                    foreach (PlayerTool pTool in CurrentCreatureTools) // If the slots a tool slot, loop through the current tools
                    {
                        ToolHolder tHolder = _slot.GetComponent<ToolHolder>();

                        if (CurrentCreatureTools.Contains(tHolder.GetObject())) // If the slots tool holders item is contained within the current tools
                        {
                            break;
                        }
                        else
                        {
                            tHolder.SetObject(pTool); // The tool is wrong, switch it.
                            _slot.ID_ItemInSlot = pTool.p_ID_ItemDescription;
                        }
                    }

                }
            }
        }
    }

    public override void Update()
    {
        base.Update();

        UpdateItemViewer(_iv_ItemViewer, m_HoveredSlot);


    }
    private void LateUpdate()
    {
        CheckForCancelMenu();
    }

    protected void UpdateItemViewer(ItemViewer _viewer, InventorySlot _slot)
    {
        if (_slot)
        {
            if (_iv_ItemViewer && _slot.ID_ItemInSlot)
            {
                if (_slot.I_InventoryImage)
                {
                    _iv_ItemViewer.UpdateItemViewer(_slot.ID_ItemInSlot.S_ItemName, _slot.ID_ItemInSlot.S_ItemDescription, _slot.ID_ItemInSlot.S_ItemArt);
                }
                else
                {
                    _iv_ItemViewer.UpdateItemViewer(_slot.ID_ItemInSlot.S_ItemName, _slot.ID_ItemInSlot.S_ItemDescription);
                }

            }

        }
    }

    public void SetHoveredSlot(InventorySlot slot)
    {
        m_HoveredSlot = slot;
    }

    public virtual void SetSelectedSlot(InventorySlot slot)
    {
        m_SelectedSlot = slot;
    }

    public InventorySlot GetSelectedSlot()
    {
        return m_SelectedSlot;
    }

    public override void Pop()
    {
        if (IsOpen == true)
        {


            base.Pop();
            PlayerBrain.s_MasterBrainRef.BRAINSTATE_CurrentBrainState = BrainState.BRAINSTATE_IDLE;
            if(!(this is HatMenu) && !(this is SouvenirMenu) && !(this is ToolMenu))
            {
                PlayerBrain.s_MasterBrainRef.UnPauseGame(); // added
            }
        
            IsOpen = false;

        }
    }


    protected void CheckForCancelMenu()
    {
        if (Input.GetButtonDown("Tool"))
        {
            if (p_e_CloseMenuViaCancelEvent)
            {
                p_e_CloseMenuViaCancelEvent.Trigger();
            }
        }
        else
        {
            return;
        }
    }

    
}
