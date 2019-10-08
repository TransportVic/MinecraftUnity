using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PerlinNoise2D;

public class TreeSpawner : MonoBehaviour {

  private SpawnerController spawner;
  private Generator heightGenerator;
  private bool hasInited = false;

  private int ConvertToUnixTimestamp(DateTime date) {
    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    TimeSpan diff = date.ToUniversalTime() - origin;
    return (int) Math.Floor(diff.TotalSeconds);
  }

  void Initalise() {
    heightGenerator = new Generator(ConvertToUnixTimestamp(DateTime.Now) + 2);

    for (int i = 0; i < 12; i++) {
      Vector3Int treeBase = Vector3Int.FloorToInt(new Vector3(UnityEngine.Random.value * 128, 65, UnityEngine.Random.value * 128));

      while (spawner.GetBlockAt(treeBase) == 0) {
        treeBase.y--;
      }

      float baseHeight = heightGenerator.GetHeight(treeBase.x / 48f, treeBase.z / 48f);
      int trunkHeight = 6 + (int) (4f * baseHeight);
      System.Random treeSizeRand = new System.Random((int) baseHeight * 1000000);

      for (int y = 0; y < trunkHeight; y++) {
        spawner.GenerateBlockAt(treeBase + new Vector3Int(0, y + 1, 0), 4);
      }

      int leavesHeight = 3 + treeSizeRand.Next(3);

      for (int y = 0; y < leavesHeight; y++) {
        int size = y * 2 + 1;

        for (int x = 0; x < size; x++) {
          for (int z = 0; z < size; z++) {
            Vector3Int leafPos = treeBase + new Vector3Int(x - y, trunkHeight - y + 2, z - y);
            if (leafPos.x == treeBase.x && leafPos.z == treeBase.z && leafPos.y <= trunkHeight + treeBase.y) continue;

            spawner.GenerateBlockAt(leafPos, 5);
          }
        }
      }
    }

    spawner.UpdateChunks();
  }

  void Start() {
    GameObject spawnerObject = GameObject.Find("WorldSpawner");
    spawner = spawnerObject.GetComponent<SpawnerController>();
  }

  void Update() {
    if (!hasInited && spawner.IsReadyToPopulate()) {
      hasInited = true;
      Initalise();
    }
  }
}
