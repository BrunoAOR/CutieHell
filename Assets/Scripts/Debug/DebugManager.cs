﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour {

    [Header("Debug stats")]
    [SerializeField]
    private StatsDebug statsDebug;
    [SerializeField]
    private GameObject memoryUsage;

    [Space]
    [Header("Debug camera")]

    [SerializeField]
    private GameObject debugCameraCanvas;
    [SerializeField]
    private Text values;
    [SerializeField]
    private Image grid;

    [Space]
    [Header("World camera debug")]
    [SerializeField]
    private GameObject worldCamera;
    [SerializeField]
    private GameObject instructionsWorldCamera;

    private CameraController cameraController;
    private Player playerScript;
    private Camera worldCameraComponent;

    private bool showStats = false;
    private bool showCameraDebug = false;
    private bool showWorldCamera = false;
    private bool showGrid = false;
    private bool followPlayer = true;

    private GameObject player;

	void Start () {
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player = GameObject.FindGameObjectWithTag("Player");
        worldCameraComponent = worldCamera.GetComponent<Camera>();
    }

	void Update () {
        ProcessInput();
        ActivateDebug();
	}

    private void ProcessInput() 
    {
        if (Input.GetKeyDown(KeyCode.I)) 
        {
            showWorldCamera = !showWorldCamera;
            if (showCameraDebug) 
            {
                showCameraDebug = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.O)) 
        {
            showStats = !showStats;
        }
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            showCameraDebug = !showCameraDebug;
            if (showWorldCamera) 
            {
                showWorldCamera = false;
            }
        }
    }
    private void ActivateDebug() 
    {
        if(showStats) {
            statsDebug.Show_Stats = true;
            memoryUsage.SetActive(true);
        } else {
            statsDebug.Show_Stats = false;
            memoryUsage.SetActive(false);
        }
        if (showCameraDebug) {
            debugCameraCanvas.SetActive(true);
            DebugCamera();
        } else {
            debugCameraCanvas.SetActive(false);
        }
        if (showWorldCamera) {
            worldCamera.SetActive(true);
            instructionsWorldCamera.SetActive(true);
            WorldCameraDebug();
        } else {
            worldCamera.SetActive(false);
            instructionsWorldCamera.SetActive(false);
        }
    }
    private void DebugCamera() {
        switch (playerScript.cameraState) {
            case Player.CameraState.MOVE:
                if (Input.GetKeyDown(KeyCode.Z)) {
                    showGrid = !showGrid;
                }
                grid.gameObject.SetActive(showGrid);
                if (Input.GetKey(KeyCode.X)) {
                    cameraController.distance += Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.C)) {
                    cameraController.distance -= Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.V)) {
                    cameraController.cameraX -= Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.B)) {
                    cameraController.cameraX += Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.N)) {
                    cameraController.cameraY += Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.M)) {
                    cameraController.cameraY -= Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.S)) {
                    cameraController.fov += Time.deltaTime * 4f;
                }
                if (Input.GetKey(KeyCode.D)) {
                    cameraController.fov -= Time.deltaTime * 4f;
                }
                if (Input.GetKey(KeyCode.F)) {
                    cameraController.focusDistance += Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.G)) {
                    cameraController.focusDistance -= Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.H)) {
                    cameraController.focusX -= Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.J)) {
                    cameraController.focusX += Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.K)) {
                    cameraController.focusY += Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.L)) {
                    cameraController.focusY -= Time.deltaTime * 0.5f;
                }
                if (Input.GetKeyDown(KeyCode.Return)) {
                    cameraController.distance = 3.0f;
                    cameraController.cameraX = 0.5f;
                    cameraController.cameraY = 1.75f;
                    cameraController.focusDistance = 0.4f;
                    cameraController.focusX = 0.3f;
                    cameraController.focusY = 1.7f;
                    cameraController.fov = 60;
                    showGrid = false;
                }
                values.text = "Distance : " + cameraController.distance + "\nCameraX : " + 
                    cameraController.cameraX + "\nCameraY : " + cameraController.cameraY + "\nFocus Distance" +
                    cameraController.focusDistance + "\nFocusX : " + cameraController.focusX + "\nFocusY : " + cameraController.focusY
                    + "\nFOV : " + cameraController.fov;
                break;
            case Player.CameraState.TURRET:
                if (Input.GetKeyDown(KeyCode.Z)) {
                    showGrid = !showGrid;
                }
                grid.gameObject.SetActive(showGrid);
                if (Input.GetKey(KeyCode.X)) {
                    cameraController.t_distance += Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.C)) {
                    cameraController.t_distance -= Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.N)) {
                    cameraController.t_cameraY += Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.M)) {
                    cameraController.t_cameraY -= Time.deltaTime * 0.5f;
                }
                if (Input.GetKey(KeyCode.S)) {
                    cameraController.t_fov += Time.deltaTime * 4f;
                }
                if (Input.GetKey(KeyCode.D)) {
                    cameraController.t_fov -= Time.deltaTime * 4f;
                }
                if (Input.GetKeyDown(KeyCode.Return)) {
                    cameraController.t_distance = 3.0f;
                    cameraController.t_cameraY = 1.75f;
                    cameraController.t_fov = 40;
                    showGrid = false;
                }
                values.text = "Distance : " + cameraController.t_distance + "\nCameraY : " + cameraController.t_cameraY +
                    "\nFOV : " + cameraController.t_fov;
                break;
        }
    }

    private void WorldCameraDebug() {

        if (Input.GetKeyDown(KeyCode.C)) followPlayer = !followPlayer;

        if (Input.GetKey(KeyCode.Z)) {
            if (worldCameraComponent.orthographicSize > 1) {
                worldCameraComponent.orthographicSize -= Time.deltaTime * 7;
            }
        }
        if (Input.GetKey(KeyCode.X)) {
            if (worldCameraComponent.orthographicSize < 50) {
                worldCameraComponent.orthographicSize += Time.deltaTime * 7;
            }
        }

        if (followPlayer) {
            worldCamera.transform.position = player.transform.position + Vector3.up * 50;
        } else {
            if (Input.GetKey(KeyCode.UpArrow)) {
                worldCamera.transform.Translate(Vector3.up * Time.deltaTime * 10);
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                worldCamera.transform.Translate(Vector3.up * Time.deltaTime * -10);
            }
            if (Input.GetKey(KeyCode.LeftArrow)) {
                worldCamera.transform.Translate(Vector3.left * Time.deltaTime * 10);
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                worldCamera.transform.Translate(Vector3.left * Time.deltaTime * -10);
            }
        }
    }
}
