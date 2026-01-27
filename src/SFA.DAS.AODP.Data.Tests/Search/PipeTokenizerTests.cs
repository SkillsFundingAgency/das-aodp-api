using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Util;
using Moq;
using SFA.DAS.AODP.Data.Search;

namespace SFA.DAS.AODP.Data.UnitTests.Search;

public class PipeTokenizerTests
{
    [Fact]
    public void Tokens_SplitsOnPipe_ReturnsExpectedTokens()
    {
        // Arrange
        const string input = "one|two|three";
        using var tokenizer = new PipeTokenizer(LuceneVersion.LUCENE_48, new StringReader(input));

        // Act
        var tokens = CollectTokens(tokenizer);

        // Assert
        Assert.Equal(new[] { "one", "two", "three" }, tokens);
    }

    [Fact]
    public void Tokens_PreservesSpacesWhenNotPipe()
    {
        // Arrange
        const string input = "b c|d";
        using var tokenizer = new PipeTokenizer(LuceneVersion.LUCENE_48, new StringReader(input));

        // Act
        var tokens = CollectTokens(tokenizer);

        // Assert
        Assert.Equal(new[] { "b c", "d" }, tokens);
    }

    [Fact]
    public void Tokens_OnlyPipes_ReturnsNoTokens()
    {
        // Arrange
        const string input = "|||";
        using var tokenizer = new PipeTokenizer(LuceneVersion.LUCENE_48, new StringReader(input));

        // Act
        var tokens = CollectTokens(tokenizer);

        // Assert
        Assert.Empty(tokens);
    }

    [Fact]
    public void Constructor_WithMockedTextReader_WorksAndIsUsed()
    {
        // Arrange
        var underlying = new StringReader("x|y");

        var mockReader = new Mock<TextReader>(MockBehavior.Loose);

        mockReader
            .Setup(m => m.Read(It.IsAny<char[]>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((char[] buffer, int index, int count) => underlying.Read(buffer, index, count));

        using var tokenizer = new PipeTokenizer(LuceneVersion.LUCENE_48, mockReader.Object);

        // Act
        var tokens = CollectTokens(tokenizer);

        // Assert
        Assert.Equal(new[] { "x", "y" }, tokens);
        mockReader.Verify(m => m.Read(It.IsAny<char[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
    }

    private static List<string> CollectTokens(PipeTokenizer tokenizer)
    {
        var tokens = new List<string>();
        var termAttr = tokenizer.AddAttribute<ICharTermAttribute>();

        tokenizer.Reset();
        try
        {
            while (tokenizer.IncrementToken())
            {
                tokens.Add(termAttr.ToString());
            }

            tokenizer.End();
        }
        finally
        {
            tokenizer.Dispose();
        }

        return tokens;
    }
}
