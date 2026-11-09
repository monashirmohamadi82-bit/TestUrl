namespace RubikaFileSender.Services;

public interface IBotService
{
    Task SendFileAsync(
        string token,
        string fileUrl,
        string chatId,
        string text
    );
}