﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Delegate for the currency changed event
public delegate void CurrencyChanged();

/// <summary>
/// Class <c>GameManager</c> handles all currency related features, the currently selected components
/// and it control the information panel and the module panel that displays component specific information
/// </summary>
public class GameManager : Singleton<GameManager> {

    #region EVENTS

    // Event is triggered when currency changes
    public event CurrencyChanged Changed;

    #endregion

    #region LEVEL_INFORMATION_PANEL

    [Header("Information Panel")]

    [SerializeField] // Reference to the level information panel
    private GameObject levelInformationPanel;

    [SerializeField] // Reference to the background panel
    private GameObject informationPanel;

    [SerializeField] // Reference to the information panel title
    public Text informationPanelTitle;

    [SerializeField] // Reference to the information panel text
    public Text informationPanelText;

    /// <summary>
    /// Closes the level information panel
    /// </summary>
    public void CloseInformationPanel() {
        this.levelInformationPanel.SetActive(false);
    }

    #endregion

    #region MODULE_PANEL

    [Header("Module Panel")]

    [SerializeField] // A reference to the panel object
    public GameObject modulePanel;

    [SerializeField] // A reference to the panel image
    private Image panelImage;

    [SerializeField] // A reference to the panel name
    private Text panelName;

    [SerializeField] // A reference to panel status
    private Text panelStatus;

    [SerializeField] // A reference to the panel durability text
    private Text panelDurability;

    [SerializeField] // A reference to the repair button
    private Button repairButton;

    [SerializeField] // A reference to the repair text price
    private Text repairText;

    [SerializeField] // A reference to the upgrade button
    private Button upgradeButton;
    
    [SerializeField] // A reference to the upgrade text price
    private Text txtPrice;

    [SerializeField] // A reference to the sell button
    private Button sellButton;

    [SerializeField] // A reference to the sell button text
    private Text sellText;
    
    [Header("Miscellaneous")]

    [SerializeField] // A reference to the currency text
    private Text currencyText;

    #endregion

    #region VARIABLES

    // The current selected component
    private Component selectedComponent;

    // Fix this
    private GameObject selectedGameObject;

    private IEnumerator coroutine;

    /// <summary>
    /// if there is a current selected component,
    /// return it, and if not return null
    /// </summary>
    public Component GetSelectedComponent {
        get {
            if (this.selectedComponent != null) {
                return this.selectedComponent;
            } else {
                return null;
            }
        }
    }

    /// <summary>
    /// if there is a current selected game object,
    /// return it, and if not return null
    /// </summary>
    public GameObject GetSelectedGameObject {
        get {
            if (this.selectedGameObject != null) {
                return this.selectedGameObject;
            } else {
                return null;
            }
        }
    }

    // The player's currency
    private int currency;

    // Indicates if the game is over
    private bool gameOver;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Returns the current currency
    /// </summary>
    /// <returns>Returns the current currency</returns>
    public int GetCurrency() {
        return this.currency;
    }

    /// <summary>
    /// Sets the currency and activates event
    /// </summary>
    public void SetCurrency(int value) {
        this.currency = value;
        ChangeCurrencyTextColor("#FDFF00", value);
        OnCurrencyChanged();
    }

    /// <summary>
    /// Returns true if value was successfully 
    /// subtracted from currency and false if not
    /// </summary>
    /// <param name="value">value to subtract</param>
    /// <returns>true if value was successfully 
    /// subtracted from currency and false if not</returns>
    public bool SubtractFromCurrency(int value) {
        if ((GetCurrency() - value) >= 0) {
            SetCurrency(GetCurrency() - value);
            return true;
        } else {
            this.coroutine = ChangeCurrencyTextColorSwap();
            StartCoroutine(this.coroutine);
            return false;
        }
    }

    /// <summary>
    /// Called when coroutine is started and swaps
    /// the currency text color to failure and back
    /// </summary>
    /// <returns></returns>
    public IEnumerator ChangeCurrencyTextColorSwap() {
        ChangeCurrencyTextColor("#FF0000", this.currency);
        yield return new WaitForSeconds(2.0f);
        ChangeCurrencyTextColor("#FDFF00", this.currency);
    }

    /// <summary>
    /// Changes the currency text to specified color
    /// </summary>
    /// <param name="color">color of the currency text</param>
    /// <param name="currency">currency to display</param>
    private void ChangeCurrencyTextColor(string color, int currency) {
        this.currencyText.text = "<color=" + color + ">Resources: " + currency + "</color>";
    }

    #endregion

    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start() {
		// Close all panels when user press cancel/escape, or presses on an empty space.
		EventManager.onCancel += ClosePanels;
		EventManager.onCanvasClick += ClosePanels;

		EventManager.onRefreshPanel += UpdateComputerPanel;
	}

	/// <summary>
	/// Closes all panels visible on screen.
	/// </summary>
	private void ClosePanels() {
        if  (informationPanel != null) {
            informationPanel.SetActive(false);
            DeselectGameObject();
            ShowStats(false);
        }
	}

