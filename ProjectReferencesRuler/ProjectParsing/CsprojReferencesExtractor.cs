using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ProjectReferencesRuler.Rules.Project;
using ProjectReferencesRuler.Rules.References;

namespace ProjectReferencesRuler.ProjectParsing
{
    public class CsprojReferencesExtractor : IReferencesExtractor
    {
        // csproj elements
        private const string ItemGroup = "ItemGroup";
        private const string PropertyGroup = "PropertyGroup";
        private const string ProjectReference = "ProjectReference";
        private const string PackageReference = "PackageReference";
        private const string Include = "Include";
        private const string PrivateAssets = "PrivateAssets";
        private const string Version = "Version";
        private const string TargetFramework = "TargetFramework";
        private const string TargetFrameworks = "TargetFrameworks";
        private const string Import = "Import";
        private const string Project = "Project";

        public IEnumerable<Reference> GetProjectReferences(string csprojPath)
        {
            var from = Path.GetFileNameWithoutExtension(csprojPath);
            return GetReferences(csprojPath, ProjectReference)
                .Where(tuple => !string.IsNullOrEmpty(tuple.Item1))
                .Select(tuple => new Reference(
                    from: from,
                    to: Path.GetFileNameWithoutExtension(CleanPath(tuple.Item1)),
                    isPrivateAssetsAllSet: tuple.Item2 == "All",
                    versionOrNull: null));
        }

        public IEnumerable<Reference> GetPackageReferences(string csprojPath)
        {
            var from = Path.GetFileNameWithoutExtension(csprojPath);
            return GetReferences(csprojPath, PackageReference)
                .Select(tuple => new Reference(
                    from: from,
                    to: tuple.Item1,
                    isPrivateAssetsAllSet: tuple.Item2 == "All",
                    versionOrNull: tuple.Item3));
        }

        public IEnumerable<string> GetProjectReferencePaths(string csprojPath)
        {
            var doc = ParseXml(csprojPath);

            var projectReferenceItemGroup = GetItemGroups(doc, elementName: ProjectReference);

            return projectReferenceItemGroup
                .ElementsIgnoreNamespace(ProjectReference)
                .Select(GetIncludeContent)
                .Where(r => !string.IsNullOrEmpty(r));
        }

        private static XDocument ParseXml(string csprojPath)
        {
            var xmlString = File.ReadAllText(csprojPath);
            var doc = XDocument.Parse(xmlString);
            return doc;
        }

        public Project GetProjectProperties(string csprojPath)
        {
            var doc = ParseXml(csprojPath);
            return new Project(
                name: CleanPath(Path.GetFileNameWithoutExtension(csprojPath)),
                importedProps: GetImportedProps(doc).Select(CleanPath).ToList(),
                targetFrameworks: GetTargetFrameworks(doc).ToList());
        }

        /// <summary>
        /// Replaces \ with / in order for this same code to work on both Windows and Linux.
        /// </summary>
        private static string CleanPath(string path)
        {
            return path.Replace("\\", "/");
        }

        private IEnumerable<string> GetTargetFrameworks(XDocument doc)
        {
            foreach (var propertyGroup in doc.Root.ElementsIgnoreNamespace(PropertyGroup))
            {
                foreach (var targetFrameworkElement in propertyGroup.ElementsIgnoreNamespace(TargetFramework))
                {
                    yield return targetFrameworkElement.Value.Trim();
                }

                foreach (var targetFrameworks in propertyGroup.ElementsIgnoreNamespace(TargetFrameworks))
                {
                    foreach (var targetFramework in targetFrameworks.Value.Split(','))
                    {
                        yield return targetFramework.Trim();
                    }
                }
            }
        }

        private IEnumerable<string> GetImportedProps(XDocument doc)
        {
            foreach (var import in doc.Root.ElementsIgnoreNamespace(Import))
            {
                yield return import.Attribute(Project)?.Value;
            }
        }

        private IEnumerable<(string, string, string)> GetReferences(string csprojPath, string referenceType)
        {
            var doc = ParseXml(csprojPath);

            var projectReferenceItemGroup = GetItemGroups(doc, elementName: referenceType);

            return projectReferenceItemGroup
                .ElementsIgnoreNamespace(referenceType)
                .Select(xel => (GetIncludeContent(xel), GetPrivateAssetsContent(xel), GetVersionOrNull(xel)));
        }

        private static IEnumerable<XElement> GetItemGroups(XDocument doc, string elementName)
        {
            return doc.Root.ElementsIgnoreNamespace(ItemGroup)
                .Where(ig => ig.ElementsIgnoreNamespace(elementName).Any());
        }

        private static string GetIncludeContent(XElement pr)
        {
            return pr.Attribute(Include)?.Value;
        }

        private static string GetPrivateAssetsContent(XElement pr)
        {
            return pr.Attribute(PrivateAssets)?.Value;
        }

        private static string GetVersionOrNull(XElement pr)
        {
            return pr.Attribute(Version)?.Value;
        }
    }

    internal static class Extensions
    {
        public static IEnumerable<XElement> ElementsIgnoreNamespace(this XElement element, string name)
        {
            return element.Elements()
                .Where(e => e.Name.LocalName == name);
        }

        public static IEnumerable<XElement> ElementsIgnoreNamespace(this IEnumerable<XElement> elements, string name)
        {
            return elements.Elements()
                .Where(e => e.Name.LocalName == name);
        }
    }
}
