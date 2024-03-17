
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
            var rows = File.ReadLines(file).ToList();
            List<string> output = ParseOcrNumbers(rows);
            GenerateOutputFile(file, output);
        }

        private static void GenerateOutputFile(string file, List<string> output)
        {
            List<string> fileOutput = new();
            foreach (var o in output)
            {
                if (ChecksumValidator.IsCheckSumValid(o))
                {
                    fileOutput.Add(o);
                }

                else
                {
                    var nums = ChecksumValidator.FixInvalidNumber(o);
                    if (nums.Count == 0)
                    {
                        fileOutput.Add(o + " ILL");
                    }
                    else if (nums.Count == 1)
                    {
                        fileOutput.Add(nums[0]);
                    }
                    else
                    {
                        // AMB output
                        string ambOutput = $"{o} AMB [{string.Join(',', nums.Select(n => $"'{n}'"))}]";
                        fileOutput.Add(ambOutput);
                    }
                }
            }

            var fileName = Path.GetFileName(file);
            var outputFile = $"output-{fileName}";
            Console.WriteLine($"Writing output to {outputFile}");
            File.WriteAllLines($"{outputFile}", fileOutput);
        }

        private List<string> ParseOcrNumbers(List<string> rows)
        {
            List<string> output = new();
            for (int startOfNewEntry = 0; startOfNewEntry <= rows.Count(); startOfNewEntry += 4)
            {
                var sb = new StringBuilder();

                for (int c = 0; c <= 24; c += 3)
                {
                    // get 3 chars from each line
                    var num = GetNumber(
                        rows[startOfNewEntry].Substring(c, 3),
                        rows[startOfNewEntry + 1].Substring(c, 3),
                        rows[startOfNewEntry + 2].Substring(c, 3));
                    sb.Append(num);
                }
                output.Add(sb.ToString());
            }

            return output;
        }

        public string GetNumber(string firstLine, string secondLine, string thirdLine)
        {
            string numberKey = $"{firstLine}{secondLine}{thirdLine}";

            if (ocrToNumberMappings.TryGetValue(numberKey, out int number))
            {
                return number.ToString();
            }

            // HandleIllNumber
            return DeterminePossibleNumber(firstLine, secondLine, thirdLine);
        }

        private string DeterminePossibleNumber(string firstLine, string secondLine, string thirdLine)
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
            return possibleNums[0].ToString();
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