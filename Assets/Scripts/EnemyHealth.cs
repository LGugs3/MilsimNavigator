using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float health;

    public float getHealth()
    {
        return health;
    }

    public void changeHealth(float amount)
    {
        health += amount;
    }

    public void setHealth(float amount)
    {
        health = amount;
    }
}
