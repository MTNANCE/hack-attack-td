﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackupManager : Singleton<BackupManager> {

    [SerializeField] // A reference to the name of the backup selection panel
    private string structureCanvasName;

    [SerializeField] // A reference to the backup popup panel
    private GameObject backupPanel;

    [SerializeField] // A reference to the backup selection panel
    private GameObject backupSelectionPanel;

    [SerializeField] // A reference to the prefab used to display backup selection
    private GameObject backupBarItemSlotPrefab;

    // Checks to see if user can select component to backup
    public bool BackupReady { get; set; }

    // The selected component to backup
    private GameObject selectedGameObject;

    // The backupped component to replace the current component with
    public GameObject BackuppedComponent { get; set; }

    // Object pool containing backupped components
    private List<GameObject> listOfBackuppedComponents;

    // Checks if user have already selected a backup component to replace existing component
    public bool BackupComponentSelected { get; set; }

    // Getter and setter for the structure canvas for backup selection panel
    public GameObject structureCanvas { get; private set; }

    private void Awake() {
        structureCanvas = GameObject.Find(structureCanvasName);
        if (structureCanvas == null) {
            throw new System.SystemException("CompController has invalid reference to structure canvas. Please check the serialized fields.");
        }
    }

    private void Start() {
        this.listOfBackuppedComponents = new List<GameObject>();
        this.BackupComponentSelected = false;
        this.BackupReady = false;
    }

    public void InitBackup(GameObject componentToBackup) {
        this.selectedGameObject = componentToBackup;
    }

    public void CancelBackup(GameObject backup) {
        Defenses.CompController.Instance.HighlightAllStructures(false);
        Destroy(backup);
    }

    public void OpenPanel(Vector3 itemSlotPos) {
        this.backupPanel.transform.position = new Vector3(itemSlotPos.x, (itemSlotPos.y + 50), 0);
        this.backupPanel.SetActive(!(this.backupPanel.activeSelf));
    }

    /// <summary>
    /// Replaces the current component in canvas with the selected backup component
    /// </summary>
    /// <param name="selectedBackup">selected backup component</param>
    /// <param name="objectToReplace">component to be replaced</param>
    public void ReplaceComponent(GameObject selectedBackup, GameObject objectToReplace) {
        Defenses.CompController.Instance.HighlightAllStructures(false);
        if (this.BackupComponentSelected) {
            // Get the type of the object to replace
            System.Type type = ((Component) objectToReplace.GetComponent(typeof(Component))).GetType();
            // Get input and output fields of the object to replace
            System.Reflection.FieldInfo[] fields = type.GetFields();
            // Add the input and output fields extracted above into the backup to be placed
            foreach (System.Reflection.FieldInfo field in fields) {
                field.SetValue((Component) selectedBackup.GetComponent(typeof(Component)), field.GetValue((Component) objectToReplace.GetComponent(typeof(Component))));
            }
            // Add the selected backup to the canvas where the object to replace was
            selectedBackup = Instantiate(selectedBackup);
            // Set the selected backup position to that of the current component position
            selectedBackup.transform.position = objectToReplace.transform.position;
            // Destroy the object to replace from canvas
            Destroy(objectToReplace);
            // Set the selected backup in the object in canvas layer
            selectedBackup.transform.SetParent(Defenses.CompController.Instance.structureCanvas.transform);
            // Reset values
            this.BackupComponentSelected = false;
            this.BackuppedComponent = null;
        }
    }

    /// <summary>
    /// Is fired when "Add Backup" button 
    /// in backup panel is clicked, and sets global
    /// variables to "waiting" status.
    /// </summary>
    public void AddComponentToBackup() {
        this.ShowBackupPanel(false);
        this.BackupReady = true;
        Defenses.CompController.Instance.HighlightAllStructures(true);
    }

    /// <summary>
    /// After AddComponent() is fired and user clicks a component in canvas, 
    /// then this component will be taken a backup off and added to 
    /// backup pool/backup selection manager
    /// </summary>
    /// <param name="gameObject">gameobject to backup</param>
    public void AddToBackupPool(GameObject gameObject) {
        this.BackupReady = false;
        Defenses.CompController.Instance.HighlightAllStructures(false);
        System.Type type = ((Component) gameObject.GetComponent(typeof(Component))).GetType();
        // TODO FIX this so it isnt instantiated and added to project structure folder
        if (GameObject.Find("ListOfBackuppedGameObjects").transform.childCount >= 7) {
            Destroy(GameObject.Find("ListOfBackuppedGameObjects").transform.GetChild(0).gameObject);
        }
        GameObject clone = Instantiate(gameObject);
        clone.transform.SetParent(GameObject.Find("ListOfBackuppedGameObjects").transform);
        // Copy all the values of the component inside gameObject and add the to the clone
        System.Reflection.FieldInfo[] fields = type.GetFields();
        System.Reflection.PropertyInfo[] properties = type.GetProperties();
        clone.AddComponent(gameObject.GetComponents(typeof(Component)).GetType());
        // Assign properties
        for (int i = 0; i <= 17; i++) {
            System.Reflection.PropertyInfo property = properties[i];
                property.SetValue((Component) clone.GetComponent(typeof(Component)), property.GetValue((Component)  gameObject.GetComponent(typeof(Component))));
        }
        AddBackupToListOfBackups(clone);
    }

    /// <summary>
    /// Creates a backup slot from the given backup object
    /// </summary>
    /// <param name="backupObject">backup object to create backup slot from</param>
    public void ConvertGameObjectToBackupBarItemSlot(GameObject backupObject) {
        // Instantiate new backupslot
        GameObject backupSlot = Instantiate(this.backupBarItemSlotPrefab);
        // Set the backupslot to be placed in BackupSelection
        backupSlot.transform.SetParent(this.structureCanvas.transform);
        // Set the backupped game object in backupslot prefab
        ((BackupBarItemSlot) backupSlot.GetComponent(typeof(BackupBarItemSlot))).backuppedComponent = backupObject;
        // Set the backup slot image to be the same of backupObject
        ((BackupBarItemSlot) backupSlot.GetComponent(typeof(BackupBarItemSlot))).canvasImage.sprite = ((Component) backupObject.GetComponent(typeof(Component))).Sprite;
    }

    /// <summary>
    /// Adds the backupobject to the list of backupped components.
    /// If the list contains more then 6 backups delete the oldest backup
    /// </summary>
    /// <param name="backupObject">backup object to add to the list of backupped components</param>
    public void AddBackupToListOfBackups(GameObject backupObject) {
        // If list of backupped components is equal to 5 delete the oldest and add new
        if (this.listOfBackuppedComponents.Count >= 6) {
            this.listOfBackuppedComponents.RemoveAt(0);
        }
        // Add the component to the list of backupped components
        this.listOfBackuppedComponents.Add(backupObject);
        // Delete all current backup slots (TODO Add feature so we dont have to delete all. just replace single)
        foreach (Transform obj in this.structureCanvas.transform) {
            Destroy(obj.gameObject);
        } 
        // In the end, refresh the backup selection manager to show updated list of backupped components
        foreach(GameObject obj in this.listOfBackuppedComponents) {
            ConvertGameObjectToBackupBarItemSlot(obj);
        }
    }

    /// <summary>
    /// Highlights only the available structures you can swap
    /// the selected backup component with
    /// </summary>
    /// <param name="component">selected backup component</param>
    /// <param name="condition">condition for if it should be highligted or not</param>
    public void HighlightReplacableComponents(Component component, bool condition) {
        foreach (Transform child in Defenses.CompController.Instance.structureCanvas.transform) {
            Component comp;
            if ((comp = child.gameObject.GetComponent(typeof(Component)) as Component) != null) {
                if (comp.GetType() == component.GetType()) {
                    comp.ShowHighlight(condition);
                }
            }
        }
    }

    /// <summary>
    /// Shows or hides the backup panel
    /// depending on the input param. true for showing
    /// and false for hiding
    /// </summary>
    /// <param name="active">either true or false</param>
    public void ShowBackupPanel(bool active) {
        this.backupPanel.SetActive(active);
    }

    /// <summary>
    /// Shows and hides the backup selection panel
    /// </summary>
    public void ShowBackupSelectionPanel() {
        ShowBackupPanel(false);
        this.backupSelectionPanel.SetActive(!this.backupSelectionPanel.activeSelf);
    }

}