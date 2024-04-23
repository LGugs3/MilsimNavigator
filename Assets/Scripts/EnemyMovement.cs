using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float maxPatrolTime;
    float patrolTime;
    [SerializeField] float speed;
    bool allyFound, chasingAlly;
    private int corner;

    //Child sprite full and blip
    Transform fullSprite;
    Transform blipSprite;

    [SerializeField] float viewDistance;
    AllyUnits emptyUnit;

    bool nonMoving;

    private void Awake()
    {
        //find which full sprite is the child sprite
        Transform tmp;
        tmp = gameObject.transform.Find("Armor");
        if (tmp != null) { fullSprite = tmp; }
        else { fullSprite = gameObject.transform.Find("Infantry"); }
        nonMoving = false;

        //if the game object is an emplacement
        if (fullSprite == null)
        {
            fullSprite = gameObject.transform.Find("Artillery");
            nonMoving = true;
        }
        if (fullSprite == null)
        {
            fullSprite = gameObject.transform.Find("SAM");
        }

        //Blip object is always named the same
        blipSprite = gameObject.transform.Find("Blip");

        emptyUnit = gameObject.GetComponent<AllyUnits>();

        allyFound = false;
        chasingAlly = false;
        patrolTime = maxPatrolTime;
        corner = 0;
        if (!nonMoving) { changeDirection(); }
    }

    private void Update()
    {
        
        if (!allyFound && !nonMoving)
        {
            patrolTime -= Time.deltaTime;
            if (patrolTime <= 0)
            {
                corner++;
                if (corner > 3) { corner = 0; }

                
                patrolTime = maxPatrolTime;
            }
            if (!nonMoving) { changeDirection(); }
            patrolMove();//patrol movement
        }

    }

    private void changeDirection()
    {
        switch (corner)
        {
            //move to the right
            case 0:
                fullSprite.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                break;
            //move down
            case 1:
                fullSprite.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                break;
            //move left
            case 2:
                fullSprite.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                break;
            //move up
            case 3:
                fullSprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
        }
    }

    //patrol movement
    private void patrolMove()
    {
        switch (corner)
        {
            //move to the right
            case 0:
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime,
                                                transform.position.y,
                                                transform.position.z);

                break;
            //move down
            case 1:
                transform.position = new Vector3(transform.position.x,
                                                transform.position.y - speed * Time.deltaTime,
                                                transform.position.z);
                break;
            //move left
            case 2:
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime,
                                                transform.position.y,
                                                transform.position.z);
                break;
            case 3:
                transform.position = new Vector3(transform.position.x,
                                                transform.position.y + speed * Time.deltaTime,
                                                transform.position.z);
                break;
        }
    }

    public void setAllyFound(bool isAllyFound)
    {
        if (isAllyFound)
        {
            chasingAlly = true;
            allyFound = true;
        }
        else
        {
            patrolTime = maxPatrolTime;
            corner = 0;
            allyFound = false;
            chasingAlly = false;
        }
    }

    public float getSpeed()
    {
        return speed;
    }

    public bool getIsChasingAlly()
    {
        return chasingAlly;
    }

    public void setLookDirection(Quaternion targetRotation, float rotationSpeed)
    {
        if (nonMoving) { return; }
        fullSprite.transform.rotation = Quaternion.RotateTowards(fullSprite.transform.rotation, targetRotation, rotationSpeed);
    }

    public void setChasingAlly(bool chasing)
    {
        chasingAlly = chasing;
    }
}