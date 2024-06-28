using System.Globalization;
using DatabaseService;
using MovieHandlerService.Utils;

namespace MovieHandlerService.Handlers;

public class QueryHandler
{
    private readonly Trie _trie = new();
    private readonly SheetsHandler _sheets = new();

    public QueryHandler()
    {
        // TODO: prepopulate data into memory and database.
        var datesDDMMRange2022 =
            _sheets.GetDataInRange("C2", "D123");
        // DateTime.ParseExact M/d/y
        var datesMMDDRange2022 =
            _sheets.GetDataInRange("C124", "D522");
        var datesMMDDRange2023 =
            _sheets.GetDataInRange("C523", "D3361");
        var datesMMDDRange2024 =
            _sheets.GetDataInRange("C3362", "D4521"); // and more
    }

    /// <summary>
    /// Returns a value in string type, giving either the date, multiple matches,
    /// or no matches, given a title query.
    /// </summary>
    /// <param name="query">The query to do a date lookup.</param>
    /// <returns>An array of either a date, a list of matches, or nothing.</returns>
    public string[]? MovieNameFindLastWatchedDate(string query)
    {
        var matches = _trie.Search(query);
        var enumerable = matches as string[] ?? matches.ToArray();
        var size = enumerable.Length;

        switch (size)
        {
            case 1:
                var lastWatchedDate = DbHandler.GetMovieLastWatchedDate(
                    enumerable.First());

                return [lastWatchedDate.ToString("dd MMMM, yyyy",
                    CultureInfo.InvariantCulture)];

            case > 1:
                return enumerable;
            case < 1:
                return null;
        }
    }
}