using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAIWander : EntityAITask {

  private Vector3 target;
  private GameObject entity;
  private bool targetInitiallyPassed = false;

  public EntityAIWander(GameObject entity) {
    this.entity = entity;
    SetMutexBits(1);
  }

  public override bool CanExecute() {
    return RandomUtils.RandomInt(120) == 0;
  }

  public override bool ShouldContinueExecuting() {
    return entity.transform.position.x > target.x ^ targetInitiallyPassed;
  }

  public override void StartExecution() {
    entity.transform.LookAt(target);
    Vector3 rotation = entity.transform.localEulerAngles;
    rotation.x = 0;
    rotation.y += 180;
    rotation.y %= 360;
    entity.transform.localEulerAngles = rotation;
  }

  public override void UpdateExecution() {
    entity.transform.position = entity.transform.position - entity.transform.forward * 0.1f;

    RaycastHit intersection;
    if (Physics.Linecast(entity.transform.position + new Vector3(0f, 2f, 0f), entity.transform.position + new Vector3(0f, -1000f, 0f), out intersection))
      entity.transform.position = intersection.point;
  }

  public override void ResetTask() {
    target = RandomUtils.RandomPoint(entity, 10);
    targetInitiallyPassed = entity.transform.position.x < target.x;
  }

  public override bool IsInterruptible() {
    return true;
  }
}
