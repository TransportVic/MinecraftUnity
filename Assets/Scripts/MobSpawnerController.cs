using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawnerController : MonoBehaviour {

  public GameObject sheep;

  void Start() {
    for (int i = 0; i < 5; i++) {
      int x = (int) Random.value * 128;
      int z = (int) Random.value * 128;
      Instantiate(sheep, new Vector3(x, 100, z), Quaternion.identity);
    }
  }

  void Update() {

  }
}
