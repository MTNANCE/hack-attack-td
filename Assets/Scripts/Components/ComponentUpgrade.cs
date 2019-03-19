﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentUpgrade {

    /// <summary>
    /// Generates getter and private setter for upgrade name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Generates getter and private setter for upgrade price
    /// </summary>
    public int Price { get; private set; }

    /// <summary>
    /// Generates getter and private setter for upgrade durability
    /// </summary>
    public float Durability { get; private set; }

    /// <summary>
    /// Generates getter and private setter for upgrade encryption
    /// </summary>
    public float Encryption { get; private set; }

    /// <summary>
    /// Generates getter and private setter for repair price
    /// </summary>
    public int RepairPrice { get; private set; }

    /// <summary>
    /// Generates getter and private setter for sell value
    /// </summary>
    public int SellValue { get; private set; }

    /// <summary>
    /// Generates getter and private setter for upgrade sprite
    /// </summary>
    public Sprite Sprite { get; private set; }

    /// <summary>
    /// General constructor for regular component upgrades
    /// </summary>
    /// <param name="price">price of the new component upgrade</param>
    /// <param name="sprite">sprite of the new component upgrade</param>
    public ComponentUpgrade(string name, int price, Sprite sprite, int repairPrice, int sellVal) {
        this.Name = name;
        this.Price = price;
        this.Sprite = sprite;
        this.RepairPrice = repairPrice;
        this.SellValue = sellVal;
    }

    /// <summary>
    /// Constructor for Computer Component upgrade
    /// </summary>
    /// <param name="price">price of the new computer upgrade</param>
    /// <param name="durability">durability of the new computer upgrade</param>
    /// <param name="sprite">sprite of the new computer upgrade</param>
    public ComponentUpgrade(string name, int price, Sprite sprite, int repairPrice, int sellVal, float durability) {
        this.Name = name;
        this.Price = price;
        this.Sprite = sprite;
        this.RepairPrice = repairPrice;
        this.SellValue = sellVal;
        this.Durability = durability;
    }

    /// <summary>
    /// Constructor for encryption upgrade
    /// </summary>
    /// <param name="price">price of the new computer upgrade</param>
    /// <param name="encryption">encryption of the new computer upgrade</param>
    /// <param name="sprite">sprite of the new computer upgrade</param>
    public ComponentUpgrade(string name, int price, Sprite sprite, float encryption) {
        this.Name = name;
        this.Price = price;
        this.Sprite = sprite;
        this.Encryption = encryption;
    }

}
