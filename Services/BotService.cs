
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Headers;
namespace RubikaFileSender.Services;

public class BotService : IBotService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BotService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendFileAsync(
        string token,
        string fileUrl,
        string chatId,
        string text)
    {
        var fileName = Path.GetFileName(
            new Uri(fileUrl).LocalPath
        );

        // if (string.IsNullOrWhiteSpace(fileName))
        //     fileName = "downloaded_file";

        // var fileType = GetFileType(fileName);

        var client = _httpClientFactory.CreateClient();

        // Step 1: Request upload URL

        // Step 1: Request upload URL

        var fileType = GetFileType(fileName);

        var requestData = new
        {
            type = fileType
        };
        var requestUrl = $"https://botapi.rubika.ir/v3/{token}/requestSendFile";

        var response = await client.PostAsJsonAsync(requestUrl, requestData);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        if (!result.TryGetProperty("data", out var data) ||
            !data.TryGetProperty("upload_url", out var uploadUrlElement))
        {
            throw new Exception("Failed to get upload URL from Rubika.");
        }

        var uploadUrl = uploadUrlElement.GetString()
            ?? throw new Exception("Upload URL is empty.");
        // var responseBody = await response.Content.ReadAsStringAsync();

        // Console.WriteLine("Rubika Response:");
        // Console.WriteLine(responseBody);

        // // بررسی پاسخ روبیکا
        // var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

        // if (!result.TryGetProperty("data", out var data) ||
        //     !data.TryGetProperty("upload_url", out var uploadUrlElement))
        // {
        //     throw new Exception($"Rubika request failed. Response: {responseBody}");
        // }

        // var uploadUrl = uploadUrlElement.GetString();

        // // Step 2: Download file as Stream

        // using var downloadResponse = await client.GetAsync(
        //     fileUrl,
        //     HttpCompletionOption.ResponseHeadersRead
        // );

        // downloadResponse.EnsureSuccessStatusCode();

        // using var stream = await downloadResponse.Content.ReadAsStreamAsync();

        // // Step 3: Upload Stream to Rubika

        // using var formData = new MultipartFormDataContent();

        // using var streamContent = new StreamContent(stream);

        // streamContent.Headers.ContentType =
        //     new MediaTypeHeaderValue("application/octet-stream");

        // formData.Add(streamContent, "file", fileName);

        // var uploadResponse = await client.PostAsync(
        //     uploadUrl!,
        //     formData
        // );

        // uploadResponse.EnsureSuccessStatusCode();

        // var uploadResult = await uploadResponse.Content
        //     .ReadFromJsonAsync<JsonElement>();

        // var fileId = uploadResult
        //     .GetProperty("data")
        //     .GetProperty("file_id")
        //     .GetString();


        // Step 2: Download file
        using var stream = await client.GetStreamAsync(fileUrl);

        // Step 3: Upload file to Rubika
        using var formData = new MultipartFormDataContent
        {
            { new StreamContent(stream), "file", fileName }
        };

        var uploadResponse = await client.PostAsync(uploadUrl, formData);

        var uploadResult = await uploadResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        uploadResponse.EnsureSuccessStatusCode();

        if (!uploadResult.TryGetProperty("data", out var uploadData) ||
            !uploadData.TryGetProperty("file_id", out var fileIdElement))
        {
            throw new Exception("Failed to upload file to Rubika.");
        }

        var fileId = fileIdElement.GetString()
            ?? throw new Exception("File ID is empty.");
        // Step 4: Send file to chat

        var sendData = new
        {
            chat_id = chatId,
            file_id = fileId,
            text = text
        };

        var sendResponse = await client.PostAsJsonAsync(
            $"https://botapi.rubika.ir/v3/{token}/sendFile",
            sendData
        );

        sendResponse.EnsureSuccessStatusCode();
    }
    private string GetFileType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        switch (extension)
        {
            case ".jpg":
            case ".jpeg":
            case ".png":
            case ".webp":
                return "Image";

            case ".mp4":
                return "Video";

            case ".mp3":
                return "Music";

            default:
                return "File";
        }
    }
}