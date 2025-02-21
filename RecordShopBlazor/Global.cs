using System.Text.Json;

using RecordShopBlazor.Tables;

public static class Global
{
    public static void JsonSrl<t>(t input)
    {
        Console.WriteLine(JsonSerializer.Serialize(input, new JsonSerializerOptions { WriteIndented = true }));
    }


    //Do not dispose, reuse same client!
    public static HttpClient client = new HttpClient() { BaseAddress = new("https://localhost:7148") };

    public static List<Albums> GetAllRecords()
    {
        return client.GetFromJsonAsync<List<Albums>>("/Album").Result;
    }

    public static Albums GetRecordById(int id)
    {
        return client.GetFromJsonAsync<Albums>($"/Album/{id}").Result;
    }
    
}

