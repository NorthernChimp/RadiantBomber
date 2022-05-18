// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("kH4WZOIW2utTQ+VYFeGgsxflWNROFbxfUMa4GWNBWqlT4niqFwK88/c5bbDKgBFZPzEjD1tRNnSoysjJI8nLyk/+EastrbOXpmUWKVCaBafAcvHSwP32+dp2uHYH/fHx8fXw84ooFCIg/ZRl3TvVZzVyIhuF9dkbcvH/8MBy8frycvHx8ADex+T7KPJCUMsbUBgeJBz1eAkUuKsZgm09CRofBvEwufwQGcazz3jhPoPVXmX/SgHwDmk5j4rCh/6tQGDgFvaCK5dPQNGJ9hflsztVf2U03di2hNRJ5qW2Ze5skDPZWvPafL6vwiwIEAq3Q1xrK9NBYUZFa2FpqZHJzUWbFKkmFJPgnqcmwemCPeT6LtZs4rs6pb8+Fm3DCg8nP/Lz8fDx");
        private static int[] order = new int[] { 6,13,7,12,6,7,13,10,13,9,11,11,13,13,14 };
        private static int key = 240;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
