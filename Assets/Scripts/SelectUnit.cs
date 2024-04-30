using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor;
using UnityEngine.InputSystem;


public enum UNIT
{
    AlliedArmorUnit1,
    AlliedArmorUnit2,
    AlliedArmorUnit3,
    AlliedArmorUnit4,
    AlliedInfantryUnit1,
    AlliedInfantryUnit2,
    AlliedInfantryUnit3,
    AlliedInfantryUnit4,
    AlliedInfantryUnit5,
    AlliedInfantryUnit6
}

public class SelectUnit : MonoBehaviour
{
    public static SelectUnit Instance;
    
    //variables for unit selection
    public GameObject[] units;
    GameObject selectedMoveUnit;
    int numUnits;

    public InputAction RightClick;

    //variables for health bar
    private float maxInfantryHealth = 25f, maxArmorHealth = 50f;
    private int numArmorUnits = 4;
    private string[] unitToolbarNames = { "First Armor", "Second Armor", "Third Armor", "Fourth Armor", "1st Infantry", "2nd Infantry", "3rd Infantry", "4th Infantry", "5th Infantry", "6th Infantry" };
    private float[] unitHealths;
    const string textPrefix = "\n\nHealth:\n";

    private void Start()
    {
        Instance = this;

        RightClick.Enable();

        numUnits = units.Length;
        unitHealths = new float[numUnits];

        //setting max healths
        int i;
        for (i = 0; i < numArmorUnits; i++) { unitHealths[i] = maxArmorHealth; }
        for (i = numArmorUnits; i < numUnits; i++) { unitHealths[i] = maxInfantryHealth; }

        i = 0;
        string buttonName;
        foreach (UNIT unit in Enum.GetValues(typeof(UNIT)))
        {
            buttonName = unit.ToString();

            //sets initial health of units
            if (i > numArmorUnits) { updateHealthTextInButton(buttonName, unitToolbarNames[i], maxInfantryHealth.ToString()); }
            else { updateHealthTextInButton(buttonName, unitToolbarNames[i], maxArmorHealth.ToString()); }
            i++;
        }
    }

    private void Update()
    {
        if (RightClick.IsPressed())
        {
            resetSelectedMoveUnit(selectedMoveUnit);
        }
    }

    //selects unit
    public void selectArmor1() { if (unitHealths[(int)UNIT.AlliedArmorUnit1] > 0) { selectedMoveUnit = units[(int)UNIT.AlliedArmorUnit1]; } }
    public void selectArmor2() { if (unitHealths[(int)UNIT.AlliedArmorUnit2] > 0) { selectedMoveUnit = units[(int)UNIT.AlliedArmorUnit2]; } }
    public void selectArmor3() { if (unitHealths[(int)UNIT.AlliedArmorUnit3] > 0) { selectedMoveUnit = units[(int)UNIT.AlliedArmorUnit3]; } }
    public void selectArmor4() { if (unitHealths[(int)UNIT.AlliedArmorUnit4] > 0) { selectedMoveUnit = units[(int)UNIT.AlliedArmorUnit4]; } }
    public void selectInfantry1() { if (unitHealths[(int)UNIT.AlliedInfantryUnit1] > 0) { selectedMoveUnit = units[(int)UNIT.AlliedInfantryUnit1]; } }
    public void selectInfantry2() { if (unitHealths[(int)UNIT.AlliedInfantryUnit2] > 0) { selectedMoveUnit = units[(int)UNIT.AlliedInfantryUnit2]; } }
    public void selectInfantry3() { if (unitHealths[(int)UNIT.AlliedInfantryUnit3] > 0) { selectedMoveUnit = units[(int)UNIT.AlliedInfantryUnit3]; } }
    public void selectInfantry4() { if (unitHealths[(int)UNIT.AlliedInfantryUnit4] > 0) { selectedMoveUnit = units[(int)UNIT.AlliedInfantryUnit4]; } }
    public void selectInfantry5() { if (unitHealths[(int)UNIT.AlliedInfantryUnit5] > 0) { selectedMoveUnit = units[(int)UNIT.AlliedInfantryUnit5]; } }
    public void selectInfantry6() { if (unitHealths[(int)UNIT.AlliedInfantryUnit6] > 0) { selectedMoveUnit = units[(int)UNIT.AlliedInfantryUnit6]; } }

    //gets current selected unit
    public GameObject getSelectedMoveUnit() { return selectedMoveUnit; }

    //deselecting unit
    public void resetSelectedMoveUnit(GameObject unitToReset) {if (selectedMoveUnit == unitToReset) { selectedMoveUnit = null; }}

    public void changeHealth(GameObject unit, int healthValue)
    {
        //loops through enum to match GameObject with UNIT name, since they are identical
        int i = 0;
        foreach(UNIT unitName in Enum.GetValues(typeof(UNIT)))
        {
            if (unitName.ToString() == unit.ToString()) { break; }
            i++;
        }

        //works w/ both pos and negative values
        unitHealths[i] += healthValue;
        if (unitHealths[i] <= 0) { Destroy(unit); } //if the unit lost all health

        string buttonName;
        buttonName = unit.ToString();
        updateHealthTextInButton(buttonName, unitToolbarNames[i], unitHealths[i].ToString());

        


    }

    private void updateHealthTextInButton(string buttonName, string unitName, string newHealth)
    {
        //this section modifies the name to fit the corresponding GameObject name

        buttonName = buttonName.Remove(0, 6); //removes "Allied" from string
        //removes "Unit" from string
        if (buttonName.Length == 10) { buttonName = buttonName.Remove(5, 4); }//if the unit is an armor type
        else { buttonName = buttonName.Remove(8, 4); }//if the unit is an infantry type
        buttonName += "Button";

        //Gets the array of buttons in the Canvas object
        GameObject[] btn = GameObject.FindGameObjectsWithTag("Button");

        GameObject.Find(buttonName).GetComponentInChildren<TextMeshProUGUI>().text = unitName + textPrefix + newHealth;

    }

    public float getUnitHealth(GameObject allyUnit)
    {
        string unit = allyUnit.name;
        int i = 0;
        foreach(string unitName in Enum.GetNames(typeof(UNIT)))
        {
            if (unit == unitName) { return unitHealths[i]; }
            i++;
        }

        return -1;
    }

    public float getUnitHealth(int index)
    {
        return unitHealths[index];
    }

    public int getNumUnits()
    {
        return numUnits;
    }

    public GameObject getUnit(int index)
    {
        if (index > numUnits) { return null; }

        return units[index];
    }

    public string getToolbarName(string name)
    {
        int i = 0;
        foreach (string unit in Enum.GetNames(typeof(UNIT)))
        {
            if(unit == name)
            {
                return unitToolbarNames[i];
            }
            i++;
        }

        return null;
    }

    public void setUnitHealth(int index, float newHealth)
    {
        unitHealths[index] = newHealth;
    }
}
