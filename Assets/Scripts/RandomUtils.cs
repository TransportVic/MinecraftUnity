using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUtils {

  public static int RandomInt(int max) {
    return (int) Mathf.Round(Random.value * max);
  }

  public static Vector3 RandomPoint(GameObject entity, int distance) {
    int xOffset = RandomInt(distance);
    if (RandomInt(1) == 0) xOffset *= -1;
    int zOffset = RandomInt(distance);
    if (RandomInt(1) == 0) zOffset *= -1;

    return entity.transform.position + new Vector3(xOffset, 0, zOffset);
  }

}
