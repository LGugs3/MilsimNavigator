using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectiveAirstrikeRadius : MonoBehaviour
{
    private List<GameObject> targetsInRange;

    private void Start()
    {
        targetsInRange = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetsInRange.Count == 0 )
        {
            targetsInRange.Add(collision.gameObject);
        }
        else
        {
            int index = targetsInRange.IndexOf(collision.gameObject);
            if (index == -1)
            {
                Debug.Log("Enter found unit " + collision.gameObject.name);
                targetsInRange.Add(collision.gameObject);
            }
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (targetsInRange.Count == 0)
        {
            targetsInRange.Add(collision.gameObject);
        }
        else
        {
            int index = targetsInRange.IndexOf(collision.gameObject);
            if (index == -1)
            {
                Debug.Log("Stay found unit " + collision.gameObject.name);
                targetsInRange.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (targetsInRange.IndexOf(collision.gameObject) != -1)
        {
            Debug.Log(collision.gameObject.name + " has left effective radius");
            targetsInRange.Remove(collision.gameObject);
        }
    }

    public void excuteAttack(float damage)
    {
        List<GameObject> tmpList = new List<GameObject>();

        if (!targetsInRange.Any()) { return; }
        foreach (GameObject target in targetsInRange)
        {
            Debug.Log(target.name + " has taken " + damage + " damage");
            target.GetComponent<EnemyHealth>().changeHealth(damage);

            if (target.GetComponent<EnemyHealth>().getHealth() <= 0)
            {
                tmpList.Add(target);
            }
        }

        foreach(GameObject target in tmpList)
        {
            targetsInRange.Remove(target);
            target.GetComponent<EnemyAppearance>().hideAllSprites();
            target.SetActive(false);
        }
    }
}
