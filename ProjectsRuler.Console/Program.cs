using System.IO;
using ProjectReferencesRuler;
using ProjectReferencesRuler.ProjectParsing;
using ProjectReferencesRuler.ProjectRunners;
using ProjectReferencesRuler.Rules;
using ProjectReferencesRuler.Rules.References;

namespace MonolithRuler.Console
{
    public class Program
    {
        private const string DevinitePath = @"C:\Development\devinite";

        static void Main(string[] args)
        {
//            TestReferenceRulerInConsole();
//            TestReferencesExistenceCheckerInConsole();
//            TestPackagesRulerInConsole();
//            TestPrivateAssetsInConsole();
            TestSolutionGuidsCheckerInConsole();
        }

        private static void TestPrivateAssetsInConsole()
        {
            var rules = new[]
            {
                // The rules
                new ReferenceRule(@"*", @"*", RuleKind.Forbidden, description: "Everybody has to use PrivateAssets=\"All\"!", isPrivateAssetsAllSet: false),

                // All the exceptions:
                // Applications & Tests etc.
//                new ReferenceRule(@"Dg.*Test*", @"*", RuleKind.Allowed,
//                    description: "It is allowed to reference anything from a tests project.", isPrivateAssetsAllSet: true),
            };

            var runner = new ReferencesRulerRunner(
                extractor: new CsprojReferencesExtractor(),
                referencesRuler: new ReferencesRuler(
                    patternParser: new WildcardPatternParser(),
                    rules: rules),
                filesRunner: new ProjectFilesRunner(
                    solutionPath: DevinitePath,
                    filesExtension: "*.csproj"));

            System.Console.WriteLine(runner.GetComplaintsForProjectReferences());
        }

        private static void TestReferencesExistenceCheckerInConsole()
        {
            new ReferencesExistenceChecker(new SolutionParser(), new CsprojReferencesExtractor()).CheckProjectReferencesExistenceInSolution(DevinitePath, ".csproj");
        }

        private static void TestSolutionGuidsCheckerInConsole()
        {
//            var solutionPath = Path.Combine(DevinitePath, "devinite.FullWithoutExternal.sln");
//            var solutionPath = Path.Combine(@"C:\Development\devinite", "devinite.FullWithoutExternal.sln");
//            System.Console.WriteLine(new SolutionProjectGuidChecker(new SolutionParser()).CheckSolutionProjectGuids(solutionPath, "9A19103F-16F7-4668-BE54-9A1E7A4F7556"));

            var solutionDir = @"C:\Development\devinite";
            var projectGuidChecker = new SolutionProjectGuidChecker(new SolutionParser());
            var complaints = new ProjectFilesRunner(solutionDir, "*.sln").CollectComplaintsForFiles(
                filePath => new[] { projectGuidChecker.CheckSolutionProjectGuids(filePath, "9A19103F-16F7-4668-BE54-9A1E7A4F7556", ".csproj") });

            System.Console.WriteLine(complaints);
        }

        private static void TestPackagesRulerInConsole()
        {
            var rules = new[]
            {
                // The rules
                new ReferenceRule(@"Dg.*.Contracts", @"Dg", RuleKind.Forbidden,
                    description: "Contracts are not allowed to reference the Dg package!"),

                // All the exceptions:
                // Applications & Tests etc.
                new ReferenceRule(@"Dg.*Test*", @"*", RuleKind.Allowed,
                    description: "It is allowed to reference anything from a tests project."),
            };

            var runner = new ReferencesRulerRunner(
                extractor: new CsprojReferencesExtractor(),
                referencesRuler: new ReferencesRuler(
                    patternParser: new WildcardPatternParser(),
                    rules: rules),
                filesRunner: new ProjectFilesRunner(
                    solutionPath: DevinitePath,
                    filesExtension: "*.csproj"));

            System.Console.WriteLine(runner.GetComplaintsForPackageReferences());
        }

