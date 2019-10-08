using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PerlinNoise2D;
using WorldMeshGenerator;

public class SpawnerController : MonoBehaviour {

  public GameObject chunkPrefab;
  private Generator surfaceGenerator;
  private Generator dirtGenerator;
  private ChunkController[,] chunks = new ChunkController[8,8];
  private bool chunksGenerated = false;
  private bool readyToPopulate = false;

  public bool IsReadyToPopulate() {
    return readyToPopulate;
  }

  private int ConvertToUnixTimestamp(DateTime date) {
    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    TimeSpan diff = date.ToUniversalTime() - origin;
    return (int) Math.Floor(diff.TotalSeconds);
  }

  public int[] GenerateBlockAt(Vector3 pos, int type) {
    return GenerateBlockAt((int) pos.x, (int) pos.y, (int) pos.z, type);
  }

  public int[] GenerateBlockAt(int x, int y, int z, int type) {
    int chunkX = (int) Mathf.Floor(x / 16f);
    int chunkZ = (int) Mathf.Floor(z / 16f);

    if (chunkX > 7 || chunkZ > 7 || chunkX < 0 || chunkZ < 0) return new int[]{};

    chunks[chunkX, chunkZ].SetBlockAt(x % 16, y, z % 16, type);

    return new int[] {chunkX, chunkZ};
  }

  public int GetBlockAt(Vector3 pos) {
    return GetBlockAt((int) pos.x, (int) pos.y, (int) pos.z);
  }

  public int GetBlockAt(int x, int y, int z) {
    int chunkX = (int) Mathf.Floor(x / 16);
    int chunkZ = (int) Mathf.Floor(z / 16);

    if (chunkX > 7 || chunkZ > 7 || chunkX < 0 || chunkZ < 0) return -1;

    return chunks[chunkX, chunkZ].GetBlockAt(x % 16, y, z % 16);
  }

  void GenerateChunks() {
    for (int chunkX = 0; chunkX < 8; chunkX++) {
      for (int chunkZ = 0; chunkZ < 8; chunkZ++) {
        GameObject chunkController = Instantiate(chunkPrefab, new Vector3(chunkX * 16, 0, chunkZ * 16), Quaternion.identity);
        chunkController.name = "Chunk: " + chunkX + "-" + chunkZ;
        chunks[chunkX, chunkZ] = chunkController.GetComponent<ChunkController>();
        chunks[chunkX, chunkZ].Initalise();
      }
    }
  }

  public void UpdateChunk(int chunkX, int chunkZ) {
    chunks[chunkX, chunkZ].UpdateMesh();
  }

  public void UpdateChunks() {
    for (int i = 0; i < chunks.GetLength(0); i++) {
      for (int j = 0; j < chunks.GetLength(1); j++) {
        UpdateChunk(i, j);
      }
    }
  }

  void Start() {
    GenerateChunks();
    surfaceGenerator = new Generator(ConvertToUnixTimestamp(DateTime.Now));
    dirtGenerator = new Generator(ConvertToUnixTimestamp(DateTime.Now) + 1);

    for (int x = 0; x < 128; x++) {
      for (int z = 0; z < 128; z++) {
        int surfaceY = (int) Mathf.Round(24f * surfaceGenerator.GetHeight(x / 48f, z / 48f)) + 30;
        int lowerDirtY = surfaceY - 13 + (int) Mathf.Round(10f * dirtGenerator.GetHeight(x / 48f, z / 48f));
        GenerateBlockAt(x, surfaceY, z, 1);
        for (int dirtY = surfaceY - 1; dirtY >= lowerDirtY; dirtY--) {
          GenerateBlockAt(x, dirtY, z, 2);
        }
        for (int stoneY = lowerDirtY - 1; stoneY >= 0; stoneY--) {
          GenerateBlockAt(x, stoneY, z, 3);
        }
      }
    }

    readyToPopulate = true;
  }

  void Update() {
    if (!chunksGenerated) {
      UpdateChunks();
      chunksGenerated = true;
    }
  }
}
