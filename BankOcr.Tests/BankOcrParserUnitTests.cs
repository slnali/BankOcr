using FluentAssertions;
using BankOcr;

namespace BankOcr.Tests
{
    using FluentAssertions;
    using BankOcr;

    [TestFixture]
    public class BankOcrParserUnitTests
    {
        private BankOcrParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new BankOcrParser();
        }

        // Add your test methods here

        [TestCase("   ", "  |", "  |", 1)]
        [TestCase(" _ ", "| |", "|_|", 0)]
        [TestCase(" _ ", " _|", "|_ ", 2)]
        [TestCase(" _ ", " _|", " _|", 3)]
        [TestCase(" _ ", "|_ ", " _|", 5)]
        [TestCase(" _ ", "|_ ", "|_|", 6)]
        [TestCase(" _ ", "  |", "  |", 7)]
        [TestCase(" _ ", "|_|", "|_|", 8)]
        [TestCase(" _ ", "|_|", " _|", 9)]
        public void TestGetNumber(string firstLine, string secondLine, string thirdLine, int expected)
        {
            // Act
            var result = _parser.GetNumber(firstLine, secondLine, thirdLine);

            // Assert
            result.Should().Be(expected.ToString());

        }

        [TestCase("   ", " _|", "  |", 1)]
        [TestCase("   ", "| |", "|_|", 0)]
        [TestCase(" _ ", " _ ", " _|", 3)]
        public void TestGetNumberUnknowns(string firstLine, string secondLine, string thirdLine, int expected)
        {
            // Act
            var result = _parser.GetNumber(firstLine, secondLine, thirdLine);

            // Assert
            result.Should().Be(expected.ToString());
        }
    }
}