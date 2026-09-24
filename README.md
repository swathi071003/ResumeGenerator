# Resume Generator (ASP.NET Core)

A resume builder with a live preview, an ATS-friendliness score bar that
updates as you type, and server-generated PDF download.

## Requirements
- .NET 8 SDK (https://dotnet.microsoft.com/download)

## Run it

```bash
cd ResumeGenerator
dotnet restore
dotnet run
```

Then open the URL shown in the terminal (usually `https://localhost:5001` or
`http://localhost:5000`).

## How it works
- **Models/ResumeModel.cs** — the resume data shape (personal info, summary,
  experience, education, skills, projects).
- **Services/AtsScoreService.cs** — heuristic scoring (0-100) based on
  contact info completeness, summary length, bullet usage, quantified
  achievements, skills count, and overall length, plus specific tips.
- **Services/PdfService.cs** — builds the downloadable PDF using QuestPDF
  (Community license, free for this kind of use — set in `Program.cs`).
- **Controllers/ResumeController.cs** — serves the form page, and exposes
  `POST /Resume/CalculateAts` (returns JSON score+tips) and
  `POST /Resume/DownloadPdf` (returns the generated PDF file).
- **wwwroot/js/site.js** — all client-side state, live preview rendering,
  the debounced call to the ATS endpoint, and the download button.

## Notes / next steps you might want
- Currently there's no database — nothing is saved between page loads.
  Easiest next step: serialize `state` to `localStorage` on every change
  and restore it on page load, no backend change needed.
- To go further (multi-user accounts, saved resumes across devices), add
  EF Core + a database (SQLite is the easiest to start with) and a couple
  of endpoints to save/load a resume by user.
- To make the ATS score smarter, you could pass in a target job description
  and score keyword overlap against it — that's the single highest-impact
  addition for a resume tool like this.
