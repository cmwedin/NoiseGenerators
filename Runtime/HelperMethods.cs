using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public static class HelperMethods
    {
        /// <summary>
        /// Calculates a the factors of a number O(sqrt(n)) time
        /// </summary>
        /// <param name="n">the number to calculate the factors of</param>
        /// <returns>A list of the factors of n</returns>
        public static List<int> Factor(int n) {
            int max = (int)Math.Sqrt(n);
            List<int> factors = new List<int>();

            for (int i = 1; i <= max; i++) {
                if(n % i == 0) {
                    factors.Add(i);
                    if(i != max) {
                        factors.Add(n / i);
                    }
                }
            }
            return factors;
        }
        /// <summary>
        /// Finds the closest factor of a number to a target number, up to O(sqrt(n)) time
        /// </summary>
        /// <param name="n">the number to find a factor of</param>
        /// <param name="t">the target number to find a factor close to</param>
        /// <returns>the closest factor of n to t</returns>
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

        /// <summary>
        /// Releases the resources of a render texture an recreates it with a new size
        /// </summary>
        /// <param name="renderTexture">the render texture to resize</param>
        /// <param name="newTexWidth">the new width to use for the texture</param>
        /// <param name="newTexHeight">the new height of the texture</param>
        public static void Resize(this RenderTexture renderTexture, int newTexWidth, int newTexHeight) {
            renderTexture.Release();
            renderTexture.width = newTexWidth;
            renderTexture.height = newTexHeight;
            renderTexture.Create();
        }

        public static RenderTexture Copy(this RenderTexture src) {
            var output = new RenderTexture(src.width, src.height, src.depth);
            output.enableRandomWrite = true;
            output.Create();
            output.wrapMode = TextureWrapMode.Repeat;
            Graphics.CopyTexture(src, output);
            return output;
        }
    }
}