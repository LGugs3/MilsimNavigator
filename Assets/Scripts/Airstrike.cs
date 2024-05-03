using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class Airstrike : MonoBehaviour
{
    private static float strafeDamage, heavyDamage, lightDamage,
                         strafeCooldown, heavyCooldown, lightCooldown,
                         maxStrafeCooldown, maxHeavyCooldown, maxLightCooldown;

    private uint airSelection;

    private float SAMTurretRange;

    public GameObject originalTargetMarker;
    Vector3 attackPos;
    private MousePosition mousePos;
    public InputAction RightClick, LeftClick;

    public GameObject airUnit;

    private GameObject[] SAMTurrets;

    public static Airstrike Instance;

    private float cooldown;

    private bool airstrikeInProgress;

    private void Start()
    {
        Instance = this;
        
        airSelection = 0;

        RightClick.Enable();
        LeftClick.Enable();

        mousePos = GameObject.FindGameObjectWithTag("MousePosition").GetComponent<MousePosition>();

        SAMTurrets = GameObject.FindGameObjectsWithTag("SAMTurret");
        SAMTurretRange = 10f;

        strafeDamage = -5f;
        lightDamage = -15f;
        heavyDamage = -40f;

        strafeCooldown = 0f;
        heavyCooldown = 0f;
        lightCooldown = 0f;

        maxStrafeCooldown = 20f;
        maxLightCooldown = 30f;
        maxHeavyCooldown = 60f;

        cooldown = 0f;

        airstrikeInProgress = false;
        originalTargetMarker.SetActive(false);
    }

    private void Update()
    {
        if (RightClick.IsPressed()) { airSelection = 0; }

        if (LeftClick.IsPressed() && airSelection > 0 && cooldown <= 0f && !airstrikeInProgress)
        {
            cooldown = 0.5f;
            //reget sam turrets still alive before attacking
            Array.Clear(SAMTurrets, 0, SAMTurrets.Length);
            SAMTurrets = GameObject.FindGameObjectsWithTag("SAMTurret");

            attackPos = mousePos.getWorldPosition();
            originalTargetMarker.transform.position = attackPos;
            showMarker(originalTargetMarker);

            if (getClosestSAMTurretDistance() < SAMTurretRange)
            {
                executeFailedAirstrike();
                deselectOption();
                return;
            }

            airstrikeInProgress = true;
            switch (airSelection)
            {
                case 0:
                    hideMarker(originalTargetMarker);
                    airstrikeInProgress = false;
                    return;
                case 1:
                    strafeAttack(originalTargetMarker);
                    break;
                case 2:
                    HeavyAttack(originalTargetMarker);
                    break;
                case 3:
                    LightAttack(originalTargetMarker);
                    break;
            }
            deselectOption();
        }

        if (strafeCooldown > 0f) { strafeCooldown -= Time.deltaTime; }
        if (heavyCooldown > 0f) { heavyCooldown -= Time.deltaTime; }
        if (lightCooldown > 0f) { lightCooldown -= Time.deltaTime; }
        if (cooldown > 0f) { cooldown -= Time.deltaTime; }

        updateCooldownTextInButton();
    }

    private float getClosestSAMTurretDistance()
    {
        float closestSAM = SAMTurretRange + 1f, comparingDist;

        int i;
        for (i = 0; i < SAMTurrets.Length; i++)
        {
                comparingDist = Vector3.Distance(attackPos, SAMTurrets[i].transform.position);
                if (comparingDist < closestSAM)
                {
                    closestSAM = comparingDist;
                }
        }

        return closestSAM;
    }

    private void executeFailedAirstrike()
    {

    }

    private void executeSuccessfulAirstrike(GameObject marker, int attackType)
    {
        float damage = 0f;
        switch (attackType)
        {
            case 0:
                return;
            case 1:
                damage = strafeDamage;
                break;
            case 2:
                damage = heavyDamage;
                break;
            case 3:
                damage = lightDamage;
                break;
        }

        marker.GetComponent<EffectiveAirstrikeRadius>().excuteAttack(damage);
    }

    public void selectStrafeAttack() { airSelection = 1; }

    public void selectHeavyAttack() {  airSelection = 2; }

    public void selectLightAttack() { airSelection = 3; }

    public void selectRecon() { airSelection= 4; }

    public void deselectOption() { airSelection = 0; }

    private void strafeAttack(GameObject marker)
    {
        if (strafeCooldown > 0f) { return; }
        strafeCooldown = maxStrafeCooldown;

        float strafeRadius = 5f;
        marker.GetComponent<CircleCollider2D>().radius = strafeRadius;

        airUnit.transform.position = new Vector3(marker.transform.position.x - 20f, marker.transform.position.y);
        showAirUnit();


        StartCoroutine(moveAirUnit(marker, 1));

    }

    private void HeavyAttack(GameObject marker)
    {
        if (heavyCooldown > 0f) { return; }
        heavyCooldown = maxHeavyCooldown;

        float heavyRadius = 2f;
        marker.GetComponent<CircleCollider2D>().radius = heavyRadius;

        airUnit.transform.position = new Vector3(marker.transform.position.x - 20f, marker.transform.position.y);
        showAirUnit();

        StartCoroutine(moveAirUnit(marker, 2));
    }

    private void LightAttack(GameObject marker)
    {
        if (lightCooldown > 0f) { return; }
        lightCooldown = maxLightCooldown;

        float lightRadius = 3f;
        marker.GetComponent<CircleCollider2D>().radius = lightRadius;

        airUnit.transform.position = new Vector3(marker.transform.position.x - 20f, marker.transform.position.y, marker.transform.position.z);
        showAirUnit();

        StartCoroutine(moveAirUnit(marker, 3));
    }

    IEnumerator moveAirUnit(GameObject marker, int attackType)
    {
        float speed = 5f;

        bool isXClose = false, attackExecuted = false;
        while (!isXClose)
        {
            airUnit.transform.position = new Vector3(airUnit.transform.position.x + speed * Time.deltaTime, marker.transform.position.y, marker.transform.position.z);

            if (Vector3.Distance(airUnit.transform.position, marker.transform.position) < 0.5f)
            {
                if (!attackExecuted)
                {
                    attackExecuted = true;
                    executeSuccessfulAirstrike(marker, attackType);
                }
            }

            if (airUnit.transform.position.x - (marker.transform.position.x + 20f) < 1f && airUnit.transform.position.x - (marker.transform.position.x + 20f) > -1f) { isXClose = true; }

            yield return null;
        }
        hideAirUnit();
        hideMarker(marker);
        airstrikeInProgress = false;
    }

    private void hideAirUnit()
    {
        airUnit.GetComponent<Renderer>().enabled = false;
    }

    private void showAirUnit()
    {
        airUnit.GetComponent<Renderer>().enabled = true;
    }

    private void hideMarker(GameObject marker)
    {
        marker.SetActive(false);
    }

    private void showMarker(GameObject marker)
    {
        originalTargetMarker.SetActive(true);
    }

    private void updateCooldownTextInButton()
    {
        GameObject parentObject;
        Button button;

        /*****************************************************************************************************************************
         * Strafe option
         *****************************************************************************************************************************/
        if (strafeCooldown > 0f)
        {
            parentObject = GameObject.Find("Strafe");
            button = parentObject.GetComponent<Button>();
            
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            button.colors = colors;
            
            parentObject.GetComponentInChildren<TextMeshProUGUI>().text = Math.Ceiling(strafeCooldown).ToString();
            button.interactable = false;
        }
        else
        {
            parentObject = GameObject.Find("Strafe");
            button = parentObject.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.green;
            button.colors = colors;

            parentObject = GameObject.Find("Strafe");
            parentObject.GetComponentInChildren<TextMeshProUGUI>().text = "Strafe";
            button.interactable = true;
        }

        /*****************************************************************************************************************************
         * Heavy option
         *****************************************************************************************************************************/
        if (!Mathf.Approximately(heavyCooldown, 0f) && !(heavyCooldown < 0f))
        {
            parentObject = GameObject.Find("Heavy");
            button = parentObject.GetComponent<Button>();

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            button.colors = colors;

            parentObject.GetComponentInChildren<TextMeshProUGUI>().text = Math.Ceiling(heavyCooldown).ToString();
            button.interactable = false;
        }
        else
        {
            parentObject = GameObject.Find("Heavy");
            button = parentObject.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.green;
            button.colors = colors;

            parentObject = GameObject.Find("Heavy");
            parentObject.GetComponentInChildren<TextMeshProUGUI>().text = "Heavy";
            button.interactable = true;
        }

        /*****************************************************************************************************************************
         * Light option
         *****************************************************************************************************************************/
        if (lightCooldown > 0f)
        {
            parentObject = GameObject.Find("Light");
            button = parentObject.GetComponent<Button>();
            
            GameObject.Find("Light").GetComponentInChildren<TextMeshProUGUI>().text = Math.Ceiling(lightCooldown).ToString();
            button.interactable = false;
        }
        else
        {
            parentObject = GameObject.Find("Light");
            button = parentObject.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.green;
            button.colors = colors;

            parentObject = GameObject.Find("Light");
            parentObject.GetComponentInChildren<TextMeshProUGUI>().text = "Light";
            button.interactable = true;
        }
    }
}
