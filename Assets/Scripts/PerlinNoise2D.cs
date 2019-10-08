using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerlinNoise2D {

  public class Generator {

    private System.Random random;
    private int[] permutation;

    private Vector2[] gradients;

    public Generator(int seed) {
      random = new System.Random(seed);
      CalculatePermutation();
      CalculateGradients();
    }

    private void CalculatePermutation() {
      permutation = Enumerable.Range(0, 256).ToArray();

      for (int i = 0; i < permutation.Length; i++) {
        int randomIndex = random.Next(permutation.Length);

        int originalValue = permutation[i];
        permutation[i] = permutation[randomIndex];
        permutation[randomIndex] = originalValue;
      }
    }

    private float NextFloat() {
      return (float) random.NextDouble();
    }

    private void CalculateGradients() {
      gradients = new Vector2[256];

      for (int i = 0; i < gradients.Length; i++) {
        Vector2 gradient;
        do {
          gradient = new Vector2(NextFloat() * 2 - 1, NextFloat() * 2 - 1);
        } while (gradient.sqrMagnitude >= 1);

        gradient.Normalize();
        gradients[i] = gradient;
      }
    }

    private float Fade(float t) {
      t = Math.Abs(t);
      return 1f - t * t * t * (t * (t * 6 - 15) + 10);
    }

    private float Q(float u, float v) {
      return Fade(u) * Fade(v);
    }

    public float GetHeight(float x, float y) {
      Vector2 cell = new Vector2(Mathf.Floor(x), Mathf.Floor(y));
      float total = 0f;

      Vector2[] corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
      foreach(Vector2 corner in corners) {
        Vector2 cellCorner = cell + corner;
        Vector2 cellUV = new Vector2(x - cellCorner.x, y - cellCorner.y);

        int index = permutation[(int) cellCorner.x % permutation.Length];
        index = permutation[(int) (index + cellCorner.y) % permutation.Length];

        Vector2 gradient = gradients[index % gradients.Length];

        total += Q(cellUV.x, cellUV.y) * Vector2.Dot(gradient, cellUV);
      }

      return (total + 1) / 2;
    }

  }

}
