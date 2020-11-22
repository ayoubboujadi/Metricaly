using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metricaly.Angular
{
    public static class RedisExtensions
    {
        public static async Task<T[]> SortedSetRangeByRankWithScoresAsync<T>(this IDatabase redisDb, RedisKey key, long start = 0, long stop = -1)
        {
            var sortedSetEntries = await redisDb.SortedSetRangeByRankWithScoresAsync(key, start, stop);

            if (sortedSetEntries.Length == 0)
                return new T[] { };

            var result = new T[sortedSetEntries.Length];

            for (int i = 0; i < sortedSetEntries.Length; i++)
            {
                var entry = sortedSetEntries[i];
                var json = entry.Element.ToString().Trim('"');
                result[i] = JsonConvert.DeserializeObject<T>(json);
            }

            return result;
        }

        public static async Task<T[]> SortedSetRangeByScoreAsync<T>(this IDatabase redisDb, RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity)
        {
            var sortedSetEntries = await redisDb.SortedSetRangeByScoreAsync(key, start, stop);

            if (sortedSetEntries.Length == 0)
                return new T[] { };

            var result = new T[sortedSetEntries.Length];

            for (int i = 0; i < sortedSetEntries.Length; i++)
            {
                var entry = sortedSetEntries[i];
                var json = entry.ToString().Trim('"');
                result[i] = JsonConvert.DeserializeObject<T>(json);
            }

            return result;
        }

        public static async Task<bool> SortedSetAddAsync<T>(this IDatabase redisDb, RedisKey key, T data, double score)
        {
            var json = JsonConvert.SerializeObject(data);
            return await redisDb.SortedSetAddAsync(key, json, score);
        }

    }
}
