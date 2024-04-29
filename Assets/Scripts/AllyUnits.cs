using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllyUnits : MonoBehaviour
{
    public static AllyUnits Instance;

    public GameObject unitScriptHolder;
    private SelectUnit unitSelector;

    //there only needs to be one instance of every one of these
    private static GameObject[] units;
    private static float[] healths;
    private int numUnits;

    private void Start()
    {
        Instance = this;

        unitSelector = unitScriptHolder.GetComponent<SelectUnit>();

        numUnits = unitSelector.getNumUnits();
        Debug.Log("number of units: " + numUnits);
        units = new GameObject[numUnits];
        healths = new float[numUnits];
        fillArray();
    }

    private void Update()
    {
        updateHealthArray();

        if (numUnits == 0) { numUnits = unitSelector.getNumUnits(); }
        if (units.Length < numUnits)
        {
            units = new GameObject[numUnits];
            healths = new float[numUnits];
        }

        fillArray();
    }

    private void fillArray()
    {
        int i;
        for (i = 0; i < numUnits; i++)
        {
            units[i] = unitSelector.getUnit(i);
            healths[i] = unitSelector.getUnitHealth(i);
        }
    }

    public GameObject getUnit(int index)
    {
        return units[index];
    }

    public float getHealth(GameObject unit)
    {
        for(int i = 0; i < units.Length; i++)
        {
            if (units[i].name == unit.name)
            {
                if (healths[i]  < 0f)
                {
                    return 0f;
                }
                return healths[i];
            }
        }
        return -1f;
    }

    public void changeHealth(GameObject unit, float amount)
    {
        changeAllyHealth(unit,amount);
    }

    public int getNumUnits()
    {
        return numUnits;
    }

    private void updateHealthArray()
    {
        for (int i = 0; i < numUnits; i++)
        {
            unitSelector.setUnitHealth(i, healths[i]);
        }
    }

    public string regetToolbarName(string name)
    {
        return unitSelector.getToolbarName(name);
    }

    public void setAllyHealth(string unitName, float newHealth)
    {
        int i;
        for(i = 0; i < units.Length; i++)
        {
            if (units[i].name == unitName) { break; }
        }

        healths[i] = newHealth;
    }

    public void changeAllyHealth(GameObject unit, float amount)
    {
        
        int i;
        for (i = 0; i < units.Length; i++)
        {
            if (units[i].name == unit.name) { break; }
        }
        Debug.Log("ally unit is: " + units[i].name);

        healths[i] += amount;
        Debug.Log("ally unit changed health to " + healths[i]);

        updateHealthTextInButton(unit.name, regetToolbarName(unit.name), unit);
    }

    private void updateHealthTextInButton(string buttonName, string toolbarName, GameObject unit)
    {
        //this section modifies the name to fit the corresponding GameObject name

        buttonName = buttonName.Remove(0, 6); //removes "Allied" from string
        //removes "Unit" from string
        if (buttonName.Length == 10) { buttonName = buttonName.Remove(5, 4); }//if the unit is an armor type
        else { buttonName = buttonName.Remove(8, 4); }//if the unit is an infantry type
        buttonName += "Button";
        if(GameObject.Find(buttonName) == null ) { Debug.Log("Button not found"); }

        Debug.Log("setting viewable ally health to " + Math.Ceiling(getHealth(unit)));
        GameObject.Find(buttonName).GetComponentInChildren<TextMeshProUGUI>().text = toolbarName + "\n\nHealth:\n" +
                                                    (int)Math.Ceiling(getHealth(unit));

    }
}
