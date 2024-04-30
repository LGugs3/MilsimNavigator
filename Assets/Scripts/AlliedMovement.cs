using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class AlliedMovement : MonoBehaviour
{
    Vector3 newPosition;
    Rigidbody2D rigidbody2d;

    [SerializeField] private GameObject projectilePrefab;
    public InputAction LeftClick;
    private MousePosition mousePos;

    float cooldown;
    const float maxCooldown = 0.2f;
    bool isCooldown;
    
    [SerializeField]float speed;
    GameObject[] currentMovingUnits;
    GameObject[] tempArr;
    bool isMoving;
    GameObject moveUnit;

    [SerializeField] float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        mousePos = GameObject.FindGameObjectWithTag("MousePosition").GetComponent<MousePosition>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        LeftClick.Enable();

        currentMovingUnits = new GameObject[1];//cannot initialize array as empty
        currentMovingUnits[0] = projectilePrefab;//junk data

        cooldown = 0.0f;
        isCooldown = false;
        isMoving = true;

    }

    // Update is called once per frame
    void Update()
    {
        
        if (isCooldown) { cooldown -= Time.deltaTime; }

        if (LeftClick.IsPressed() && !isCooldown)
        {
            cooldown = maxCooldown;
            isCooldown = true;

            isMoving = false;
            moveUnit = SelectUnit.Instance.getSelectedMoveUnit();
            for (int i = 0; i < currentMovingUnits.Length; i++)
            {
                if (moveUnit == currentMovingUnits[i])
                { 
                    isMoving = true;
                }
            }
            if (!isMoving)
            {
                Launch();
            }
            
        }

        if (cooldown < 0) { isCooldown = false; }



    }

    void Launch()
    {
        
        if (moveUnit == null) { return; }

        for (int i = 0; i < currentMovingUnits.Length; i++)
        {
            if (moveUnit.name == currentMovingUnits[i].name) { return; }//if the unit that is requesting to move is currently in motion
        }


        //creates a temp array to store old gameobject data
        tempArr = new GameObject[currentMovingUnits.Length];
        Array.Copy(currentMovingUnits, tempArr, currentMovingUnits.Length);

        //recreates the old array one size larger than last time and refills the array with the old data
        currentMovingUnits = new GameObject[tempArr.Length + 1];
        Array.Copy(tempArr, currentMovingUnits, tempArr.Length);

        //appends the new data to the end
        currentMovingUnits[currentMovingUnits.Length - 1] = moveUnit;

        //Clones NavEndpoint sprite and places it at world position where clicked
        GameObject EndSprite = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        newPosition = mousePos.getWorldPosition();
        EndSprite.transform.position = newPosition;

        //rotate sprite towards end point
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, (newPosition - moveUnit.transform.position));
        moveUnit.transform.rotation = Quaternion.RotateTowards(moveUnit.transform.rotation, targetRotation, rotationSpeed);

        //if selected unit exists
        Vector3 directionNormalized = (newPosition - moveUnit.transform.position).normalized;
        StartCoroutine(MoveUnit(moveUnit, EndSprite, newPosition, directionNormalized));

        


    }

    IEnumerator MoveUnit(GameObject unit, GameObject endPoint, Vector3 endPos, Vector3 direction)
    {
        for (int i = 0; i < currentMovingUnits.Length; i++)
        {
            if (unit == currentMovingUnits[i]) { yield return null; }//if the unit that is requesting to move is currently in motion
        }

        bool isXClose = false, isYClose = false;
        while (!(isXClose && isYClose))
        {
            //set new position
            unit.transform.position = new Vector3(unit.transform.position.x + speed * direction.x * Time.deltaTime,
                                                  unit.transform.position.y + speed * direction.y * Time.deltaTime,
                                                  unit.transform.position.z);

            //checks if the unit has reached its desintation
            if (unit.transform.position.x - endPos.x > -0.2f && unit.transform.position.x - endPos.x < 0.2f ) { isXClose = true; }
            if (unit.transform.position.y - endPos.y > -0.2f && unit.transform.position.y - endPos.y < 0.2f) { isYClose = true; }
            if (isXClose && isYClose)
            {
                //tempArr is one less to remove game object
                tempArr = new GameObject[currentMovingUnits.Length - 1];
                for (int i = 0; i < currentMovingUnits.Length; i++)
                {
                    if (currentMovingUnits[i] == unit)
                    {
                        //removes old value from array
                        Array.Copy(currentMovingUnits, 0, tempArr, 0, i);
                        Array.Copy(currentMovingUnits, i + 1, tempArr, i, currentMovingUnits.Length - i - 1);

                        //resetting array
                        currentMovingUnits = new GameObject[tempArr.Length];
                        Array.Copy(tempArr, currentMovingUnits, tempArr.Length);

                        //deselecting unit once it reaches its destination
                        SelectUnit.Instance.resetSelectedMoveUnit(unit);
                        break;
                    }
                }
            }

            yield return null;

        }
        //destroys Nav point
        Destroy(endPoint);
    }

}
