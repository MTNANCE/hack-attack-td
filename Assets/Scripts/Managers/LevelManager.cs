﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>LevelManager</c> handles the level specific text and values 
/// (Currently not used that much as we only have one level)
/// </summary>
public class LevelManager : MonoBehaviour {

    /// <summary>
    /// Initializes level 1 related values and information
    /// </summary>
    private void Start() {
        // Set the initial player currency
        GameManager.Instance.SetCurrency(800);
        // Set the information panel title
        GameManager.Instance.informationPanelTitle.text = "Level 1 Information";
        // Set the information panel text
        GameManager.Instance.informationPanelText.text = "In this level you will have to defend yourself and the classified document from incoming attacks from the web. You have access to specific component features, firewall ports control and backup management tool.";
    }

}