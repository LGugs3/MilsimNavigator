using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

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

        strafeDamage = 5f;
        lightDamage = 15f;
        heavyDamage = 40f;

        strafeCooldown = 0f;
        heavyCooldown = 0f;
        lightCooldown = 0f;

        maxStrafeCooldown = 20f;
        maxLightCooldown = 30f;
        maxHeavyCooldown = 60f;

        cooldown = 0f;

        airstrikeInProgress = false;
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

            GameObject newTargetMarker = Instantiate(originalTargetMarker, Vector2.up * 0.5f, Quaternion.identity);
            attackPos = mousePos.getWorldPosition();
            newTargetMarker.transform.position = attackPos;

            if (getClosestSAMTurretDistance() < SAMTurretRange)
            {
                executeFailedAirstrike();
                return;
            }

            airstrikeInProgress = true;
            switch (airSelection)
            {
                case 0:
                    Destroy(newTargetMarker);
                    return;
                case 1:
                    strafeAttack(newTargetMarker);
                    break;
                case 2:
                    HeavyAttack(newTargetMarker);
                    break;
                case 3:
                    LightAttack(newTargetMarker);
                    break;
            }
        }

        if (strafeCooldown > 0f) { strafeCooldown -= Time.deltaTime; }
        if (heavyCooldown > 0f) { heavyCooldown -= Time.deltaTime; }
        if (lightCooldown > 0f) { lightCooldown -= Time.deltaTime; }
        if (cooldown > 0f) { cooldown -= Time.deltaTime; }
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

    private void executeSuccessfulAirstrike()
    {
        float damage = 0f;
        switch (airSelection)
        {
            case 0:
                return;
            case 1:
                damage = strafeDamage;
                break;
            case 2:
                damage = lightDamage;
                break;
            case 3:
                damage = heavyDamage;
                break;
        }

        GetComponent<EffectiveAirstrikeRadius>().excuteAttack(damage);
        airstrikeInProgress = false;
    }

    public void selectStrafeAttack() { airSelection = 1; }

    public void selectHeavyAttack() {  airSelection = 2; }

    public void selectLightAttack() { airSelection = 3; }

    public void selectRecon() { airSelection= 4; }

    private void strafeAttack(GameObject marker)
    {
        if (strafeCooldown > 0f) { return; }

        float strafeRadius = 3f;
        marker.GetComponent<CircleCollider2D>().radius = strafeRadius;

        
    

    airUnit.transform.position = new Vector3(marker.transform.position.x - 20f, marker.transform.position.y);
        showAirUnit();


        StartCoroutine(moveAirUnit(marker));

    }

    private void HeavyAttack(GameObject marker)
    {
        if (heavyCooldown > 0f) { return; }

        float heavyRadius = 1f;
        marker.GetComponent<CircleCollider2D>().radius = heavyRadius;

        airUnit.transform.position = new Vector3(marker.transform.position.x - 20f, marker.transform.position.y);
        showAirUnit();

        StartCoroutine(moveAirUnit(marker));
    }

    private void LightAttack(GameObject marker)
    {
        if (lightCooldown > 0f) { return; }

        float lightRadius = 2f;
        marker.GetComponent<CircleCollider2D>().radius = lightRadius;

        airUnit.transform.position = new Vector3(marker.transform.position.x - 20f, marker.transform.position.y, marker.transform.position.z);
        showAirUnit();

        StartCoroutine(moveAirUnit(marker));
    }

    IEnumerator moveAirUnit(GameObject marker)
    {
        float speed = 5f;

        bool isXClose = false;
        while (!isXClose)
        {
            airUnit.transform.position = new Vector3(airUnit.transform.position.x + speed * Time.deltaTime, marker.transform.position.y, marker.transform.position.z);

            if (Vector3.Distance(airUnit.transform.position, marker.transform.position) < 0.5f) { executeSuccessfulAirstrike(); }

            if (airUnit.transform.position.x - (marker.transform.position.x + 20f) < 1f && airUnit.transform.position.x - (marker.transform.position.x + 20f) > -1f) { isXClose = true; }

            yield return null;
        }
        hideAirUnit();
        Destroy(marker);
    }

    private void hideAirUnit()
    {
        airUnit.GetComponent<Renderer>().enabled = false;
    }

    private void showAirUnit()
    {
        airUnit.GetComponent<Renderer>().enabled = true;
    }
}
