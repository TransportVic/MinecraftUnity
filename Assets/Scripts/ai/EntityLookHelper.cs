using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityLookHelper {

  private GameObject entity;
  private GameObject entityHead1;
  private GameObject entityHead2;
  private bool isTurning = false;

  private float yawDifference;

  public EntityLookHelper(GameObject entity) {
    this.entity = entity;

    entityHead1 = entity.transform.Find("head").gameObject;
    entityHead2 = entity.transform.Find("head_001").gameObject;
  }

  private void SetHeadRotation(float yaw, float pitch) {

  }


  public void LookAt(Vector3 target) {
    float m = (target - entity.transform.position).magnitude;
    float facingDifference = (entity.transform.forward - target + entity.transform.position).magnitude;
    yawDifference = Mathf.Acos((1 + m * m - Mathf.Pow(facingDifference, 2)) / 2 * m);
  }

  public void UpdateLookingAt(float deltaYaw, float deltaPitch) {
    Vector3 rotation = entity.transform.localEulerAngles;
    if ((rotation.y += deltaYaw) < yawDifference)
      rotation.y -= (deltaYaw - yawDifference);
    entity.transform.localEulerAngles = rotation;
  }

}
