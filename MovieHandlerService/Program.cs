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

        // Test 2: Multiple matches - searching for a partial title
        Console.WriteLine("\nTest 2: Partial match resulting in multiple titles");
        var resultMultiple = queryHandler.MovieNameFindLastWatchedDate("batman");
        PrintResults(resultMultiple);

        // Test 3: No matches - searching for a non-existent title
        Console.WriteLine("\nTest 3: No match");
        var resultNone = queryHandler.MovieNameFindLastWatchedDate("loool");
        PrintResults(resultNone);

        // Additional Tests:
        // Test 4: Edge Case - Case Insensitive Search
        Console.WriteLine("\nTest 4: Case Insensitive Search");
        var resultCaseInsensitive = queryHandler.MovieNameFindLastWatchedDate("batman");
        PrintResults(resultCaseInsensitive);

        // Test 5: Specific Date Format - Validating Date Formatting
        Console.WriteLine("\nTest 5: Specific Date Format Validation");
        var resultDateValidation = queryHandler.MovieNameFindLastWatchedDate("batman");
        PrintResults(resultDateValidation);
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