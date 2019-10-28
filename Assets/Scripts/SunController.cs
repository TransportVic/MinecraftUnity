using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour {
  float x;

  void Start() {

  }

  void Update () {
    x += Time.deltaTime * 2.5f;
    transform.rotation = Quaternion.Euler(x, 0, 0);
  }
}
