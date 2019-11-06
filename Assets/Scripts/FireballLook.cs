using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballLook : MonoBehaviour
{
  public GameObject player;
  public Vector3 dir;
  public float speed = 1f;

  void Update()
  {
    transform.rotation = Quaternion.LookRotation(transform.position - player.transform.position);
    transform.Translate(dir * speed);
  }
}
