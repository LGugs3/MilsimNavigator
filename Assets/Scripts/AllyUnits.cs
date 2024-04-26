using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyUnits : MonoBehaviour
{
    public static AllyUnits Instance;

    public GameObject unitScriptHolder;
    private SelectUnit unitSelector;
    private GameObject[] units;
    private float[] healths;
    private int numUnits;

    private void Start()
    {
        Instance = this;

        unitSelector = unitScriptHolder.GetComponent<SelectUnit>();

        numUnits = unitSelector.getNumUnits();
        units = new GameObject[numUnits];
        healths = new float[numUnits];
        fillArray();
    }

    private void Update()
    {
        updateHealthArray();
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
        changeAllyHealth(unit.name,amount);
    }

    public int getNumUnits()
    {
        return units.Length;
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

    public void changeAllyHealth(string unitName, float amount)
    {
        int i;
        for (i = 0; i < units.Length; i++)
        {
            if (units[i].name == unitName) { break; }
        }

        healths[i] += amount;
        Debug.Log("ally unit changed health to " + healths[i]);

        
    }
}
