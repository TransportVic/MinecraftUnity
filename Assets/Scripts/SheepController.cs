using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour {

  private Vector3 target;

  private bool targetXPassed = false;
  private EntityAITasks tasks = new EntityAITasks();

  void Start() {
    transform.position = new Vector3(Random.value * 128, 65, Random.value * 128);
    ResetTarget();
  }

  void ResetTarget() {
    target = new Vector3(Random.value * 128, 65, Random.value * 128);
    targetXPassed = transform.position.x < target.x;

    transform.LookAt(target);
    Vector3 rotation = transform.localEulerAngles;
    rotation.x = 0;
    rotation.y += 180;
    rotation.y %= 360;
    transform.localEulerAngles = rotation;
  }

  void Move() {
    transform.position = transform.position - transform.forward * 0.05f;
    if (transform.position.x < target.x ^ targetXPassed)
      ResetTarget();
  }

  void Update() {
    Move();

    RaycastHit intersection;
    if (Physics.Linecast(transform.position + new Vector3(0f, 2f, 0f), transform.position + new Vector3(0f, -1000f, 0f), out intersection))
      transform.position = intersection.point;

    tasks.Update();
  }
}
