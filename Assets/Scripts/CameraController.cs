using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

  private SpawnerController spawner;
  private RectTransform canvasDims;
  private GameObject debugText;
  public GameObject camera;
  public GameObject fireball;

  public float jumpSpeed;
  public float origin_speed;
  public int lineRange;
  public float sprintMultiplier;
  private float speed;

  private Quaternion lockRot;
  private Vector3 lookingAt;
  private Vector3Int lookingAtBlock;
  private int lookingAtBlockFace;
  private float startedLookingAtBlockTime;
  private bool isLookingAtBlock;
  private bool hasTriggered = false;
  private Rigidbody rb;
  private bool isGrounded;
  public GameObject player;
  private AudioSource walk;
  private int walkTime;

  private float cameraWidth, cameraHeight;

  void Start() {
    GameObject spawnerObject = GameObject.Find("WorldSpawner");
    spawner = spawnerObject.GetComponent<SpawnerController>();

    canvasDims = GameObject.Find("Canvas").GetComponent<RectTransform>();
    debugText = GameObject.Find("Debug Text");

    lockRot = Quaternion.Euler(0f, 0f, 0f);
    rb = GetComponent<Rigidbody>();

    speed = origin_speed;
    walk = player.GetComponent<AudioSource>();
  }

  void OnBlockLookedAtTimeout() {
    int[] chunkCoords = spawner.GenerateBlockAt(lookingAtBlock.x, lookingAtBlock.y, lookingAtBlock.z, /*type = */3);
    spawner.UpdateChunk(chunkCoords[0], chunkCoords[1]);
  }

  void OnBlockLookedAt() {
    hasTriggered = false;
    startedLookingAtBlockTime = Time.time;
  }

  void OnBlockLookedAway() {
    hasTriggered = false;
  }

  string FacingToName(int facing) {
    int facingAxis = (int) Mathf.Abs(facing);
    string facingName;
    if (facing < 0) facingName = "Negative ";
    else facingName = "Positive ";

    string[] axesNames = new string[] {"", "X", "Y", "Z"};
    facingName += axesNames[facingAxis];

    return facingName;
  }

  void OnGUI() {
    if (cameraWidth != canvasDims.rect.width || cameraHeight != canvasDims.rect.height) {
      cameraWidth = canvasDims.rect.width;
      cameraHeight = canvasDims.rect.height;

      RectTransform debugTextTransform = debugText.GetComponent<RectTransform>();
      debugTextTransform.anchoredPosition = new Vector2(
      -cameraWidth / 2 + debugTextTransform.rect.width / 2 + 5,
      cameraHeight / 2 - debugTextTransform.rect.height / 2 - 5);
    }

    int facing = GetFacing(transform.GetChild(0).forward);

    debugText.GetComponent<Text>().text =
      "Player position: " + transform.position + "\n" +
      "Player facing: " + FacingToName(facing) + "\n" +
      "Looking at: " + (isLookingAtBlock ? lookingAt + "" : "null") + "\n" +
      "Looking at block: " + (isLookingAtBlock ? lookingAtBlock + "" : "null") + "\n" +
      "Looking at block face: " + (isLookingAtBlock ? FacingToName(lookingAtBlockFace) : "null");
  }

  /*
  1: x axis
  2: y axis
  3: z axis
  negative for negative dir of axis
  */
  int GetFacing(Vector3 forward) {
    float xFacing = forward.x,
          yFacing = forward.y,
          zFacing = forward.z;

    float absXFacing = Mathf.Abs(xFacing),
          absYFacing = Mathf.Abs(yFacing),
          absZFacing = Mathf.Abs(zFacing);

    int[] facingsDirections = new int[] {1, 2, 3};
    float[] absFacings = new float[] {absXFacing, absYFacing, absZFacing};
    float[] facings = new float[] {xFacing, yFacing, zFacing};

    Array.Sort(absFacings, facingsDirections);

    int facing = facingsDirections[2];
    return facing * (facings[facing - 1] < 0 ? -1 : 1);
  }

  //returns the axis # of the face being looked at on that block.

  int GetBlockFacing(Vector3 differences) {
    float xDiff = differences.x,
          yDiff = differences.y,
          zDiff = differences.z;

    int facing = 0;

    if (xDiff == 0) facing = -1;
    if (xDiff == 1) facing = +1;
    if (yDiff == 0) facing = -2;
    if (yDiff == 1) facing = +2;
    if (zDiff == 0) facing = -3;
    if (zDiff == 1) facing = +3;

    return facing;
  }

  Vector3 zero = new Vector3(0,0,0);

  void Update()
  {
    transform.localRotation = lockRot;
    Vector3 right = camera.transform.right;
    float cameraY = camera.transform.localRotation.eulerAngles.y * 3.14159265f / 180f;
    Vector3 forward = new Vector3(Mathf.Sin(cameraY), 0, Mathf.Cos(cameraY));
    //Debug.Log("forward: " + forward);
    forward.Normalize();

    speed = Input.GetButton("Fire1") ? origin_speed * sprintMultiplier : origin_speed;
    Vector3 newPosition = transform.position + forward * speed * Input.GetAxis("Vertical");
    newPosition += right * speed * Input.GetAxis("Horizontal");

    newPosition.x = Mathf.Min(128, Mathf.Max(0, newPosition.x));
    newPosition.z = Mathf.Min(128, Mathf.Max(0, newPosition.z));

    RaycastHit intersection;
    if (Physics.Linecast(newPosition, newPosition + new Vector3(0f, -1000f, 0f), out intersection)) {
      if (newPosition.y - 1 < intersection.point.y) {
        newPosition.y = intersection.point.y + 1;
      }
    }

    if (transform.position - newPosition != zero) {
      Debug.Log(walkTime % 20);
      if (++walkTime % 20 == 0) {
        walk.Play();
      }
    }

    transform.position = newPosition;

    Vector3 cameraPosition = camera.transform.position;
    isLookingAtBlock = Physics.Linecast(cameraPosition + camera.transform.forward, cameraPosition + camera.transform.forward * 5f, out intersection);

    RaycastHit groundHit;
    if (Physics.Linecast(transform.position, transform.position - new Vector3(0f, 1000f, 0f), out groundHit))
    {
        // Debug.Log("Raycast hit at " + hit.point);
        isGrounded = transform.position.y <= groundHit.point.y + 1;
    }
    else Debug.Log("Raycast failed");

    if (Input.GetButtonDown("Fire4")) Magic_Teleport(); // Square button
    if (Input.GetButtonDown("Fire5")) Magic_Fireball(); // Triangle button
  }

  /*
  void Magic_LineOfBlocks()
  {
    RaycastHit line;
    if (Physics.Linecast(camera.transform.position, camera.transform.forward * lineRange, out line))
    {
      Debug.Log(line.point);
    }
  }
  */

  void Magic_Teleport()
  {
    RaycastHit checkForBlockHit;
    if (Physics.Raycast(camera.transform.position, camera.transform.forward, out checkForBlockHit, lineRange, 3))
    {
      transform.position = checkForBlockHit.point + new Vector3(0f, 1f, 0f);
    }
    else transform.position += camera.transform.forward * lineRange;
  }

  void Magic_Fireball()
  {
    GameObject fb = Instantiate(fireball, transform.position + camera.transform.forward, Quaternion.identity);
    Destroy(fb, 5);
    fb.GetComponent<FireballLook>().dir = transform.forward;
  }

  void FixedUpdate() //Rigidbody code goes here
  {
    if (Input.GetButton("Jump") && isGrounded) {
            rb.velocity = new Vector3(0f, jumpSpeed, 0f);
    }
  }

  void OnCollisionEnter(Collider col)
  {

  }
}
