using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public static class HelperMethods
    {
        public static List<int> Factor(int n) {
            int max = (int)Math.Sqrt(n);
            List<int> factors = new List<int>();

            for (int i = 1; i <= max; i++) {
                if(n % i == 0) {
                    factors.Add(i);
                    if(i != n/i) {
                        factors.Add(n / i);
                    }
                }
            }
            return factors;
        }
        public static Vector2Int PartitionTexture(int textureWidth, int textureHeight, int partitionCount) {
            var factors = Factor(partitionCount);
            int xCount;
            int yCount;
            if(factors.Count % 2 != 0) { //? The partition count has an integer square root
                xCount = yCount = factors[^1];
            } else {
                if(textureWidth >= textureHeight) {
                    xCount = factors[^1];
                    yCount = factors[^2];
                } else {
                    xCount = factors[^2];
                    yCount = factors[^1];
                }
            }
            return new Vector2Int(xCount,yCount);
        }
    }
}