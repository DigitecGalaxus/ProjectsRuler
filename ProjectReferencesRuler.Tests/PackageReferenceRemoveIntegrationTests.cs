using ProjectReferencesRuler.ProjectParsing;
using ProjectReferencesRuler.ProjectRunners;
using ProjectReferencesRuler.Rules;
using ProjectReferencesRuler.Rules.References;
using Xunit;

namespace ProjectReferencesRuler
{
    /// <summary>
    /// Integration tests that exercise the full stack (extractor → ruler → runner)
    /// using real XML test project files - mirroring a typical README use-case.
    ///
    /// Regression coverage for: <PackageReference Remove="..." /> causing an
    /// ArgumentNullException because the element has no Include attribute, leaving
    /// Reference.To as null, which then crashes Regex.IsMatch.
    /// </summary>
    public class PackageReferenceRemoveIntegrationTests
    {
        /// <summary>
        /// Reproduces the production crash: processing Dg.Component.xml which contains
        ///   &lt;PackageReference Remove="XUnit" /&gt;
        /// must not throw an ArgumentNullException.
        /// </summary>
        [Fact]
        public void GetComplaintsForPackageReferences_WhenProjectContainsPackageReferenceRemoveElement_DoesNotThrow()
        {
            // A catch-all forbidden rule is used to force every extracted reference
            // through the full regex-matching path, which is where the crash occurred.
            var runner = CreateRunner(
                new ReferenceRule(
                    @"*",
                    @"*",
                    RuleKind.Forbidden,
                    description: "Catch-all rule to exercise the full evaluation path."
                )
            );

            var exception = Record.Exception(() => runner.GetComplaintsForPackageReferences());

            Assert.Null(exception);
        }

        /// <summary>
        /// A &lt;PackageReference Remove="XUnit" /&gt; element should be silently skipped
        /// and never surfaced as a reference or a rule violation.
        /// </summary>
        [Fact]
        public void GetComplaintsForPackageReferences_WhenProjectContainsPackageReferenceRemoveElement_RemoveElementIsIgnoredAndNotReportedAsViolation()
        {
            var runner = CreateRunner(
                new ReferenceRule(
                    @"*",
                    @"XUnit",
                    RuleKind.Forbidden,
                    description: "XUnit package references are forbidden."
                )
            );

            var complaints = runner.GetComplaintsForPackageReferences();

            Assert.DoesNotContain("XUnit", complaints ?? string.Empty);
        }

        private static ReferencesRulerRunner CreateRunner(params ReferenceRule[] rules) =>
            new ReferencesRulerRunner(
                extractor: new CsprojReferencesExtractor(),
                referencesRuler: new ReferencesRuler(
                    patternParser: new WildcardPatternParser(),
                    rules: rules
                ),
                filesRunner: new ProjectFilesRunner(
                    solutionPath: @"../../../TestProjectFiles/",
                    filesExtension: "*.xml"
                )
            );
    }
}
