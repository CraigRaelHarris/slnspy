using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace slnspy;
internal static class VSFilesHelper
{
    internal static List<Dependency> GetProjectReferences(string csprojFilePath)
    {
        var references = new List<Dependency>();

        try
        {
            var doc = new XmlDocument();
            doc.Load(csprojFilePath);

            var projectReferenceNodes = doc.SelectNodes("//ProjectReference/@Include");

            foreach (XmlNode node in projectReferenceNodes)
            {
                references.Add(new Dependency
                {
                    RelativePath = node.InnerText
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return references;
    }

    internal static (List<Project>, List<(string, string)>) ParseSlnFile(string filePath)
    {
        var projects = new List<Project>();
        var folderStructure = new List<(string, string)>();

        string projectPattern = @"Project\(""\{[A-F0-9\-]+\}""\) = ""(.*?)"", ""(.*?)"", ""\{([A-F0-9\-]+)\}""";
        string dependencyPattern = @"\{\b[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}\b\} = \{\b[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}\b\}";

        string content = File.ReadAllText(filePath);

        foreach (Match match in Regex.Matches(content, projectPattern))
        {
            var isProject = match.Groups[2].Value.EndsWith(".csproj");
            projects.Add(new Project
            {
                Guid = match.Groups[3].Value,
                Name = match.Groups[1].Value,
                Path = match.Groups[2].Value,
                IsProject = isProject,
                Dependencies = isProject ? GetProjectReferences(match.Groups[2].Value) : null
            });
        }

        foreach (var project in projects.Where(o => o.IsProject))
        {
            foreach (var dep in project.Dependencies)
            {
                dep.Project = projects.FirstOrDefault(o => dep.RelativePath.EndsWith(o.Name + ".csproj"));

            }
        }

        foreach (Match depMatch in Regex.Matches(content, dependencyPattern, RegexOptions.Singleline))
        {

            var guids = ExtractGuids(depMatch.ToString());
            folderStructure.Add((guids.Value.Item2.ToString(), guids.Value.Item1.ToString()));
        }

        return (projects, folderStructure);
    }


    static (string, string)? ExtractGuids(string input)
    {
        string pattern = @"\{([A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12})\} = \{([A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12})\}";
        Match match = Regex.Match(input, pattern);

        if (match.Success)
        {
            string guid1 = match.Groups[1].Value;
            string guid2 = match.Groups[2].Value;
            return (guid1, guid2);
        }
        else
        {
            return null;
        }
    }
}