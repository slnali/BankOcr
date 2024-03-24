
using System.Text;

namespace BankOcr
{
    public class BankOcrParser
    {
        private Dictionary<string, int> ocrToNumberMappings = new Dictionary<string, int>()
            {
                { " _ | ||_|", 0 },
                { "     |  |", 1 },
                { " _  _||_ ", 2 },
                { " _  _| _|", 3 },
                { "   |_|  |", 4 },
                { " _ |_  _|", 5 },
                { " _ |_ |_|", 6 },
                { " _   |  |", 7 },
                { " _ |_||_|", 8 },
                { " _ |_| _|", 9 }
            };

        public void ProcessFile(string file)
        {
            var fileLines = File.ReadLines(file).ToList();
            var accountNumbers = ParseOcrNumbers(fileLines);
            GenerateOutputFile(file, accountNumbers);
        }

        private void GenerateOutputFile(string file, List<List<INumber>> accountNumbers)
        {
            List<string> fileOutput = new();
            foreach (var accountNumber in accountNumbers)
            {
                if (ChecksumValidator.IsCheckSumValid(accountNumber))
                {
                    fileOutput.Add(convertToAccountNumberText(accountNumber));
                }

                else
                {
                    HandleInvalidNumber(fileOutput, accountNumber);
                }
            }

            var fileName = Path.GetFileName(file);
            var outputFile = $"output-{fileName}";
            Console.WriteLine($"Writing output to {outputFile}");
            File.WriteAllLines($"{outputFile}", fileOutput);
        }

        private void HandleInvalidNumber(List<string> fileOutput, List<INumber> accountNumber)
        {
            var potentialAccountNumbers = ChecksumValidator.FixInvalidNumber(accountNumber);
            if (potentialAccountNumbers.Count == 0)
            {
                fileOutput.Add(convertToAccountNumberText(accountNumber) + " ILL");
            }
            else if (potentialAccountNumbers.Count == 1)
            {
                fileOutput.Add(potentialAccountNumbers[0]);
            }
            else
            {
                // AMB output
                string ambOutput = $"{convertToAccountNumberText(accountNumber)} AMB [{string.Join(',', potentialAccountNumbers.Select(n => $"'{n}'"))}]";
                fileOutput.Add(ambOutput);
            }
        }

        private string convertToAccountNumberText(List<INumber> accountNumber)
        {
            return string.Join("", accountNumber.Select(n => (n as Number).Value));
        }

        private List<List<INumber>> ParseOcrNumbers(List<string> rows)
        {
            List<List<INumber>> rowsOfAccountNumbers = new();
            for (int startOfNewEntry = 0; startOfNewEntry <= rows.Count(); startOfNewEntry += 4)
            {
                List<INumber> accountNumber = new();

                for (int c = 0; c <= 24; c += 3)
                {
                    // get 3 chars from each line
                    var num = GetNumber(
                        rows[startOfNewEntry].Substring(c, 3),
                        rows[startOfNewEntry + 1].Substring(c, 3),
                        rows[startOfNewEntry + 2].Substring(c, 3));
                    accountNumber.Add(num);
                }
                rowsOfAccountNumbers.Add(accountNumber);
            }


            return rowsOfAccountNumbers;
        }

        public INumber GetNumber(string firstLine, string secondLine, string thirdLine)
        {
            string numberKey = $"{firstLine}{secondLine}{thirdLine}";

            if (ocrToNumberMappings.TryGetValue(numberKey, out int number))
            {
                return new Number() { Value = number };
            }

            // HandleIllNumber
            return GetPossibleNumbers(firstLine, secondLine, thirdLine);
        }

        private PossibleNumber GetPossibleNumbers(string firstLine, string secondLine, string thirdLine)
        {
            Dictionary<int, int> possibleNumToCount = new();

            foreach (var ocrMapping in this.ocrToNumberMappings)
            {
                if (ocrMapping.Key.Substring(0, 3).Contains(firstLine))
                {
                    UpdatePossibleNumCount(possibleNumToCount, ocrMapping.Value);
                }

                if (ocrMapping.Key.Substring(3, 3).Contains(secondLine))
                {
                    UpdatePossibleNumCount(possibleNumToCount, ocrMapping.Value);
                }

                if (ocrMapping.Key.Substring(6, 3).Contains(thirdLine))
                {
                    UpdatePossibleNumCount(possibleNumToCount, ocrMapping.Value);
                }
            }

            int maxCountValue = possibleNumToCount.Values.Max();

            var possibleNums = possibleNumToCount.Where(pair => pair.Value == maxCountValue).Select(pair => pair.Key).ToList();

            // let the similarity function in checksum validator evaluate if other possible numbers match
            return new PossibleNumber() { Value = possibleNums };
        }

        private void UpdatePossibleNumCount(Dictionary<int, int> possibleNumToCount, int number)
        {
            if (possibleNumToCount.ContainsKey(number))
            {
                possibleNumToCount[number] += 1;
            }
            else
            {
                possibleNumToCount[number] = 1;
            }
        }
    }
}