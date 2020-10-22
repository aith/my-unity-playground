using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using LitJson;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour, ISaveable
{
    private string SAVE_ID = "player";

    // this is given in the inspector
    public Camera FirstPersonCam;
    public Camera ThirdPersonCam;
    public CharacterController controller;
    private Transform transform;
    private float moveSpeed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public int ActiveCam = 3;
    public Text scoreText;
    public int score = 0;
    public float localX = 0f;
    public float localY = 0f;
    public float localZ = 0f;
    public string LOCALX_KEY = "localX";
    public string LOCALY_KEY = "localY";
    public string LOCALZ_KEY = "localZ";

    public void Start()
    {
        transform = this.GetComponent<Transform>();
        ThirdPersonCam.enabled = false;
        FirstPersonCam.enabled = false;
        if (ActiveCam == 1) FirstPersonCam.enabled = true;
        if (ActiveCam == 3) ThirdPersonCam.enabled = true;
    }

    private void Update()
    {
        // GetAxisRaw doesn't smooth values. In this case I want to smooth it myself, for keyboard controls
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        // Normalize in order to account for multiple key presses
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (!Mathf.Approximately(direction.magnitude, 0f))
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            // TODO : inject a camera here
            if (ActiveCam == 1) targetAngle += FirstPersonCam.transform.eulerAngles.y;
            if (ActiveCam == 3) targetAngle += ThirdPersonCam.transform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
            var movDirVec = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(movDirVec * moveSpeed * Time.deltaTime);
        }
    }

    public void Grow()
    {
        transform.localScale += new Vector3(0.0f, 0.5f, 0.0f);
    }

    public void SwapCam()
    {
        if (ActiveCam == 1)
        {
            FirstPersonCam.enabled = false;
            ThirdPersonCam.enabled = true;
            ActiveCam = 3;
        }
        else
        {
            FirstPersonCam.enabled = true;
            ThirdPersonCam.enabled = false;
            ActiveCam = 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PowerUp"))
        {
            score++;
            scoreText.text = $"Score: {score}";
            Grow();
            Destroy(other.gameObject);
        }
    }

    public string SaveID
    {
        get { return SAVE_ID; }
    }

    public JsonData SavedData
    {
        get

        {
            var Transform = this.gameObject.GetComponent<Transform>();
            var data = new JsonData();
            //
            var position = Transform.position;
            data[LOCALX_KEY] = position.x;
            data[LOCALY_KEY] = position.y;
            data[LOCALZ_KEY] = position.z;

            return data;
        }
    }

    public void LoadFromData(JsonData data)
    {
        this.gameObject.transform.position = new Vector3(
            float.Parse(data[LOCALX_KEY].ToString()),
            float.Parse(data[LOCALY_KEY].ToString()),
            float.Parse(data[LOCALZ_KEY].ToString())
        );
    }
}