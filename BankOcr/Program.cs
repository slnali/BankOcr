namespace BankOcr
{
    public class BankOcrProgram
    {


        public static void Main(string[] args)
        {
            var bankOcrParser = new BankOcrParser();

            foreach (var file in args)
            {
                bankOcrParser.ProcessFile(file);
            }
        }

    }
}