using slnspy;

if (args.Length < 2 || (args[0] != "folders" && args[0] != "projects" && args[0] != "uponly"))
{
    Console.WriteLine("To get a mermaid diagram of folder structure: ");
    Console.WriteLine("   slnspy folders C:/path/to/sln/solution.sln");
    Console.WriteLine("To get a mermaid diagram of all projects: ");
    Console.WriteLine("   slnspy projects C:/path/to/sln/solution.sln");
    Console.WriteLine("To get a mermaid diagram of specific projects: ");
    Console.WriteLine("   slnspy projects C:/path/to/sln/solution.sln projectname1 projectname2 ...");
    Console.WriteLine("To get a mermaid diagram of reverse dependencies only of specific projects: ");
    Console.WriteLine("   slnspy uponly C:/path/to/sln/solution.sln projectname1 projectname2 ...");
    return;
}

string filePath = args[1];

if (!Path.Exists(filePath) || !File.Exists(filePath))
{
    Console.WriteLine("Path or File does not exist: " + filePath);
    return;
}

var path = Path.GetDirectoryName(filePath);
Directory.SetCurrentDirectory(path);
var (projects, folderStructure) = VSFilesHelper.ParseSlnFile(filePath);

if (args[0] == "folders")
{
    Console.WriteLine(MermaidHelper.GenerateMermaidForFolders(projects, folderStructure));
}
else
{
    var upOnly = args[0] == "uponly";

    if (args.Length == 2)
    {
        Console.WriteLine(MermaidHelper.GenerateMermaidForProjects(projects, projects, upOnly));
        return;
    }

    var selectedProjects = new List<Project>();
    for (int i = 2; i < args.Length; i++)
    {
        var p = projects.FirstOrDefault(o =>o.Name == args[i]);
        if (p == null)
        {
            Console.WriteLine("Unable to find project: " +  args[i]);
            return;
        }
        selectedProjects.Add(p);
    }

    Console.WriteLine(MermaidHelper.GenerateMermaidForProjects(projects, selectedProjects, upOnly));
}
