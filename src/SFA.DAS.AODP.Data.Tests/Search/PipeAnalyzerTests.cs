using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using Moq;
using SFA.DAS.AODP.Data.Search;

namespace SFA.DAS.AODP.Data.UnitTests.Search;

public class PipeAnalyzerTests
{
    [Fact]
    public void Analyzer_SplitsOnPipe_And_LowerCasesTokens()
    {
        // Arrange
        const string input = "One|TWO|Three";
        using var analyzer = new PipeAnalyzer();

        // Act
        var tokens = GetTokensFromAnalyzer(analyzer, "field", new StringReader(input));

        // Assert
        Assert.Equal(new[] { "one", "two", "three" }, tokens);
    }

    [Fact]
    public void Analyzer_PreservesSpacesExceptPipe()
    {
        // Arrange
        const string input = "A B|C D";
        using var analyzer = new PipeAnalyzer();

        // Act
        var tokens = GetTokensFromAnalyzer(analyzer, "field", new StringReader(input));

        // Assert
        Assert.Equal(new[] { "a b", "c d" }, tokens);
    }

    [Fact]
    public void Analyzer_OnlyPipes_ReturnsNoTokens()
    {
        // Arrange
        const string input = "|||";
        using var analyzer = new PipeAnalyzer();

        // Act
        var tokens = GetTokensFromAnalyzer(analyzer, "field", new StringReader(input));

        // Assert
        Assert.Empty(tokens);
    }

    [Fact]
    public void Analyzer_UsesProvidedTextReader_ReadIsCalled()
    {
        // Arrange
        var underlying = new StringReader("x|y");

        var mockReader = new Mock<TextReader>(MockBehavior.Loose);

        mockReader
            .Setup(m => m.Read(It.IsAny<char[]>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((char[] buffer, int index, int count) => underlying.Read(buffer, index, count));

        using var analyzer = new PipeAnalyzer();

        // Act
        var tokens = GetTokensFromAnalyzer(analyzer, "field", mockReader.Object);

        // Assert
        Assert.Equal(new[] { "x", "y" }, tokens);
        mockReader.Verify(m => m.Read(It.IsAny<char[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
    }

    private static List<string> GetTokensFromAnalyzer(Analyzer analyzer, string fieldName, TextReader reader)
    {
        var tokens = new List<string>();
        using var tokenStream = analyzer.GetTokenStream(fieldName, reader);
        var termAttr = tokenStream.AddAttribute<ICharTermAttribute>();

        tokenStream.Reset();
        try
        {
            while (tokenStream.IncrementToken())
            {
                tokens.Add(termAttr.ToString());
            }

            tokenStream.End();
        }
        finally
        {
            tokenStream.Dispose();
        }

        return tokens;
    }
}
