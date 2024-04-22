using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    //infantryDamage = 2; armorDamage = 5; airDamage = 15; artilleryDamage = 5; SAM damage instant kill on air
    [SerializeField] private int infantryDamage, armorDamage, airDamage, artilleryDamage;
    [SerializeField] private float maxCooldown;
    private bool inCombat;
    float cooldown;

    private bool isChasing;

    private void Awake()
    {
        cooldown = maxCooldown;
        isChasing = GetComponent<EnemyMovement>().getIsChasingAlly();
        inCombat = false;
    }

    public void beginCombatCycle()
    {
        cooldown = 0;
        inCombat = true;

        setButtonColorRed(GetComponent<AllyDetection>().getChasingAlly());
        StartCoroutine(CombatCycle(GetComponent<AllyDetection>().getChasingAlly()));
    }

    //while both units health are above 0, every cooldown seconds do damage to each other
    IEnumerator CombatCycle(GameObject allyUnit)
    {
        int allyHealth = gameObject.GetComponent<AllyUnits>().getHealth(GetComponent<AllyDetection>().getChasingAlly());
        int enemyHealth = gameObject.GetComponent<EnemyHealth>().getHealth();
        Debug.Log("Enemy health = " + enemyHealth);
        Debug.Log("ally health = " + allyHealth);

        while (allyHealth > 0 && enemyHealth > 0 && inCombat)
        {
            //if not chasing, exit combat cycle
            isChasing = GetComponent<EnemyMovement>().getIsChasingAlly();
            if (!isChasing)
            {
                Debug.Log("Exiting combat");
                exitCombatCycle(false, allyUnit);
                break;
            }

            //if the unit is "reloaded," do damage
            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
            }
            else if (cooldown <= 0)
            {
                Debug.Log("Damage Done");
                cooldown = maxCooldown;
                
                //ally damage taken
                if (gameObject.name.Contains("Armor"))
                {
                    allyHealth -= armorDamage;

                }
                else if (gameObject.name.Contains("Infantry"))
                {
                    allyHealth -= infantryDamage;

                }
                else if (gameObject.name.Contains("Artillery"))
                {
                    allyHealth -= artilleryDamage;

                }


                //enemy damage taken
                if (allyUnit.name.Contains("Armor"))
                {
                    enemyHealth -= armorDamage;
                    Debug.Log("Enemy took armor damage and has " + enemyHealth + "health reamaing");

                }
                else if (allyUnit.name.Contains("Infantry"))
                {
                    enemyHealth -= infantryDamage;
                    Debug.Log("Enemy took infantry damage and has " + enemyHealth + "health reamaing");

                }
                else if (allyUnit.name.Contains("Air"))
                {
                    enemyHealth -= airDamage;
                    Debug.Log("Enemy took air damage and has " + enemyHealth + "health reamaing");

                }


                //update ally health on button
                updateHealthTextInButton(allyUnit.name, gameObject.GetComponent<AllyUnits>().regetToolbarName(allyUnit.name), allyHealth);
                gameObject.GetComponent<AllyUnits>().setAllyHealth(allyUnit.name, allyHealth);

                //update enemy health
                gameObject.GetComponent<EnemyHealth>().setHealth(enemyHealth);

            }

            yield return null;
        }

        //check healths
        if (enemyHealth <= 0)
        {
            Debug.Log(gameObject.name + "has died");
            gameObject.GetComponent<EnemyAppearance>().hideAllSprites();
            gameObject.SetActive(false);
        }
        if (allyHealth <= 0)
        {
            Debug.Log(allyUnit.name + "has died");
            setAllyDeath(allyUnit);

            exitCombatCycle(true, allyUnit);
        }
    }

    public void exitCombatCycle(bool isAllyDead, GameObject allyUnit)
    {
        inCombat = false;

        if (!isAllyDead)
        {
            resetAllyButtonColor(allyUnit);
        }
    }

    private void setAllyDeath(GameObject unit)
    {
        //formats the game object name to match the button name
        string buttonName = unit.name;
        buttonName = buttonName.Remove(0, 6); //removes "Allied" from string
        //removes "Unit" from string
        if (buttonName.Length == 10) { buttonName = buttonName.Remove(5, 4); }//if the unit is an armor type
        else { buttonName = buttonName.Remove(8, 4); }//if the unit is an infantry type
        buttonName += "Button";


        Image button = GameObject.Find(buttonName).GetComponent<Image>();

        unit.GetComponent<Renderer>().enabled = false;
        button.color = Color.black;
    }

    private void resetAllyButtonColor(GameObject unit)
    {
        //formats the game object name to match the button name
        string buttonName = unit.name;
        buttonName = buttonName.Remove(0, 6); //removes "Allied" from string
        //removes "Unit" from string
        if (buttonName.Length == 10) { buttonName = buttonName.Remove(5, 4); }//if the unit is an armor type
        else { buttonName = buttonName.Remove(8, 4); }//if the unit is an infantry type
        buttonName += "Button";


        Image button = GameObject.Find(buttonName).GetComponent<Image>();

        button.color = Color.white;
    }

    private void updateHealthTextInButton(string buttonName, string unitName, int newHealth)
    {
        //this section modifies the name to fit the corresponding GameObject name

        buttonName = buttonName.Remove(0, 6); //removes "Allied" from string
        //removes "Unit" from string
        if (buttonName.Length == 10) { buttonName = buttonName.Remove(5, 4); }//if the unit is an armor type
        else { buttonName = buttonName.Remove(8, 4); }//if the unit is an infantry type
        buttonName += "Button";

        GameObject.Find(buttonName).GetComponentInChildren<TextMeshProUGUI>().text = unitName + "\n\nHealth:\n" + newHealth;

    }

    public bool getInCombat()
    {
        return inCombat;
    }

    private void setButtonColorRed(GameObject unit)
    {
        //formats the game object name to match the button name
        string buttonName = unit.name;
        buttonName = buttonName.Remove(0, 6); //removes "Allied" from string
        //removes "Unit" from string
        if (buttonName.Length == 10) { buttonName = buttonName.Remove(5, 4); }//if the unit is an armor type
        else { buttonName = buttonName.Remove(8, 4); }//if the unit is an infantry type
        buttonName += "Button";


        Image button = GameObject.Find(buttonName).GetComponent<Image>();

        button.color = Color.red;
    }

}
