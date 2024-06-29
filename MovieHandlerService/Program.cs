using MovieHandlerService.Handlers;

class Program
{
    static void Main()
    {
        // Initialize the QueryHandler which also prepopulates data
        var queryHandler = new QueryHandler();

        // Test 1: Normal case - searching for an exact movie title
        Console.WriteLine("Test 1: Exact match");
        var resultExact = queryHandler.MovieNameFindLastWatchedDate("batman");
        PrintResults(resultExact);

        // TODO: Exception caught early internally, needs fixing
        // // Test 2: No matches - searching for a non-existent title
        // Console.WriteLine("\nTest 2: No match");
        // var resultNone = queryHandler.MovieNameFindLastWatchedDate("loool");
        // PrintResults(resultNone);

        // Test 3: Return a date of a recorded movie title
        Console.WriteLine("\nTest 3: Get a date");
        // TODO: need explicit title support, or at least an option choosing method
        var result = queryHandler.MovieNameFindLastWatchedDate("spirited away");
        PrintResults(result);
    }

    private static void PrintResults(string[] results)
    {
        if (results.Length == 0)
        {
            Console.WriteLine("No results found.");
            return;
        }

        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
    }
}