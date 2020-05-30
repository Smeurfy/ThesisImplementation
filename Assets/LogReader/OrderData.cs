using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class OrderData
{
    /// <summary>
    /// Dictionary that stores the count of true values of each tier.
    /// </summary>
    /// <param name="string">Tier</param>
    /// <param name="int">Count</param>
    public Dictionary<string, int> _monstDifferent = new Dictionary<string, int>();

    /// <summary>
    /// Var that stores how many people ordered correctly
    /// </summary>
    public int _correctOrder = 0;

    /// <summary>
    /// Var that stores the total time taken to order each monster
    /// </summary>
    /// <param name="string">MonsterName</param>
    /// <param name="int">Time</param>
    public float _totalTime = 0;

    /// <summary>
    /// Var that stores how many people selected all version before assigning
    /// an order
    /// </summary>
    /// <param name="string">MonsterName</param>
    /// <param name="int">Count</param>
    public int _allSelected = 0;

    /// <summary>
    /// Dictionary that stores how many person started the order with the hardest
    /// version or the easyiest.
    /// </summary>
    /// <param name="string">Order Assigned</param>
    /// <param name="int">Count</param>
    public Dictionary<string, int> _orderPick = new Dictionary<string, int>();
}