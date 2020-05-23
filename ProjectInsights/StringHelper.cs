using System;

namespace ProjectInsights
{
    public class StringHelper
    {
        public static int ComputeLevenshteinDistance(string source, string target)
        {
            if (source == target)
                return source.Length;

            int[,] distance = new int[source.Length + 1, target.Length + 1];

            // Step 2
            for (int i = 0; i <= source.Length; distance[i, 0] = i++) ;
            for (int j = 0; j <= target.Length; distance[0, j] = j++) ;

            Step3and4(source, target, distance);

            return distance[source.Length, target.Length];
        }

        private static void Step3and4(string source, string target, int[,] distance)
        {
            for (int i = 1; i <= source.Length; i++)
                for (int j = 1; j <= target.Length; j++)
                    CalculateDistance(source, target, distance, i, j);
        }

        private static void CalculateDistance(string source, string target, int[,] distance, int i, int j)
        {
            int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
            distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
        }

        public static int GetSimilarityPercentage(string source, string target)
        {
            if (source == target)
                return 100;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            var percentage = GetPercentage(source, target, stepsToSame);
            return Convert.ToInt32(percentage * 100);
        }

        private static double GetPercentage(string source, string target, int stepsToSame)
        {
            return 1.0 - (stepsToSame / (double)Math.Max(source.Length, target.Length));
        }
    }
}
