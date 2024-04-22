using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health;

    public int getHealth()
    {
        return health;
    }

    public void changeHealth(int amount)
    {
        health += amount;
    }

    public void setHealth(int amount)
    {
        health = amount;
    }
}
