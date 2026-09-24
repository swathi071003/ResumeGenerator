using ResumeGenerator.Models;

namespace ResumeGenerator.Services;

public interface IPdfService
{
    byte[] Generate(ResumeModel resume);
}
