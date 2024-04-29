using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    //infantryDamage = 2; armorDamage = 5; airDamage = 15; artilleryDamage = 5; SAM damage instant kill on air
    private static float infantryDamage, armorDamage, airDamage, artilleryDamage;
    [SerializeField] private float maxCooldown;
    private float bufferDamage;
    private bool inCombat;
    float cooldown;
    private int numCombatInstances;

    private bool isChasing;

    private List<GameObject> combatingUnits;

    private void Start()
    {
        cooldown = maxCooldown;
        isChasing = GetComponent<EnemyMovement>().getIsChasingAlly();
        inCombat = false;
        bufferDamage = 2f;

        infantryDamage = 2f;
        armorDamage = 5f;
        artilleryDamage = 10f;
        airDamage = 15f;

        numCombatInstances = 0;

        //trying to get the number of units returns a null ref error
        combatingUnits = new List<GameObject>(10 + 1);
        //junk shit so i dont get ref null error
        combatingUnits.Add(gameObject);
    }

    public void beginCombatCycle(GameObject allyUnit)
    {
        if (isUnitCombating(allyUnit)) { return; }
        combatingUnits.Add(allyUnit);
        Debug.Log(allyUnit.name + " has " + GetComponent<AllyUnits>().getHealth(allyUnit) + " health remaining");


        cooldown = 0;
        inCombat = true;
        numCombatInstances++;

        setButtonColorRed(GetComponent<AllyDetection>().getChasingAlly());
        StartCoroutine(CombatCycle(allyUnit));
    }

    //while both units health are above 0, every cooldown seconds do damage to each other
    IEnumerator CombatCycle(GameObject allyUnit)
    {
        System.Random rnd = new System.Random();
        float allyDamage,
              enemyDamage,
              allyHealth = gameObject.GetComponent<AllyUnits>().getHealth(allyUnit),
              enemyHealth = gameObject.GetComponent<EnemyHealth>().getHealth();
        
       

        while (allyHealth > 0f && enemyHealth > 0f && inCombat)
        {
            
            //for multiple instances
            if (getButtonColor(allyUnit) != Color.red)
            {
                setButtonColorRed(allyUnit);

            }

            //if not chasing, exit combat cycle
            isChasing = GetComponent<EnemyMovement>().getIsChasingAlly();
            if (!isChasing)
            {
                Debug.Log("Exiting combat");
                exitCombatCycle(false, allyUnit);
                break;
            }

            //if the unit is "reloaded," do damage
            if (cooldown > 0f)
            {
                cooldown -= Time.deltaTime;
            }
            else //if (cooldown <= 0f)
            {
                allyDamage = 0;
                enemyDamage = 0;

                cooldown = maxCooldown;
                double num = rnd.NextDouble();
                //ally damage taken
                if (gameObject.name.Contains("Armor"))
                {
                    if (Math.Round(num) % 2 == 0)
                    {
                        allyDamage = armorDamage + (bufferDamage * (float)num);
                    }
                    else
                    {
                        allyDamage = armorDamage - (bufferDamage * (float)num);
                    }

                    
                }
                else if (gameObject.name.Contains("Infantry"))
                {
                    if (Math.Round(num) % 2 == 0)
                    {
                        allyDamage = infantryDamage + (bufferDamage * (float)num);
                    }
                    else
                    {
                        allyDamage = infantryDamage - (bufferDamage * (float)num);
                    }

                }
                else if (gameObject.name.Contains("Artillery"))
                {
                    if (Math.Round(num) % 2 == 0)
                    {
                        allyDamage = artilleryDamage + (bufferDamage * (float)num);
                    }
                    else
                    {
                        allyDamage = artilleryDamage - (bufferDamage * (float)num);
                    }
                }
                


                //enemy damage taken
                if (allyUnit.name.Contains("Armor"))
                {
                    if (Math.Round(num) % 2 == 0)
                    {
                        enemyDamage = armorDamage + (bufferDamage * (float)num);
                    }
                    else
                    {
                        enemyDamage = armorDamage - (bufferDamage * (float)num);
                    }

                }
                else if (allyUnit.name.Contains("Infantry"))
                {
                    if (Math.Round(num) % 2 == 0)
                    {
                        enemyDamage = infantryDamage + (bufferDamage * (float)num);
                    }
                    else
                    {
                        enemyDamage = infantryDamage - (bufferDamage * (float)num);
                    }

                }
                else if (allyUnit.name.Contains("Air"))
                {
                    if (Math.Round(num) % 2 == 0)
                    {
                        enemyDamage = airDamage + (bufferDamage * (float)num);
                    }
                    else
                    {
                        enemyDamage = airDamage - (bufferDamage * (float)num);
                    }

                }

                //damage needs to be negative
                allyDamage *= -1;
                enemyDamage *= -1;
                if (enemyDamage > 0f) { enemyDamage = 0f; }
                if (allyDamage > 0f) { allyDamage = 0f; }
                //the more units a unit is fighting at the same time the less damage the unit does to each unit
                allyDamage /= numCombatInstances;
                enemyDamage /= numCombatInstances;

                gameObject.GetComponent<AllyUnits>().changeHealth(allyUnit, allyDamage);

                //update ally health on button
                //updateHealthTextInButton(allyUnit.name, gameObject.GetComponent<AllyUnits>().regetToolbarName(allyUnit.name),
                //                         allyUnit);

                GetComponent<EnemyHealth>().changeHealth(enemyDamage);


                //if there are multiple instances of combat cycle going at once, need to reget health every iteration
                allyHealth = gameObject.GetComponent<AllyUnits>().getHealth(allyUnit);
                enemyHealth = gameObject.GetComponent<EnemyHealth>().getHealth();

            }

            yield return null;
        }

        //check healths
        if (enemyHealth <= 0f)
        {
            Debug.Log(gameObject.name + " has died");
            gameObject.GetComponent<EnemyAppearance>().hideAllSprites();
            gameObject.SetActive(false);

            //in case they kill each other
            if (allyHealth <= 0f)
            {
                exitCombatCycle(true, allyUnit);
            }
            else
            {
                exitCombatCycle(false, allyUnit);
            }
        }
        if (allyHealth <= 0f)
        {
            Debug.Log(allyUnit.name + " has died");
            setAllyDeath(allyUnit);

            exitCombatCycle(true, allyUnit);
        }
    }

    public void exitCombatCycle(bool isAllyDead, GameObject allyUnit)
    {
        inCombat = false;
        numCombatInstances--;
        if (numCombatInstances < 0) { numCombatInstances = 0; }

        if (!isAllyDead)
        {
            resetAllyButtonColor(allyUnit);
        }

        if (isUnitCombating(allyUnit))
        {
            combatingUnits.Remove(allyUnit);
        }

    }

    private void setAllyDeath(GameObject unit)
    {
        Debug.Log("Unit to kill " + unit);
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

    //private void updateHealthTextInButton(string buttonName, string unitName, GameObject unit)
    //{
    //    //this section modifies the name to fit the corresponding GameObject name

    //    buttonName = buttonName.Remove(0, 6); //removes "Allied" from string
    //    //removes "Unit" from string
    //    if (buttonName.Length == 10) { buttonName = buttonName.Remove(5, 4); }//if the unit is an armor type
    //    else { buttonName = buttonName.Remove(8, 4); }//if the unit is an infantry type
    //    buttonName += "Button";

    //    Debug.Log("setting ally health to " + Math.Ceiling(GetComponent<AllyUnits>().getHealth(unit)));
    //    GameObject.Find(buttonName).GetComponentInChildren<TextMeshProUGUI>().text = unitName + "\n\nHealth:\n" +
    //                                                (int)Math.Ceiling(GetComponent<AllyUnits>().getHealth(unit));

    //}

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

    private Color getButtonColor(GameObject unit)
    {
        //formats the game object name to match the button name
        string buttonName = unit.name;
        buttonName = buttonName.Remove(0, 6); //removes "Allied" from string
        //removes "Unit" from string
        if (buttonName.Length == 10) { buttonName = buttonName.Remove(5, 4); }//if the unit is an armor type
        else { buttonName = buttonName.Remove(8, 4); }//if the unit is an infantry type
        buttonName += "Button";

        Image button = GameObject.Find(buttonName).GetComponent<Image>();
        return button.color;
    }

    private bool isUnitCombating(GameObject unit)
    {
        foreach (GameObject ally in combatingUnits)
        {
            if (ally.name == unit.name) { return true; }
        }
        return false;
    }

}
