using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class PowerUp : MonoBehaviour
{
    private Transform transform;
    public float speed = 100.0f;

    void Start()
    {
        transform = this.gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        transform.rotation *= Quaternion.Euler(0.0f, speed * Time.deltaTime, 0.0f);
    }

}