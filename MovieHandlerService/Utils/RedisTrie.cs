using StackExchange.Redis;
using DatabaseService;
using DatabaseService.Utils;

namespace MovieHandlerService.Utils;

// TODO: FIX. UNUSED CODE.
public static class RedisTrie
{
    private const string RootPrefix = "trieRoot";
    private const string TitleSetSuffix = ":titles";
    private const string ExistsHashField = "exists";
    private const string TerminatingKeySuffix = ":term";

    public static void Insert(string title)
    {
        var normalizedTitle = NormalizeTitle.Normalize(title);
        var db = RedisHandler.Database;
        var batch = db.CreateBatch();  // Create a batch for pipelined commands

        foreach (var start in Enumerable.Range(0, normalizedTitle.Length))
        {
            var currentPrefix = RootPrefix;
            foreach (var i in Enumerable.Range(start, normalizedTitle.Length - start))
            {
                currentPrefix += ":" + normalizedTitle[i];
                batch.StringSetAsync(currentPrefix + ExistsHashField, 1);
            }

            // Mark as terminating and add title at each node level asynchronously
            batch.StringSetAsync(currentPrefix + TerminatingKeySuffix, 1);
            batch.SetAddAsync(currentPrefix + TitleSetSuffix, title);
        }

        batch.Execute();  // Execute all pipelined commands together
    }

    public static IEnumerable<string> Search(string query)
    {
        query = NormalizeTitle.Normalize(query);
        var db = RedisHandler.Database;
        var currentPrefix = BuildKeyForQuery(query);

        return !db.KeyExists(currentPrefix + ExistsHashField) ? // Check if prefix exists
            [] : CollectAllTitles(db, currentPrefix);
    }

    private static HashSet<string> CollectAllTitles(IDatabase db, string baseKey)
    {
        // Directly pulling from terminating nodes' titles
        return [..db.SetMembers(baseKey + TitleSetSuffix).Select(v => (string)v!)];
    }

    private static string BuildKeyForQuery(string query)
    {
        return $"{RootPrefix}:{string.Join(":", query.Select(c => c))}";
    }
}