# ProjectsRuler

![Pipeline](https://github.com/DigitecGalaxus/ProjectsRuler/workflows/.NET%20Core/badge.svg)
![Nuget](https://img.shields.io/nuget/v/ReferencesRuler)

# Introduction

ReferencesRuler is a set of tools for the governance in a .NET solution. It is intended to run as an unit test since it parses project and solution files locally. It undestands the .NET Core csproj format and covers the following checks:

1. ProjectReferences including *PrivateAssets="All"* check.
2. PackageReferences including *PrivateAssets="All"* and *Version* check.
3. ProjectReferences existence check: checks if a project reference is still a valid project in the solution.
4. Project GUID in the Solution file. It should always be *"9A19103F-16F7-4668-BE54-9A1E7A4F7556"* and not *"FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"*.
5. Project and PackageReferences duplicates.

The ReferencesRuler is highly modular and extensible. You can use its components as you like. For example: you can use either the WildcardPatternParser or RegexPatternParser. You can write your own references extractor and use it (inject) instead of CsprojReferencesExtractor - if you want to support some other language, framework, framework version etc.

The main limitation: **It relies on having the sourcecode while running the tests!**

# Getting Started

1. Get the nuget package [ReferencesRuler](https://www.nuget.org/packages/ReferencesRuler/)
2. [Create a unit test](#Enforcing-rules)
3. Enjoy your stable architecture :)

# Rules

The rules are declarative and can be used for either project or package references. There are:

1. **Forbidden** - main rule
2. **Allowed** - exception from the main rule
3. **ExplicitlyForbidden** - override of the exception which cannot be overriden.

The rules are declared in that particular order, because each kind of rule is stronger than the previous one. The obligatory fields in each rule are *from*, *to*, *kind* and *description*. There are also two optional fields:

* **isPrivateAssetsAllSet** - can be used for either project or package references. It checks if the attribute *PrivateAssets="All"* is set.
* **version** - makes sense only with package references. If set with project references the ruler can deliver unexpected results.

## Rules examples

* A typical project/package reference rule:
  `new ReferenceRule(@"*B*", @"*A*", RuleKind.Forbidden, description: "B Projects must not reference A, they must use the new Framework Projects")`
* *PrivateAssets="All"* rule:
  `new ReferenceRule(@"*", @"*", RuleKind.Forbidden, description: "All references have to have PrivateAssets set to All.")`
* Version rule - only checks for the exact version:
  `new ReferenceRule(@"*", @"SomeNugetPackage", RuleKind.Forbidden, description: "Package version 1.2.3 is forbidden.", version="1.2.3")`

## Fluent builder

Rules can also be creating using the fluent builder API:

```C#
var rule = ReferenceRule.For("Project.A")
    .Referencing("Project.B")
    .IsForbidden()
    .Because("A should not reference B")
    .BuildRule();
```

# Enforcing rules <a name="enforcing_rules"></a>

In order to enforce rules, the ReferencesRuler is used. There are two separate methods for project and for package references. It is highly modular and extensible. You can use parsers and runners that suits your use case the best. Here is the typical .NET use case.

```C#
[Test]
public void ItIsNotAllowedToReferenceProjectAFromProjectB()
{
    AssertReferenceRules(
        // The rules
        new ReferenceRule(
            patternFrom: @"*B*",
            patternTo: @"*A*",
            RuleKind.Forbidden,
            description: "B Projects must not reference A, they must use the new Framework Projects")
    );
}

private void AssertReferenceRules(params ReferenceRule[] rules)
{
    var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    var solutionDir = Path.Combine(dir, @"..\..\..\");

    var complaints = ProjectsRuler.GetProjectReferencesComplaints(solutionDir, rules);
    // or var complaints = ProjectsRuler.GetPackageReferencesComplaints(solutionDir, rules);

    Assert.IsEmpty(complaints);
}
```

# Project references exitence check

There is a dedicated checker for that. It uses the same csproj parser as all the other tools in the ruler: **CsprojReferencesExtractor**.

```C#
[Test]
public void CheckForBrokenReferences()
{
    var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    var solutionDir = Path.Combine(dir, @"..\..\..\");
    var solutionFilePath = Path.Combine(solutionDir, "MySolution.sln");
    var checker = new ReferencesExistenceChecker(
        new SolutionParser(),
        new CsprojReferencesExtractor());

    var messages = checker.CheckProjectReferencesExistenceInSolution(solutionFilePath, "*.csproj").ToList;

    if (messages.Any())
    {
        Assert.Fail($"Check for broken references failed. See messages:\n{string.Join("\n", messages)}");
    }
}
```

# Solution file validity check

There is a change in the solution file format since the .net core. This code snippet checks all the solution files in the repository rood directory. It ignores folders. It only checks the projects with the given file extension.

```C#
[Test]
public void CheckIfSolutionHasAllValidProjectGuids()
{
    var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    var solutionDir = Path.Combine(dir, @"..\..\..\");

    var projectGuidChecker = new SolutionProjectGuidChecker(new SolutionParser());
    var complaints = new ProjectFilesRunner(solutionDir, "*.sln")
        .CollectComplaintsForFiles(
            filePath => new[]
            {
                projectGuidChecker.CheckSolutionProjectGuids(
                    filePath,
                    "9A19103F-16F7-4668-BE54-9A1E7A4F7556",
                    ".csproj")
            });

    Assert.IsEmpty(complaints);
}
```

# Advanced extensibility

In case of a scenario that some of the ruler components would need to be replaced, it can be easily injected and the whole setup can be done like this:

```C#
var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
var solutionDir = Path.Combine(dir, @"..\..\..\");

var runner = new ReferencesRulerRunner(
    extractor: new CsprojReferencesExtractor(),
    referencesRuler: new ReferencesRuler(
        patternParser: new WildcardPatternParser(),
        rules: rules),
    filesRunner: new ProjectFilesRunner(
        solutionPath: solutionDir,
        filesExtension: "*.csproj"));

var complaints = runner.GetPackageReferencesComplaints();
```

Remark: this is identical setup as in the ProjectsRuler static class. Do this only if your setup is different in any way. E.g. you want to use the Regex instead of the Wildcard patterns for the rules. ¯\\_(ツ)_/¯

# Build and Test

Please keep this project .NET Standard 2.0 so as many people as possible can use this tool.

# Contribute

1. Before starting please [create an issue first](https://github.com/DigitecGalaxus/ProjectsRuler/issues). That way we can discuss the feature before implementing it.
2. Create a pull request.
3. After creating a release, the new version will be available on the [nuget.org package page](https://www.nuget.org/packages/ReferencesRuler/).
