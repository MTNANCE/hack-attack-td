﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Class representing the computer component
/// </summary>
public class Computer : Component, IPointerClickHandler {

    [SerializeField] // Contains the sprites for the different upgrades
    private Sprite[] computerSprites;

    // Initialization
    private void Start() {
        Upgrades = new ComponentUpgrade[] {
            new ComponentUpgrade("Modern Laptop", 100, computerSprites[1], 100, 100, 500),
            new ComponentUpgrade("Gaming Laptop", 500, computerSprites[2], 200, 200, 1000),
        };
        Name = "Old Computer";
        Status = true;
        Sellable = false;
        Price = NextUpgrade.Price;
        RepairPrice = 50;
        SellValue = 50;
        Durability = 250f;
        Sprite = computerSprites[0];
    }

}