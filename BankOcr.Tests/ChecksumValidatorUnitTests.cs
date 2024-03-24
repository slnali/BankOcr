using FluentAssertions;

namespace BankOcr.Tests
{
    [TestFixture]
    public class ChecksumValidatorUnitTests
    {

        public List<INumber> ConvertStringToListOfNumbers(string input)
        {
            List<INumber> output = new();
            foreach (var c in input)
            {
                output.Add(new Number() { Value = int.Parse(c.ToString()) });
            }

            return output;
        }

        [TestCase("711111111", true)]
        [TestCase("123456789", true)]
        [TestCase("490867715", true)]
        [TestCase("888888888", false)]
        [TestCase("490067715", false)]
        [TestCase("012345678", false)]
        public void TestIsCheckSumValid(string accountNumber, bool expected)
        {

            var result = ChecksumValidator.IsCheckSumValid(ConvertStringToListOfNumbers(accountNumber));
            result.Should().Be(expected);
        }


        [TestCase("111111111", new[] { "711111111" })]
        [TestCase("777777777", new[] { "777777177" })]
        [TestCase("200000000", new[] { "200800000" })]
        [TestCase("333333333", new[] { "333393333" })]
        [TestCase("333333333", new[] { "333393333" })]
        [TestCase("888888888", new[] { "888886888", "888888880", "888888988" })]
        [TestCase("555555555", new[] { "555655555", "559555555" })]
        [TestCase("666666666", new[] { "666566666", "686666666" })]
        [TestCase("999999999", new[] { "899999999", "993999999", "999959999" })]
        [TestCase("490067715", new[] { "490067115", "490067719", "490867715" })]
        public void TestHandleErrNum(string accountNumber, string[] expected)
        {
            var result = ChecksumValidator.FixInvalidNumber(ConvertStringToListOfNumbers(accountNumber));
            result.Should().BeEquivalentTo(expected);
        }
    }
}