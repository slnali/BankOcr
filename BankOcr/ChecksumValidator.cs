using System;

namespace BankOcr
{
    public class ChecksumValidator
    {

        public static bool IsCheckSumValid(List<INumber> accountNumber)
        {
            if (ContainsUnknownIllNumber(accountNumber))
            {
                // if we have a list in our account number that indicates an invalid number
                return false;
            }

            int sum = 0;
            int accountNumLength = 9;
            foreach (var num in accountNumber)
            {
                sum += accountNumLength * (num as Number).Value;
                accountNumLength--;
            }
            return sum % 11 == 0;
        }

        private static bool ContainsUnknownIllNumber(List<INumber> accountNumber)
        {
            return accountNumber.Any(n => n.GetType() == typeof(PossibleNumber));
        }

        public static int ParseCharAsNumber(char num)
        {
            return int.Parse(num.ToString());
        }

        public static List<string> FixInvalidNumber(List<INumber> accountNumber)
        {

            if (ContainsUnknownIllNumber(accountNumber))
            {
                return GetIllNumbers(accountNumber);
            }

            return GetErrNumbers(accountNumber);
        }

        private static List<string> GetIllNumbers(List<INumber> accountNumber)
        {
            List<string> possibleNumbers = new();

            // find the index where list is - iterate through list and check if checksum valid for any of them
            for (int i = 0; i < accountNumber.Count; i++)
            {
                if (accountNumber[i] is PossibleNumber)
                {
                    foreach (var potentialNum in (accountNumber[i] as PossibleNumber).Value)
                    {
                        accountNumber[i] = new Number() { Value = potentialNum };
                        if (IsCheckSumValid(accountNumber))
                        {
                            var possibleNumber = string.Join("", accountNumber.Select(n => (n as Number).Value));
                            possibleNumbers.Add(possibleNumber);
                        }
                    }
                }
                continue;
            }
            return possibleNumbers;

        }

        private static List<string> GetErrNumbers(List<INumber> accountNumber)
        {
            List<string> possibleNumbers = new();

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

            // invalid checksum
            for (int i = 0; i < accountNumber.Count; i++)
            {

                // get similar numbers

                var similarNums = similarNumbers[(accountNumber[i] as Number).Value];


                if (similarNums.Count != 0)
                {
                    var existingNum = accountNumber[i];
                    foreach (var num in similarNums)
                    {
                        accountNumber[i] = new Number() { Value = num };
                        if (IsCheckSumValid(accountNumber))
                        {
                            var possibleNumber = string.Join("", accountNumber.Select(n => (n as Number).Value));
                            possibleNumbers.Add(possibleNumber);
                        }
                    }
                    accountNumber[i] = existingNum;
                }
            }

            return possibleNumbers;
        }
    }
}