using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAILookIdle : EntityAITask {

  private GameObject entity;
  private int idleTime = 0;
  private int direction = 0;

  public EntityAILookIdle(GameObject entity) {
    this.entity = entity;
    SetMutexBits(3);
  }

  public override bool CanExecute() {
    return Random.value < 0.02;
  }

  public override bool ShouldContinueExecuting() {
    return idleTime >= 0;
  }

  public override void StartExecution() {
    float randomFloat = Random.value;
    idleTime += 20 + RandomUtils.RandomInt(20);
    direction = randomFloat >= 0.5 ? 1 : -1;
  }

  public override void UpdateExecution() {
    idleTime--;

    Vector3 rotation = entity.transform.localEulerAngles;
    rotation.y += 10 * direction;
    entity.transform.localEulerAngles = rotation;
  }

  public override void ResetTask() {
  }

  public override bool IsInterruptible() {
    return true;
  }
}
