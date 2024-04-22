using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAppearance : MonoBehaviour
{
    bool unitSeen, unitScouted;
    [SerializeField] float seenDistance;


    void Start()
    {
        //hides full sprite and shows blip
        gameObject.GetComponent<Renderer>().enabled = true;
        if (gameObject.transform.Find("Armor") != null)
        {
            gameObject.transform.Find("Armor").GetComponent<Renderer>().enabled = true;

        }
        else
        {
            gameObject.transform.Find("Infantry").GetComponent<Renderer>().enabled = true;

        }

        gameObject.transform.Find("Blip").GetComponent <Renderer>().enabled = false;
        unitSeen = true;//after testing change to false
        unitScouted = false;

    }

    private void Update()
    {
        setAppearance(getClosestAlly());
    }

    public bool getUnitSeen()
    {
        return unitSeen;
    }

    public bool getUnitScouted()
    {
        return unitScouted;
    }

    public float getClosestAlly()
    {
        int unitsToCheck = gameObject.GetComponent<AllyUnits>().getNumUnits(), i;
        float newDistance, shortestDistance = 10000f;//initial point is high enough that the furthest unit will still be closer than initial value
        GameObject closestUnit = null;

        for (i = 0; i < unitsToCheck; i++)
        {
            newDistance = Vector3.Distance(transform.position, gameObject.GetComponent<AllyUnits>().getUnit(i).transform.position);

            if (newDistance < shortestDistance)
            {
                shortestDistance = newDistance;
                closestUnit = gameObject.GetComponent<AllyUnits>().getUnit(i);
            }
        }
        return shortestDistance;
    }

    public void hideFullSprite()
    {
        gameObject.transform.Find("Blip").GetComponent<Renderer>().enabled = true;

        if (gameObject.transform.Find("Armor") != null)
        {
            gameObject.transform.Find("Armor").GetComponent<Renderer>().enabled = false;

        }
        else
        {
            gameObject.transform.Find("Infantry").GetComponent<Renderer>().enabled = false;

        }
    }

    public void showFullSprite()
    {
        if (gameObject.transform.Find("Armor") != null)
        {
            gameObject.transform.Find("Armor").GetComponent<Renderer>().enabled = true;

        }
        else
        {
            gameObject.transform.Find("Infantry").GetComponent<Renderer>().enabled = true;

        }
        gameObject.transform.Find("Blip").GetComponent<Renderer>().enabled = false;
    }

    public void setAppearance(float closestUnit)
    {
        if (closestUnit > seenDistance)
        {
            hideFullSprite();
        }
        else
        {
            showFullSprite();
        }
    }

    public void hideAllSprites()
    {
        if (gameObject.transform.Find("Armor") != null)
        {
            gameObject.transform.Find("Armor").GetComponent<Renderer>().enabled = false;

        }
        else
        {
            gameObject.transform.Find("Infantry").GetComponent<Renderer>().enabled = false;

        }
        gameObject.transform.Find("Blip").GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Renderer>().enabled = false;

    }

}
