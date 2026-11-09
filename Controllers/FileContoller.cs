using Microsoft.AspNetCore.Mvc;
using RubikaFileSender.Services;

namespace RubikaFileSender.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IBotService _botService;

    public FileController(IBotService botService)
    {
        _botService = botService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendFile()
    {
        string token = "CGACIE0EUTIPUTQCXRDXDPCGYLVNGMQSKDZSILAPJICAFIUWPBOHQVJAISKBPMJQ";

       string fileUrl = "https://mmg.whatsapp.net/v/t62.7118-24/881390814_998892845908773_2377336200887265531_n.enc?ccb=11-4&oh=01_Q5Aa6AFku1yYLg7Pe8DneuNsdJkH5BoNd-Qn-JX5D0_yZdMc9A&oe=6B13D029&_nc_sid=5e03e0&mms3=true";
       // string fileUrl = "http://dashboard.local/media/gaudg3le/images.jpg";

        string chatId = "b0GL6E50BIwx02536594309d577313ea";

        string text = "تست ارسال فایل";

        await _botService.SendFileAsync(
            token,
            fileUrl,
            chatId,
            text
        );

        return Ok("File sent successfully");
    }
}