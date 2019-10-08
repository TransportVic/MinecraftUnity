using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMeshGenerator;

public class ChunkController : MonoBehaviour {

  private Mesh mesh;
  private MeshCollider meshCollider;
  private FaceGenerator faceGenerator;
  private int[,,] blocks = new int[16, 80, 16];

  private Vector2[,] blockTextureMaps = new Vector2[6, 6] {
    {new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0)}, // 0: air
    {new Vector2(0, 2), new Vector2(0, 2), new Vector2(0, 1), new Vector2(0, 3), new Vector2(0, 2), new Vector2(0, 2)}, // 1: grass
    {new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1)}, // 2: dirt
    {new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0)},  // 3: stone
    {new Vector2(1, 2), new Vector2(1, 2), new Vector2(1, 3), new Vector2(1, 3), new Vector2(1, 2), new Vector2(1, 2)},  // 4: oak log
    {new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0)}  // 5: oak leaf
  };

  public void Initalise() {
    mesh = GetComponent<MeshFilter>().mesh;
    meshCollider = GetComponent<MeshCollider>();
    faceGenerator = new FaceGenerator(mesh, meshCollider);
  }

  public void SetBlockAt(int x, int y, int z, int type) {
    blocks[x, y, z] = type;
  }

  public int GetBlockAt(int x, int y, int z) {
    return blocks[x, y, z];
  }

  void GenerateBlockAt(int x, int y, int z) {
    if (blocks[x, y, z] == 0) return;

    int blockType = blocks[x, y, z];

    if (x == 0 || blocks[x - 1, y, z] == 0)
      faceGenerator.GenerateFaceX(x, y, z, false, blockTextureMaps[blockType, 0]);
    if (x == 15 || blocks[x + 1, y, z] == 0)
      faceGenerator.GenerateFaceX(x + 1, y, z, true, blockTextureMaps[blockType, 1]);

    if (y == 0 || blocks[x, y - 1, z] == 0)
      faceGenerator.GenerateFaceY(x, y, z, false, blockTextureMaps[blockType, 2]);
    if (y == 79 || blocks[x, y + 1, z] == 0)
      faceGenerator.GenerateFaceY(x, y + 1, z, true, blockTextureMaps[blockType, 3]);

    if (z == 0 || blocks[x, y, z - 1] == 0)
      faceGenerator.GenerateFaceZ(x + 1, y, z, false, blockTextureMaps[blockType, 4]);
    if (z == 15 || blocks[x, y, z + 1] == 0)
      faceGenerator.GenerateFaceZ(x + 1, y, z + 1, true, blockTextureMaps[blockType, 5]);
  }

  public void UpdateMesh() {
    faceGenerator.Reset();

    for (int i = 0; i < blocks.GetLength(0); i++) {
      for (int j = 0; j < blocks.GetLength(1); j++) {
        for (int k = 0; k < blocks.GetLength(2); k++) {
          GenerateBlockAt(i, j, k);
        }
      }
    }

    faceGenerator.UpdateMesh();
  }

  void Start() {}
  void Update() {}
}
