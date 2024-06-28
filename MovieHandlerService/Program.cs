using DatabaseService;
using MovieHandlerService.Handlers;
using MovieHandlerService.Utils;

var sheets = new SheetsHandler();

// DateTime.ParseExact D/m/y
var datesDDMMRange2022 = sheets.GetDataInRange("C2", "D123");
// DateTime.ParseExact M/d/y
var datesMMDDRange2022 = sheets.GetDataInRange("C124", "D522");
var datesMMDDRange2023 = sheets.GetDataInRange("C523", "D3361");
var datesMMDDRange2024 = sheets.GetDataInRange("C3362", "D4521"); // and more

Console.WriteLine("starting program");
var trie = new Trie();
string? dateWatched = null;
foreach (var row in datesMMDDRange2022)
{
    if (row.Count <= 1) continue;

    if (!string.IsNullOrWhiteSpace(row[0].ToString()))
    {
        dateWatched = row[0].ToString().Trim();
    }

    if (dateWatched == "skipped") continue;

    var movieWatched = row[1].ToString()?.Trim();

    if (string.IsNullOrWhiteSpace(movieWatched))
    {
        continue;
    }

    string realDate;
    string[] numDate = dateWatched.Split('/');
    if (Int32.Parse(numDate[1]) < 6)
    {
        realDate = numDate[0] + "/" + numDate[1] + "/24";
    }
    else
    {
        realDate = numDate[0] + "/" + numDate[1] + "/23";
    }

    // Console.WriteLine($"{dateWatched}, {movieWatched}");
    var parsedDate = DateTime.Parse(realDate);
    DbHandler.InsertWatchedMovie(movieWatched, parsedDate);
    trie.Insert(movieWatched);
}

// var results = RedisTrie.Search("day");
// foreach (var title in results)
// {
//     Console.WriteLine(title);
// }

var e = trie.Search("batman");

foreach (var va in e)
{
    Console.WriteLine(va);
}

Console.WriteLine(DbHandler.GetMovieLastWatchedDate("batman and superman: battle of the super sons"));

Console.WriteLine("end of program");