	/// <summary>
	/// When the currency changes
	/// </summary>
	public void OnCurrencyChanged() {
        Changed?.Invoke();
    }

    /// <summary>
    /// Selects a GameObjext by clicking it
    /// </summary>
    /// <param name="gameObject">The clicked gameObject</param>
    public void SelectGameObject(GameObject gameObject) {
        if (this.selectedGameObject != null) {
            this.selectedGameObject = gameObject;
            this.selectedComponent = (Component) this.selectedGameObject.GetComponent(typeof(Component));
        } else {
            this.selectedGameObject = gameObject;
            this.selectedComponent = (Component) this.selectedGameObject.GetComponent(typeof(Component));
            UpdateComputerPanel();
            ShowModulePanel(true);
        }
    }

    /// <summary>
    /// Called when the selectedComponent is clicked again
    /// and sets the selectedComponent to null and hides the stats panel
    /// </summary>
    public void DeselectGameObject() {
        if (this.selectedGameObject != null) {
            this.selectedGameObject = null;
            this.selectedComponent = null;
            ShowModulePanel(false);
        }
    }

    /// <summary>
    /// Updates the stats panel information and styling 
    /// depending on what component was clicked, its state and if the 
    /// user have enough currency to buy/upgrade etc..
    /// </summary>
    public void UpdateComputerPanel() {
        this.panelName.text = "Lvl " + this.selectedComponent.ComponentLevel + ": " + this.selectedComponent.Name;
        this.panelStatus.text = this.selectedComponent.GetStatus();

        // Controls the styling of upgrade button
        if (this.selectedComponent.NextUpgrade != null && this.selectedComponent.NextUpgrade.Price <= GetCurrency()) {
            this.upgradeButton.interactable = true;
            this.upgradeButton.GetComponent<Image>().color = Color.green;
            this.txtPrice.color = Color.white;
            this.txtPrice.text = "Upgrade (Cost: " + this.selectedComponent.NextUpgrade.Price + ")";
            // Comment here
        } else if (this.selectedComponent.NextUpgrade != null && this.selectedComponent.NextUpgrade.Price > GetCurrency()) {
            this.upgradeButton.interactable = false;
            this.upgradeButton.GetComponent<Image>().color = Color.grey;
            this.txtPrice.color = Color.black;
            this.txtPrice.text = "Upgrade (Cost: " + this.selectedComponent.NextUpgrade.Price + ")";
            // Comment here
        } else {
            this.upgradeButton.interactable = false;
            this.upgradeButton.GetComponent<Image>().color = Color.grey;
            this.txtPrice.color = Color.black;
            this.txtPrice.text = "Max Upgraded";
        }
        
        // Controls the styling of repair button
        if (this.selectedComponent.RepairPrice <= GetCurrency() && this.selectedComponent.Status == false) {
            this.repairButton.interactable = true;
            this.repairButton.GetComponent<Image>().color = Color.green;
            this.repairText.color = Color.white;
            this.repairText.text = "Repair (Cost: " + this.selectedComponent.RepairPrice + ")";
            // Comment here
        } else if (this.selectedComponent.RepairPrice > GetCurrency() && this.selectedComponent.Status == true) {
            this.repairButton.interactable = false;
            this.repairButton.GetComponent<Image>().color = Color.grey;
            this.repairText.color = Color.black;
            this.repairText.text = "Repair (Cost: " + this.selectedComponent.RepairPrice + ")";
        } else {
            this.repairButton.interactable = false;
            this.repairButton.GetComponent<Image>().color = Color.grey;
            this.repairText.color = Color.black;
            this.repairText.text = "Component is Active!";
        }

        // Controls the styling of sell button
        if (this.selectedComponent.Sellable == false) {
            this.sellButton.interactable = false;
            this.sellButton.GetComponent<Image>().color = Color.grey;
            this.sellText.color = Color.black;
            this.sellText.text = "Can't be sold";
        } else {
            this.sellButton.interactable = true;
            this.sellButton.GetComponent<Image>().color = Color.red;
            this.sellText.color = Color.white;
            this.sellText.text = "Sell (" + this.selectedComponent.SellValue + ")";
        }

        // Checks if selected component is computer, if so show durability text, if not hide the text
        // TODO Add check for other components using the durability field
        if (this.selectedComponent.GetComponent(typeof(Component)).GetType() == typeof(Computer)) {
            this.panelDurability.enabled = true;
            this.panelDurability.text = "Durability: " + this.selectedComponent.Durability;
        } else {
            this.panelDurability.enabled = false;
        }

        // Custom visual settings for document component
        if (this.selectedComponent.GetComponent(typeof(Component)).GetType() == typeof(Document)) {
            if (this.selectedComponent.NextUpgrade != null && this.selectedComponent.NextUpgrade.Price <= GetCurrency() && UserBehaviourProfile.Instance.documentHacked == false) {
                this.selectedComponent.Status = true;
                this.upgradeButton.interactable = true;
                this.upgradeButton.GetComponent<Image>().color = Color.green;
                this.txtPrice.color = Color.white;
                this.txtPrice.text = "Encryption (Cost: " + this.selectedComponent.NextUpgrade.Price + ")";
                // Comment here
            } else if (this.selectedComponent.NextUpgrade != null && this.selectedComponent.NextUpgrade.Price > GetCurrency() && UserBehaviourProfile.Instance.documentHacked == false) {
                this.selectedComponent.Status = true;
                this.upgradeButton.interactable = false;
                this.upgradeButton.GetComponent<Image>().color = Color.grey;
                this.txtPrice.color = Color.black;
                this.txtPrice.text = "Encryption (Cost: " + this.selectedComponent.NextUpgrade.Price + ")";
                // Comment here
            } else if (UserBehaviourProfile.Instance.documentHacked == true) {
                this.selectedComponent.Status = false;
                this.repairButton.interactable = false;
                this.repairButton.GetComponent<Image>().color = Color.grey;
                this.repairText.text = "R̴̯̿͜͝e̴̢̔p̵̨̭̾̏á̵̪͈i̷̥̰͋ȑ̸̩̖ ̷͚̪̇̂(̴͍͋C̷͖̜̀ọ̵̙̀s̷̙̙̄̑t̴̨͝)̵͙̍͝";
                this.repairText.color = Color.black;
                this.upgradeButton.interactable = false;
                this.upgradeButton.GetComponent<Image>().color = Color.grey;
                this.txtPrice.text = "U̶͚̻̚ͅp̴̭͆͝g̸̻͈͠r̶̨̤̟̃ͅạ̸̺͇̔̐̆d̶̬̠͛̋͊̅ȇ̴͉̙͈̫̏͗̑ ̶̝̞͆̿͝(̷̛̦͇C̴͇͙̻̃̅̄ͅŏ̴̢͔͙̟̒̎̒s̶̡̰̘̕̚t̷̘̳͚̀̐)̴̼̈̚";
                this.txtPrice.color = Color.black;
                this.sellButton.interactable = false;
                this.sellText.text = "S̵͓͇̆̿̉͑͐̎͋̒̾ĕ̸̡̧͙̖̰̺̼͇͜l̸͕̗̞̘͉̐̒͒̇͋̅̿̽̚͜l̶̢̫̺̪̼̤̻̑̏͑̀̃͜";
            }  else {
                this.upgradeButton.interactable = false;
                this.upgradeButton.GetComponent<Image>().color = Color.grey;
                this.txtPrice.color = Color.black;
                this.txtPrice.text = "Max Encrypted";
            }
        }

        this.panelImage.GetComponent<Image>().sprite = this.selectedComponent.Sprite;
        this.selectedComponent.SetCanvasSprite(this.selectedComponent.Sprite);
    }

