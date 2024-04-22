using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class NavEndPoint : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    GameObject projectilePrefab;


    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

}
