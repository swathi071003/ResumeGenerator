using System.Text;
using ResumeGenerator.Models;

namespace ResumeGenerator.Services;

// Heuristic ATS-friendliness score. This mirrors the kind of checks real
// applicant-tracking parsers care about: parseable contact info, standard
// section headings, bullet-based experience, quantified achievements,
// a reasonable keyword-rich skills list, and sane overall length.
// It is a guideline, not a guarantee of passing any specific ATS.
public class AtsScoreService : IAtsScoreService
{
    public AtsResult Calculate(ResumeModel r)
    {
        int score = 0;
        var tips = new List<string>();

        // Contact info completeness — 15 pts
        int contactPts = 0;
        if (!string.IsNullOrWhiteSpace(r.PersonalInfo.FullName)) contactPts += 5;
        else tips.Add("Add your full name.");
        if (!string.IsNullOrWhiteSpace(r.PersonalInfo.Email)) contactPts += 5;
        else tips.Add("Add an email address.");
        if (!string.IsNullOrWhiteSpace(r.PersonalInfo.Phone)) contactPts += 5;
        else tips.Add("Add a phone number.");
        score += contactPts;

        // Summary — 10 pts
        var summaryWords = CountWordsIn(r.Summary);
        if (summaryWords >= 15)
        {
            score += 10;
        }
        else
        {
            tips.Add("Add a short professional summary (at least ~15 words).");
        }

        // Experience section — 20 pts
        if (r.Experience.Count > 0)
        {
            score += 10;
            bool hasBullets = r.Experience.All(e => e.Bullets.Count(b => !string.IsNullOrWhiteSpace(b)) >= 2);
            if (hasBullets)
            {
                score += 10;
            }
            else
            {
                tips.Add("Use at least 2 bullet points per job — ATS parsers read bullets better than paragraphs.");
            }
        }
        else
        {
            tips.Add("Add at least one work experience entry.");
        }

        // Quantifiable achievements — 15 pts
        bool hasNumbers = r.Experience
            .SelectMany(e => e.Bullets)
            .Any(b => b.Any(char.IsDigit));
        if (hasNumbers)
        {
            score += 15;
        }
        else
        {
            tips.Add("Include measurable results (numbers, %, $) in your bullet points.");
        }

        // Education — 10 pts
        if (r.Education.Count > 0)
        {
            score += 10;
        }
        else
        {
            tips.Add("Add your education details.");
        }

        // Skills — 15 pts
        var skillCount = r.Skills.Count(s => !string.IsNullOrWhiteSpace(s));
        if (skillCount >= 5)
        {
            score += 15;
        }
        else if (skillCount > 0)
        {
            score += 7;
            tips.Add("List at least 5 relevant skills as keywords.");
        }
        else
        {
            tips.Add("Add a skills section with relevant keywords.");
        }

        // Overall length — 10 pts
        int totalWords = CountTotalWords(r);
        if (totalWords >= 250 && totalWords <= 900)
        {
            score += 10;
        }
        else if (totalWords < 250)
        {
            tips.Add("Your resume looks thin — aim for at least ~250 words of content.");
        }
        else
        {
            tips.Add("Your resume is quite long for ATS scanning — consider trimming to under ~900 words.");
        }

        // Formatting safety — 5 pts (this template avoids tables/images/columns that break parsers)
        score += 5;

        return new AtsResult
        {
            Score = Math.Clamp(score, 0, 100),
            Tips = tips
        };
    }

    private static int CountWordsIn(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        return text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static int CountTotalWords(ResumeModel r)
    {
        var sb = new StringBuilder();
        sb.Append(r.Summary).Append(' ');
        foreach (var e in r.Experience)
        {
            sb.Append(e.Company).Append(' ').Append(e.Role).Append(' ');
            foreach (var b in e.Bullets) sb.Append(b).Append(' ');
        }
        foreach (var ed in r.Education)
        {
            sb.Append(ed.School).Append(' ').Append(ed.Degree).Append(' ');
        }
        foreach (var s in r.Skills) sb.Append(s).Append(' ');
        foreach (var p in r.Projects)
        {
            sb.Append(p.Name).Append(' ').Append(p.Description).Append(' ');
        }
        return CountWordsIn(sb.ToString());
    }
}
