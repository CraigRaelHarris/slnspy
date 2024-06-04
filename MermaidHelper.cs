namespace slnspy;

internal static class MermaidHelper
{
    internal static string GenerateMermaidForProjects(List<Project> projects, List<Project> selectedProjects, bool upOnly)
    {
        var mermaidLines = new List<string> { "graph LR" };

        foreach (var project in selectedProjects)
        {
            mermaidLines.AddRange(RecursiveLeft(projects, project));
            if (!upOnly)
                mermaidLines.AddRange(RecursiveRight(projects, project));
        }

        return string.Join("\n", mermaidLines);
    }

    internal static List<string> RecursiveLeft(List<Project> projects, Project focusProject)
    {
        var output = new List<string>();
        foreach (var dependee in projects.Where(o => o.Dependencies != null && o.Dependencies.Select(o => o.Project).Contains(focusProject)))
        {
            if (focusProject != dependee)
            {
                output.Add($"    {dependee.Name} --> {focusProject.Name}");
                output.AddRange(RecursiveLeft(projects, dependee));
            }
        }
        return output;
    }

    internal static List<string> RecursiveRight(List<Project> projects, Project focusProject)
    {
        var output = new List<string>();
        foreach (var dep in focusProject.Dependencies)
        {
            output.Add($"    {focusProject.Name} --> {dep.Project.Name}");
            output.AddRange(RecursiveRight(projects, dep.Project));
        }
        return output;
    }

    internal static string GenerateMermaidForFolders(List<Project> projects, List<(string, string)> dependencies)
    {
        var mermaidLines = new List<string> { "graph LR" };

        foreach (var dep in dependencies)
        {
            var a = projects.First(o => o.Guid == dep.Item1).Name.ToString().Replace(' ', '_');
            var b = projects.First(o => o.Guid == dep.Item2).Name.ToString().Replace(' ', '_');

            mermaidLines.Add($"    {a} --> {b}");
        }

        return string.Join("\n", mermaidLines);
    }

}
