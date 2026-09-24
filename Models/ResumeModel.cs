namespace ResumeGenerator.Models;

public class ResumeModel
{
    public PersonalInfo PersonalInfo { get; set; } = new();
    public string Summary { get; set; } = "";
    public List<ExperienceItem> Experience { get; set; } = new();
    public List<EducationItem> Education { get; set; } = new();
    public List<string> Skills { get; set; } = new();
    public List<ProjectItem> Projects { get; set; } = new();
}

public class PersonalInfo
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Location { get; set; } = "";
    public string LinkedIn { get; set; } = "";
    public string Website { get; set; } = "";
}

public class ExperienceItem
{
    public string Company { get; set; } = "";
    public string Role { get; set; } = "";
    public string StartDate { get; set; } = "";
    public string EndDate { get; set; } = "";
    public List<string> Bullets { get; set; } = new();
}

public class EducationItem
{
    public string School { get; set; } = "";
    public string Degree { get; set; } = "";
    public string StartDate { get; set; } = "";
    public string EndDate { get; set; } = "";
}

public class ProjectItem
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Link { get; set; } = "";
}