        private static void TestReferenceRulerInConsole()
        {
            var rules = new[]
            {
                // The rules
                new ReferenceRule(@"Dg.*", @"devinite.*", RuleKind.Forbidden,
                    description: "It is not allowed to reference the monolith from a component."),
                new ReferenceRule(@"Dg.*.Contracts", @"*", RuleKind.Forbidden,
                    description: "Contracts are not allowed to have any project references!"),
                new ReferenceRule(@"devinite.*", @"Dg.*", RuleKind.Forbidden,
                    description: "It is not allowed to reference components from the monolith."),
                new ReferenceRule(@"Dg.CategoryManagement*", @"Dg.Logistics*", RuleKind.Forbidden,
                    description: "Areas should not touch each other directly."),
                new ReferenceRule(@"Dg.Logistics.Monolith", @"Dg.CategoryManagement*", RuleKind.Forbidden,
                    description: "Areas should not touch each other directly."),

                // Component architecture - not ready yet.
//                new ReferenceRule(@"*", @"Dg.*", RuleKind.Forbidden, description: "It is not allowed to reference a component implementation."),
//                new ReferenceRule(@"Dg\.(?<subdomain>\w+)\.DataAccess", @"Dg\.(?<subdomain>\w+)", RuleKind.Allowed, description: "DataAccess from the same component can access its component implementation"),

                // All the exceptions:
                // Applications & Tests etc.
                new ReferenceRule(@"Dg.*Test*", @"*", RuleKind.Allowed,
                    description: "It is allowed to reference anything from a tests project."),
                new ReferenceRule(@"devinite.*Test*", @"*", RuleKind.Allowed,
                    description: "It is allowed to reference anything from a tests project."),
                new ReferenceRule(@"Dg.*.Erp", @"devinite.*", RuleKind.Allowed,
                    description: "Frontend is allowed to reference all the things."),
                new ReferenceRule(@"Dg.*.Monolith", @"devinite.*", RuleKind.Allowed,
                    description: "Website is allowed to reference (almost) all the things."),
                new ReferenceRule(@"Dg.*.Website", @"devinite.*", RuleKind.Allowed,
                    description: "Website is allowed to reference all the things."),
                new ReferenceRule(@"Dg.Finance.PaymentServices", @"devinite.*.Contracts", RuleKind.Allowed,
                    description: "It is funny that ClassLibrary hast Contracts, but it is allowed to reference it."),
                new ReferenceRule(@"devinite.SystemTasks*", @"Dg.*", RuleKind.Allowed,
                    description: "Applications are allowed to reference everything."),
                new ReferenceRule(@"devinite.PortalSystem", @"Dg.*", RuleKind.Allowed,
                    description: "Applications are allowed to reference everything."),
                new ReferenceRule(@"devinite.PortalSystem.*", @"Dg.Logistics*", RuleKind.Allowed,
                    description: "Applications are allowed to reference everything."),
                new ReferenceRule(@"devinite.PortalSystem.*", @"Dg.CategoryManagement", RuleKind.Allowed,
                    description: "Applications are allowed to reference everything."),
                new ReferenceRule(@"devinite.PortalSystem.*", @"Dg.*.InversionOfControl", RuleKind.Allowed,
                    description: "Applications are allowed to reference everything."),
                new ReferenceRule(@"devinite.PortalSystem.*", @"Dg.*.Contracts", RuleKind.Allowed,
                    description: "Applications are allowed to reference everything."),
                new ReferenceRule(@"devinite.PortalSystem.*", @"Dg.Pdm*", RuleKind.Allowed,
                    description: "Applications are allowed to reference everything."),
                new ReferenceRule(@"devinite.*", @"Dg.*.Contracts", RuleKind.Allowed,
                    description: "Everybody but contracts can reference contracts."),
                new ReferenceRule(@"devinite.ConsoleApplicationForDebugging", @"*", RuleKind.Allowed,
                    description: "Everybody but contracts can reference contracts."),

                // Areas
                new ReferenceRule(@"Dg.Logistics*", @"devinite.*", RuleKind.Allowed, description: "This is an exception rule."),
                new ReferenceRule(@"Dg.CategoryManagement", @"devinite.*", RuleKind.Allowed,
                    description: "This is an exception rule."),
                new ReferenceRule(@"Dg.Finance", @"devinite.*", RuleKind.Allowed, description: "This is an exception rule."),
                new ReferenceRule(@"Dg.HumanResources", @"devinite.*", RuleKind.Allowed,
                    description: "This is an exception rule."),
                new ReferenceRule(@"Dg.Pdm*", @"devinite.*", RuleKind.Allowed, description: "This is an exception rule."),
                new ReferenceRule(@"Dg.MessageReceivers", @"devinite.*", RuleKind.Allowed,
                    description: "This is an exception rule."),
                new ReferenceRule(@"Dg.Wms*", @"devinite.*", RuleKind.Allowed, description: "This is an exception rule."),
                new ReferenceRule(@"devinite.*", @"Dg.ShopProductBoxRepository", RuleKind.Allowed,
                    description: "Let's be honest. This shit is never going away :("),

                // please clean this up!
                new ReferenceRule(@"Dg.*.Contracts", @"Dg.DgConsolidate", RuleKind.Allowed,
                    description: "Contracts shouldn't need to reference DgConsolidate. Please take a look at those."),
                new ReferenceRule(@"Dg.*.Contracts", @"Dg.InversionOfControl", RuleKind.Allowed,
                    description: "Contracts are not supposed to do IoC stuff!"),
                new ReferenceRule(@"Dg.DbCache.DataAccess", @"devinite.Db", RuleKind.Allowed,
                    description: "DbCache will get its own DataAccess. Until then, it stays like this."),
                new ReferenceRule(@"Dg.Erp*", @"devinite.*", RuleKind.Allowed,
                    description: "This stuff will be cleaned up. Trust me, I'm an engineer :D"),
                new ReferenceRule(@"Dg.Personalization", @"devinite.Db", RuleKind.Allowed,
                    description: "Hey Darwin, please don't devolve :("),
                new ReferenceRule(@"devinite.PortalSystem.*", @"Dg.Personalization", RuleKind.Allowed,
                    description: "Hey Darwin, please don't devolve :("),
                new ReferenceRule(@"Dg.ShopProductCatalog", @"devinite.ClassLibrary", RuleKind.Allowed,
                    description: "Octopussy will clean this up."),
                new ReferenceRule(@"devinite.*", @"Dg.DgConsolidate*", RuleKind.Allowed,
                    description: "DgConsolidate must die."),
                new ReferenceRule(@"devinite.*", @"Dg.InversionOfControl", RuleKind.Allowed,
                    description:
                    "InversionOfControl should get Framework suffix, and everybody is allowed to reference framework."),
                new ReferenceRule(@"devinite.PortalSystem.*", @"Dg.FeatureToggle*", RuleKind.Allowed,
                    description: "This is odd."),
                new ReferenceRule(@"devinite.PortalSystem.*", @"Dg.*.Abstractions", RuleKind.Allowed,
                    description: "This should be fine but let's look at it again later.."),
                new ReferenceRule(@"devinite.PortalSystem.*", @"Dg.Erp", RuleKind.Allowed,
                    description: "Dg.Erp should get Framework suffix."),
                new ReferenceRule(@"devinite.ClassLibrary", @"Dg.Pdm*", RuleKind.Allowed, description: "Pdm will die soon."),
                new ReferenceRule(@"devinite.PortalSystem.ClassLibrary", @"Dg.Wms.DataAccess", RuleKind.Allowed,
                    description: "Holly shit! This must go!"),
            };

            var runner = new ReferencesRulerRunner(
                extractor: new CsprojReferencesExtractor(),
                referencesRuler: new ReferencesRuler(
                    patternParser: new WildcardPatternParser(),
                    rules: rules),
                filesRunner: new ProjectFilesRunner(
                    solutionPath: DevinitePath,
                    filesExtension: "*.csproj"));

            System.Console.WriteLine(runner.GetComplaintsForProjectReferences());
        }
    }
}