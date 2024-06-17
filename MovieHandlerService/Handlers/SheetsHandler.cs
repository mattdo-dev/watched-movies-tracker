using Google.Apis.Auth.OAuth2;
using Google.Apis.Logging;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace MovieHandlerService.Handlers;

internal class SheetsHandler
{
    private SheetsService MovieSheets { get; }
    private const string SheetsId = "1ilQSVbUuBIJzAsSlkKVcen9peTgUtKvaYaZiuHNfee0";

    public SheetsHandler()
    {
        GoogleCredential credential;
        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.Spreadsheets);
        }

        MovieSheets = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Get watched movies and dates"
        });
    }

    public IList<IList<object>> GetDataInRange(string topLeft, string bottomRight)
    {
        var range = $"watched!{topLeft}:{bottomRight}";
        var request = MovieSheets.Spreadsheets.Values.Get(SheetsId, range);
        IList<IList<object>> values = request.Execute().Values;

        if (values == null || values.Count == 0)
        {
            // May need to check for valid but empty, fix this later
            throw new InvalidDataException();
        }

        return values;
    }
}