using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyDetection : MonoBehaviour
{
    [SerializeField] float engageDistance, bufferDistance;
    EnemyMovement enemyMovement;
    GameObject enemyToChase;

    float speed;
    float rotationSpeed = 720;

    private void Start()
    {
        enemyMovement = gameObject.GetComponent<EnemyMovement>();
        
        speed = enemyMovement.getSpeed();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the enemy unit is already chasing a different ally
        if (enemyMovement.getIsChasingAlly()) { return; }

        if (collision.CompareTag("AllyUnit"))
        {
            enemyToChase = collision.gameObject;
            enemyMovement.setAllyFound(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!enemyMovement.getIsChasingAlly()) { return; }//prevents collider from checking enemy unit while there is no ally unit in collision box
        if (collision.gameObject.name == enemyToChase.name)
        {            
            float distance = Vector3.Distance(enemyToChase.transform.position, gameObject.transform.position);

            //rotate sprite towards end point; the ally sprite can move within the enemy engage distance
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, (enemyToChase.gameObject.transform.position - gameObject.transform.position));
            gameObject.GetComponent<EnemyMovement>().setLookDirection(targetRotation, rotationSpeed);

            if (distance > engageDistance - bufferDistance)
            {
                moveToAlly(enemyToChase.gameObject);
            }
            else
            {
                if (!gameObject.GetComponent<Combat>().getInCombat())
                {
                    Debug.Log("Stating combat");
                    gameObject.GetComponent<Combat>().beginCombatCycle();

                }
            }

            if (distance > engageDistance)
            {
                Debug.Log("Out of range");
                gameObject.GetComponent<Combat>().exitCombatCycle(false, enemyToChase);
            }
            

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!enemyMovement.getIsChasingAlly()) { return; }//prevents collider from checking enemy unit while there is no ally unit in collision box
        if (collision.gameObject.name == enemyToChase.name)
        {
            //enemy unit lost track of ally unit
            enemyMovement.setAllyFound(false);

        }
    }

    private void moveToAlly(GameObject allyUnit)
    {
        float distance = Vector3.Distance(allyUnit.transform.position, gameObject.transform.position);
        Vector3 direction = (allyUnit.transform.position - gameObject.transform.position).normalized;

        distance = Vector3.Distance(allyUnit.transform.position, gameObject.transform.position);

        gameObject.transform.position = new Vector3(gameObject.transform.position.x + speed * direction.x * Time.deltaTime,
                                                gameObject.transform.position.y + speed * direction.y * Time.deltaTime,
                                                gameObject.transform.position.z);
    }

    public GameObject getChasingAlly()
    {
        return enemyToChase;
    }
}



