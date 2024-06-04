# snlspy

### Tech

1. .NET 8.0
2. Visual Studio Community 2022

### Basic Usage

To get a mermaid diagram of folder structure: `slnspy folders C:/path/to/sln/solution.sln`

To get a mermaid diagram of all projects: `slnspy projects C:/path/to/sln/solution.sln`

To get a mermaid diagram of specific projects: `slnspy projects C:/path/to/sln/solution.sln projectname1 projectname2 ...`

To get a mermaid diagram of reverse dependencies only of specific projects: `slnspy uponly C:/path/to/sln/solution.sln projectname1 projectname2 ...`

### Future Work

Intention is to evolve into a VS extension.
