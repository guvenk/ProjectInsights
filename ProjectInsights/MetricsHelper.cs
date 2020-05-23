using System.Collections.Generic;
using System.Linq;

namespace ProjectInsights
{
    public class MetricsHelper
    {
        private const string space = " ";

        public static void AddFileMetrics(IDictionary<string, int> metrics, IDictionary<string, int> fileMetrics)
        {
            foreach (var pair in fileMetrics)
            {
                if (!metrics.ContainsKey(pair.Key))
                    metrics[pair.Key] = pair.Value;
                else
                    metrics[pair.Key] += pair.Value;
            }
        }

        public static Dictionary<string, int> GroupMetricsByAuthorName(IList<string> authors)
            => authors.GroupBy(a => a).ToDictionary(g => g.Key, g => g.Count());

        public static bool IsCombinationFound(IDictionary<string, int> metricsDictionary, int similarityAllowance)
        {
            bool combinationFound = false;
            foreach (var metric in metricsDictionary)
            {
                string firstName = GetFirstPart(metric.Key);
                var otherKeys = GetOtherKeys(metricsDictionary, metric);
                foreach (var otherKey in otherKeys)
                {
                    combinationFound = ProcessCombination(metricsDictionary, metric, firstName, otherKey, similarityAllowance);
                    if (combinationFound) break;
                }
                if (combinationFound) break;
            }

            return combinationFound;
        }

        static bool ProcessCombination(IDictionary<string, int> metricsDictionary, KeyValuePair<string, int> metric, string firstName, string otherKey, int similarityAllowance)
        {
            string otherFirstName = GetFirstPart(otherKey);

            if (StringHelper.GetSimilarityPercentage(otherFirstName, firstName) >= similarityAllowance)
            {
                CombineAuthors(metricsDictionary, metric.Key, otherKey);
                return true;
            }

            return false;
        }

        static IEnumerable<string> GetOtherKeys(IDictionary<string, int> metricsDictionary, KeyValuePair<string, int> metric)
        {
            return metricsDictionary.Keys.Where(a => a != metric.Key);
        }

        static string GetFirstPart(string key) => key.Split(space).First();

        static void CombineAuthors(IDictionary<string, int> dict, string metricKey, string otherKey)
        {
            int total = dict[metricKey] + dict[otherKey];

            if (metricKey.Length > otherKey.Length)
            {
                dict[metricKey] = total;
                dict.Remove(otherKey);
            }
            else
            {
                dict[otherKey] = total;
                dict.Remove(metricKey);
            }
        }
    }
}
