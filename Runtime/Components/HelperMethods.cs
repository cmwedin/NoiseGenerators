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
        public static int FindClosestFactor(int n, int t) {
            int max = (int)Math.Sqrt(n);
            int bestResult = 1;
            int bestDist = t - 1;

            bool compareDecreasing = true;
            bool compareIncreasing = true;
            for (int i = 1; i <= max; i++)
            {
                if (n % i == 0) {
                    if(compareIncreasing) {
                        int dist = Math.Abs(t - i);
                        if(dist < bestDist) {
                            bestResult = i;
                            bestDist = dist;
                        }
                        if( i >= t) {
                            compareIncreasing = false;
                        }
                    } 
                    if(compareDecreasing) {
                        int j = n / i;
                        int dist = Math.Abs(t - j);
                        if(dist < bestDist) {
                            bestResult = j;
                            bestDist = dist;
                        } 
                        if( j <= t) {
                            compareDecreasing = false;
                        }
                    }
                }
                if(!compareIncreasing && ! compareDecreasing) {
                    break;
                }
            }
            return bestResult;
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
        public static void Resize(this RenderTexture renderTexture, int newTexWidth, int newTexHeight) {
            renderTexture.Release();
            renderTexture.width = newTexWidth;
            renderTexture.height = newTexHeight;
            renderTexture.Create();
        }
    }
}