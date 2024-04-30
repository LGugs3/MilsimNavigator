using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectiveAirstrikeRadius : MonoBehaviour
{
    List<GameObject> targetsInRange;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enter triggered");
        Debug.Log(collision.gameObject);
        int index = targetsInRange.IndexOf(collision.gameObject);
        if (index == -1)
        {
            Debug.Log("Enter found unit " + collision.gameObject.name);
            targetsInRange.Add(collision.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("exit triggered");

        Debug.Log(collision.gameObject);
        int index = targetsInRange.IndexOf(collision.gameObject);
        if (index  == -1)
        {
            Debug.Log("Stay found unit " + collision.gameObject.name);
            targetsInRange.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        targetsInRange.Remove(collision.gameObject);
    }

    public void excuteAttack(float damage)
    {
        Debug.Log("Damage done");
        foreach (GameObject target in targetsInRange)
        {
            Debug.Log(target.name);
            GetComponent<AllyUnits>().changeHealth(target, damage);
        }
    }
}
