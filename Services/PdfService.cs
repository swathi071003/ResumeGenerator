using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ResumeGenerator.Models;

namespace ResumeGenerator.Services;

public class PdfService : IPdfService
{
    public byte[] Generate(ResumeModel r)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                page.Header().Column(col =>
                {
                    col.Item().Text(string.IsNullOrWhiteSpace(r.PersonalInfo.FullName) ? "Your Name" : r.PersonalInfo.FullName)
                        .FontSize(20).Bold();

                    var contactParts = new[]
                    {
                        r.PersonalInfo.Email,
                        r.PersonalInfo.Phone,
                        r.PersonalInfo.Location,
                        r.PersonalInfo.LinkedIn,
                        r.PersonalInfo.Website
                    }.Where(s => !string.IsNullOrWhiteSpace(s));

                    col.Item().PaddingTop(2).Text(string.Join("  |  ", contactParts)).FontSize(9);
                });

                page.Content().PaddingTop(10).Column(col =>
                {
                    col.Spacing(8);

                    if (!string.IsNullOrWhiteSpace(r.Summary))
                    {
                        col.Item().Element(SectionHeader("SUMMARY"));
                        col.Item().Text(r.Summary);
                    }

                    if (r.Experience.Any())
                    {
                        col.Item().Element(SectionHeader("EXPERIENCE"));
                        foreach (var e in r.Experience)
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"{e.Role}{(string.IsNullOrWhiteSpace(e.Company) ? "" : " — " + e.Company)}").Bold();
                                row.ConstantItem(140).AlignRight().Text($"{e.StartDate} - {e.EndDate}").FontSize(9).Italic();
                            });
                            foreach (var b in e.Bullets.Where(b => !string.IsNullOrWhiteSpace(b)))
                            {
                                col.Item().PaddingLeft(10).Text($"• {b}");
                            }
                        }
                    }

                    if (r.Education.Any())
                    {
                        col.Item().Element(SectionHeader("EDUCATION"));
                        foreach (var ed in r.Education)
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"{ed.Degree}{(string.IsNullOrWhiteSpace(ed.School) ? "" : " — " + ed.School)}").Bold();
                                row.ConstantItem(140).AlignRight().Text($"{ed.StartDate} - {ed.EndDate}").FontSize(9).Italic();
                            });
                        }
                    }

                    if (r.Skills.Any(s => !string.IsNullOrWhiteSpace(s)))
                    {
                        col.Item().Element(SectionHeader("SKILLS"));
                        col.Item().Text(string.Join("  •  ", r.Skills.Where(s => !string.IsNullOrWhiteSpace(s))));
                    }

                    if (r.Projects.Any())
                    {
                        col.Item().Element(SectionHeader("PROJECTS"));
                        foreach (var p in r.Projects)
                        {
                            col.Item().Text(p.Name).Bold();
                            if (!string.IsNullOrWhiteSpace(p.Description))
                                col.Item().Text(p.Description);
                        }
                    }
                });
            });
        });

        return document.GeneratePdf();
    }

    private static Action<IContainer> SectionHeader(string title) => container =>
    {
        container.BorderBottom(1).BorderColor(Colors.Grey.Medium).PaddingBottom(2)
            .Text(title).Bold().FontSize(12).FontColor(Colors.Blue.Darken2);
    };
}
