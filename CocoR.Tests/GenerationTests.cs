using at.jku.ssw.Coco;

namespace CocoR.Tests;

[TestClass]
public sealed class GenerationTests
{
    public TestContext TestContext { get; set; }

    public static IEnumerable<string> Grammars => ValidGrammars.Concat(InvalidGrammars);

    public static string[] ValidGrammars { get; } =
    {
        "TestAlts",
        "TestOpts",
        "TestOpts1",
        "TestIters",
        "TestEps",
        "TestAny",
        "TestAny1",
        "TestSync",
        "TestSem",
        "TestWeak",
        "TestChars",
        "TestTokens",
        "TestComments",
        "TestDel",
        "TestLL1",
        "TestResOK",
        "TestCasing",
        "TestResIllegal"
    };

    public static string[] InvalidGrammars { get; } =
    {
        "TestTokens1",
        "TestTerminalizable",
        "TestComplete",
        "TestReached",
        "TestCircular"
    };


    [TestMethod]
    [DynamicData(nameof(Grammars), DynamicDataSourceType.Property)]
    public void TestOutput(string grammar)
    {
        var testSuitePath = GetTestSuitePath();
        var outputPath = GetOutputPath(grammar);
        Generate(grammar, testSuitePath, outputPath);
        AssertFilesAreEqual(grammar, testSuitePath, outputPath, "output.txt");
    }

    [TestMethod]
    [DynamicData(nameof(Grammars), DynamicDataSourceType.Property)]
    public void TestTrace(string grammar)
    {
        var testSuitePath = GetTestSuitePath();
        var outputPath = GetOutputPath(grammar);
        Generate(grammar, testSuitePath, outputPath);
        AssertFilesAreEqual(grammar, testSuitePath, outputPath, "trace.txt");
    }

    [TestMethod]
    [DynamicData(nameof(ValidGrammars), DynamicDataSourceType.Property)]
    public void TestScanner(string grammar)
    {
        var testSuitePath = GetTestSuitePath();
        var outputPath = GetOutputPath(grammar);
        Generate(grammar, testSuitePath, outputPath);
        AssertFilesAreEqual(grammar, testSuitePath, outputPath, "Scanner.cs");
    }

    [TestMethod]
    [DynamicData(nameof(ValidGrammars), DynamicDataSourceType.Property)]
    public void TestParser(string grammar)
    {
        var testSuitePath = GetTestSuitePath();
        var outputPath = GetOutputPath(grammar);
        Generate(grammar, testSuitePath, outputPath);
        AssertFilesAreEqual(grammar, testSuitePath, outputPath, "Parser.cs");
    }

    private static string GetTestSuitePath()
    {
        var rootPath = Path.GetDirectoryName(typeof(GenerationTests).Assembly.Location);
        ArgumentNullException.ThrowIfNull(rootPath);
        return Path.Combine(rootPath, "TestSuite");
    }

    private string GetOutputPath(string testDirectoryName)
    {
        var testRunResultsDirectory = TestContext.TestRunResultsDirectory;
        ArgumentNullException.ThrowIfNull(testRunResultsDirectory);
        var testName = TestContext.TestName;
        ArgumentNullException.ThrowIfNull(testName);
        return Path.Combine(testRunResultsDirectory, testDirectoryName, testName);
    }

    private static void Generate(string grammar, string testSuitePath, string outputPath)
    {
        var grammarFile = Path.Combine(testSuitePath, $"{grammar}.ATG");
        var traceFile = Path.Combine(testSuitePath, "trace.txt");
        var outputFile = Path.Combine(outputPath, "output.txt");
        Directory.CreateDirectory(outputPath);
        using (var writer = new StreamWriter(outputFile))
        {
            var consoleOut = Console.Out;
            Console.SetOut(writer);
            try
            {
                Coco.Generate(grammarFile, null, null, null, outputPath, emitLines: false);
            }
            finally
            {
                Console.SetOut(consoleOut);
            }
        }

        File.Move(traceFile, Path.Combine(outputPath, "trace.txt"));
        var output = File.ReadAllText(outputFile);
        File.WriteAllText(outputFile, output.Replace(traceFile, "trace.txt"));
    }

    private static void AssertFilesAreEqual(string grammar, string testSuitePath, string outputPath, string file)
    {
        var expected = File.ReadAllText(Path.Combine(testSuitePath, $"{grammar}_{file}"));
        var actual = File.ReadAllText(Path.Combine(outputPath, file));
        Assert.AreEqual(expected, actual.ReplaceLineEndings("\r\n"));
    }
}
