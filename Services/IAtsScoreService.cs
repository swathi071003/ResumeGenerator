using ResumeGenerator.Models;

namespace ResumeGenerator.Services;

public class AtsResult
{
    public int Score { get; set; }
    public List<string> Tips { get; set; } = new();
}

public interface IAtsScoreService
{
    AtsResult Calculate(ResumeModel resume);
}