    /// <summary>
    /// Updates the panels dynamic information on demand from other scripts;
    /// </summary>
    /// <param name="component">Component which contains the values that is wished to update</param>
    public void UpdateDynamicText(Component component) {
        this.panelDurability.text = "Durability: " + component.Durability;
    }

    /// <summary>
    /// Calls the selected components upgrade function
    /// if there are any upgrades left and if the user has enouch currency
    /// </summary>
    public void UpgradeComponent() {
        if (this.selectedComponent != null) {
            if (this.selectedComponent.ComponentLevel <= this.selectedComponent.Upgrades.Length && GetCurrency() >= this.selectedComponent.NextUpgrade.Price) {
                this.selectedComponent.Upgrade();
                UpdateComputerPanel();
            }
        }
    }

    /// <summary>
    /// Repairs the selected component if its status is disabled
    /// </summary>
    public void RepairComponent() {
        if (this.selectedComponent != null) {
            if (this.selectedComponent.RepairPrice < GetCurrency() && this.selectedComponent.Status == false) {
                this.selectedComponent.Repair();
                UpdateComputerPanel();
            }
        }
    }

    /// <summary>
    /// Sells the selected component and removes it from canvas
    /// </summary>
    public void SellComponent() {
        if (this.selectedComponent != null && this.selectedComponent.Sellable == true) {
			Defenses.CompController.Instance.DeleteStructure(selectedComponent);
            // Add the sell value of the component to the global currency
            SetCurrency(GetCurrency() + this.selectedComponent.SellValue);
            // Close information panels
            ShowStats(false);
            ShowModulePanel(false);
        }
    }

    /// <summary>
    /// Shows or hides the stats panel
    /// depending on the input param. true for showing
    /// and false for hiding
    /// </summary>
    /// <param name="active">either true or false</param>
    public void ShowStats(bool active) {
        this.informationPanel.SetActive(active);
    }

    public void ShowInformationPanel() {
        this.informationPanel.SetActive(!this.informationPanel.activeSelf);
    }

    /// <summary>
    /// Shows or hides the module panel
    /// depending on the input param. true for showing
    /// and false for hiding
    /// </summary>
    /// <param name="active">either true or false</param>
    public void ShowModulePanel(bool active) {
        this.modulePanel.SetActive(active);
    }

}