using Microsoft.AspNetCore.Mvc;
using ResumeGenerator.Models;
using ResumeGenerator.Services;

namespace ResumeGenerator.Controllers;

public class ResumeController : Controller
{
    private readonly IAtsScoreService _atsService;
    private readonly IPdfService _pdfService;

    public ResumeController(IAtsScoreService atsService, IPdfService pdfService)
    {
        _atsService = atsService;
        _pdfService = pdfService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ResumeModel());
    }

    [HttpPost]
    [Route("Resume/CalculateAts")]
    public IActionResult CalculateAts([FromBody] ResumeModel resume)
    {
        var result = _atsService.Calculate(resume);
        return Json(result);
    }

    [HttpPost]
    [Route("Resume/DownloadPdf")]
    public IActionResult DownloadPdf([FromBody] ResumeModel resume)
    {
        var bytes = _pdfService.Generate(resume);
        var fileName = string.IsNullOrWhiteSpace(resume.PersonalInfo.FullName)
            ? "resume.pdf"
            : $"{resume.PersonalInfo.FullName.Replace(" ", "_")}_Resume.pdf";
        return File(bytes, "application/pdf", fileName);
    }
}
