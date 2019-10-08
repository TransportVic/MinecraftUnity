using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMeshGenerator {

  public class FaceGenerator {

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    private MeshCollider meshCollider;
    private Mesh mesh;
    private float totalTextureSize = 64;
    private float textureSize = 16;
    private float textureUnit;

    public FaceGenerator(Mesh mesh, MeshCollider meshCollider) {
      this.mesh = mesh;
      this.meshCollider = meshCollider;
      textureUnit = textureSize / totalTextureSize;
    }

    public void UpdateMesh() {
      mesh.Clear();
      mesh.vertices = vertices.ToArray();
      mesh.triangles = triangles.ToArray();
      mesh.uv = uvs.ToArray();
      mesh.Optimize();
      mesh.RecalculateNormals();
      meshCollider.sharedMesh = mesh;
    }

    public void Reset() {
      vertices.Clear();
      triangles.Clear();
      uvs.Clear();
    }

    void GenerateFace(Vector3 topLeft, Vector3 topRight, Vector3 bottomRight, Vector3 bottomLeft, bool facingPositive, Vector2 textureLocation) {
      int topLeftIndex = vertices.Count,
      topRightIndex = topLeftIndex + 1,
      bottomRightIndex = topRightIndex + 1,
      bottomLeftIndex = bottomRightIndex + 1;

      vertices.Add(topLeft);
      vertices.Add(topRight);
      vertices.Add(bottomRight);
      vertices.Add(bottomLeft);

      uvs.Add(new Vector2(textureUnit * textureLocation.x, textureUnit * textureLocation.y + textureUnit));
      uvs.Add(new Vector2(textureUnit * textureLocation.x + textureUnit, textureUnit * textureLocation.y + textureUnit));
      uvs.Add(new Vector2(textureUnit * textureLocation.x + textureUnit, textureUnit * textureLocation.y));
      uvs.Add(new Vector2(textureUnit * textureLocation.x, textureUnit * textureLocation.y));

      int[] triangleIndices = new int[] {
        bottomLeftIndex, bottomRightIndex, topLeftIndex,
        bottomRightIndex, topRightIndex, topLeftIndex
      };

      if (facingPositive) {
        Array.Reverse(triangleIndices);
      }

      triangles.AddRange(triangleIndices);
    }

    // give as bottom left coord
    public void GenerateFaceX(int x, int y, int z, bool facingPositive, Vector2 textureLocation) {
      Vector3 topLeft = new Vector3(x, y + 1, z);
      Vector3 topRight = new Vector3(x, y + 1, z + 1);
      Vector3 bottomRight = new Vector3(x, y, z + 1);
      Vector3 bottomLeft = new Vector3(x, y, z);

      GenerateFace(topLeft, topRight, bottomRight, bottomLeft, facingPositive, textureLocation);
    }

    public void GenerateFaceY(int x, int y, int z, bool facingPositive, Vector2 textureLocation) {
      Vector3 topLeft = new Vector3(x, y, z + 1);
      Vector3 topRight = new Vector3(x + 1, y, z + 1);
      Vector3 bottomRight = new Vector3(x + 1, y, z);
      Vector3 bottomLeft = new Vector3(x, y, z);

      GenerateFace(topLeft, topRight, bottomRight, bottomLeft, facingPositive, textureLocation);
    }

    public void GenerateFaceZ(int x, int y, int z, bool facingPositive, Vector2 textureLocation) {
      Vector3 topLeft = new Vector3(x, y + 1, z);
      Vector3 topRight = new Vector3(x - 1, y + 1, z);
      Vector3 bottomRight = new Vector3(x - 1, y, z);
      Vector3 bottomLeft = new Vector3(x, y, z);

      GenerateFace(topLeft, topRight, bottomRight, bottomLeft, facingPositive, textureLocation);
    }
  }
}
