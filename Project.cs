namespace slnspy;

internal class Project
{
    public string Guid { get; set; }
    public string Path { get; set; }
    public string Name { get; set; }
    public bool IsProject { get; set; }
    public List<Dependency>? Dependencies { get; set; }
}
