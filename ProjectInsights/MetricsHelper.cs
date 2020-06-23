using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInsights
{
    public class MetricsHelper
    {
        private const int insertionAndDeletion = 3;
        private const int insertionOrDeletion = 2;

        public static bool IsCombinationFound(IDictionary<string, int> gitBlameMetrics, int similarityPercentage)
        {
            bool combinationFound = false;
            foreach (var metric in gitBlameMetrics)
            {
                string firstName = GetFirstPart(metric.Key);
                var otherKeys = GetOtherKeys(gitBlameMetrics, metric);
                foreach (var otherKey in otherKeys)
                {
                    combinationFound = ProcessCombination(gitBlameMetrics,
                                                          metric,
                                                          firstName,
                                                          otherKey,
                                                          similarityPercentage);
                    if (combinationFound) break;
                }
                if (combinationFound) break;
            }

            return combinationFound;
        }

        public static bool IsCombinationFound(Dictionary<string, (int, int)> commitMetricsDictionary, int similarityPercentage)
        {
            bool combinationFound = false;
            foreach (var metric in commitMetricsDictionary)
            {
                string firstName = GetFirstPart(metric.Key);
                var otherKeys = GetOtherKeys(commitMetricsDictionary, metric);
                foreach (var otherKey in otherKeys)
                {
                    combinationFound = ProcessCombination(commitMetricsDictionary,
                                                          metric,
                                                          firstName,
                                                          otherKey,
                                                          similarityPercentage);
                    if (combinationFound) break;
                }
                if (combinationFound) break;
            }

            return combinationFound;
        }

        private static bool ProcessCombination(Dictionary<string, (int, int)> commitMetricsDictionary,
            KeyValuePair<string, (int, int)> metric,
            string firstName,
            string otherKey,
            int similarityPercentage)
        {
            string otherFirstName = GetFirstPart(otherKey);

            if (Similarity.GetSimilarityPercentage(otherFirstName, firstName) >= similarityPercentage)
            {
                CombineAuthors(commitMetricsDictionary, metric.Key, otherKey);
                return true;
            }

            return false;
        }

        private static bool ProcessCombination(IDictionary<string, int> metricsDictionary,
            KeyValuePair<string, int>
            metric, string firstName,
            string otherKey,
            int similarityPercentage)
        {
            string otherFirstName = GetFirstPart(otherKey);

            if (Similarity.GetSimilarityPercentage(otherFirstName, firstName) >= similarityPercentage)
            {
                CombineAuthors(metricsDictionary, metric.Key, otherKey);
                return true;
            }

            return false;
        }

        private static IEnumerable<string> GetOtherKeys(
            Dictionary<string, (int, int)> commitMetricsDictionary,
            KeyValuePair<string, (int, int)> metric)
        {
            return commitMetricsDictionary.Keys.Where(a => a != metric.Key);
        }

        private static IEnumerable<string> GetOtherKeys(IDictionary<string, int> metricsDictionary, KeyValuePair<string, int> metric)
        {
            return metricsDictionary.Keys.Where(a => a != metric.Key);
        }

        private static string GetFirstPart(string key) => key.Split(Constants.Space).First();

        private static void CombineAuthors(IDictionary<string, int> dictionary, string metricKey, string otherKey)
        {
            int total = dictionary[metricKey] + dictionary[otherKey];

            if (metricKey.Length > otherKey.Length)
            {
                dictionary[metricKey] = total;
                dictionary.Remove(otherKey);
            }
            else
            {
                dictionary[otherKey] = total;
                dictionary.Remove(metricKey);
            }
        }

        private static void CombineAuthors(Dictionary<string, (int, int)> dictionary, string metricKey, string otherKey)
        {
            int totalChange = dictionary[metricKey].Item1 + dictionary[otherKey].Item1;
            int commitCount = dictionary[metricKey].Item2 + dictionary[otherKey].Item2;

            if (metricKey.Length > otherKey.Length)
            {
                dictionary[metricKey] = (totalChange, commitCount);
                dictionary.Remove(otherKey);
            }
            else
            {
                dictionary[otherKey] = (totalChange, commitCount);
                dictionary.Remove(metricKey);
            }
        }

        public static async Task<Dictionary<string, (int, int)>> GetMetricsFromGitLog(StreamReader output)
        {
            // key: mail, value: (totalChange, commitCount)
            var metrics = new Dictionary<string, (int, int)>();
            string all = await output.ReadToEndAsync();
            var logs = all.Split("\n\n").ToList();

            foreach (var log in logs)
                ProcessGitLog(metrics, log);

            return metrics;
        }

        private static void ProcessGitLog(Dictionary<string, (int, int)> metrics, string item)
        {
            var metricsAndMail = item.Trim().Split("\n").Reverse().ToList();
            var metricLine = metricsAndMail[0].Trim().Split(",");
            int totalChange = GetTotalChange(metricLine);

            string mail = StringHelper.SanitizeEmail(metricsAndMail[1].Trim());

            AddChangesToMetrics(metrics, totalChange, mail);
        }

        private static int GetTotalChange(string[] metricLine)
        {
            if (metricLine.Length == insertionAndDeletion)
            {
                int insertions = StringHelper.GetInsertion(metricLine);
                int deletions = StringHelper.GetRemoval(metricLine);

                return insertions + deletions;
            }
            else if (metricLine.Length == insertionOrDeletion)
            {
                int changes = StringHelper.GetChanges(metricLine);
                return changes;
            }

            return 0;
        }

        private static void AddChangesToMetrics(Dictionary<string, (int, int)> metrics, int totalChange, string mail)
        {
            if (metrics.ContainsKey(mail))
            {
                (int existingChanges, int commitCount) = metrics[mail];
                metrics[mail] = (existingChanges + totalChange, commitCount + 1);
            }
            else
                metrics.Add(mail, (totalChange, 1));
        }
    }
}
