using System;

namespace BankOcr
{
    public class ChecksumValidator
    {

        public static bool IsCheckSumValid(string accountNumber)
        {
            int sum = 0;
            int i = 9;
            foreach (var num in accountNumber)
            {
                sum += i * ParseCharAsNumber(num);
                i--;
            }
            return sum % 11 == 0;
        }

        public static int ParseCharAsNumber(char num)
        {
            return int.Parse(num.ToString());
        }

        public static List<string> FixInvalidNumber(string accountNumber)
        {

            Dictionary<int, List<int>> similarNumbers = new Dictionary<int, List<int>>()
            {
                { 0, new List<int>() { 8 } },
                { 1, new List<int>() { 7 } },
                { 2, new List<int>() {  } },
                { 3, new List<int>() { 9  } },
                { 4, new List<int>() {   } },
                { 5, new List<int>() { 6, 9  } },
                { 6, new List<int>() { 5, 8  } },
                { 7, new List<int>() { 1  } },
                { 8, new List<int>() { 0, 6, 9  } },
                { 9, new List<int>() { 3, 8, 5  } }
            };

            List<string> possibleNumbers = new();

            char[] accountNumberArray = accountNumber.ToCharArray();

            // invalid checksum
            for (int i = 0; i < accountNumberArray.Length; i++)
            {
                // get similar numbers
                var similarNums = similarNumbers[ParseCharAsNumber(accountNumberArray[i])];

                if (similarNums.Count != 0)
                {
                    var existingNum = accountNumberArray[i];
                    foreach (var num in similarNums)
                    {
                        accountNumberArray[i] = (char)(num + 48); //convert ascii to num char
                        var possibleNumber = new string(accountNumberArray);
                        if (IsCheckSumValid(possibleNumber))
                        {
                            possibleNumbers.Add(possibleNumber);
                        }
                    }
                    accountNumberArray[i] = existingNum;
                }
            }

            return possibleNumbers;
        }

    }
}