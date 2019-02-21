namespace AHK.Configuration
{
    public class ConsoleMessageGraderConfig : IConfigValidator
    {
        public string ValidationCode { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(ValidationCode))
                throw new System.Exception("ValidationCode beallitas hianyzik a ConsoleMessageGrader-bol");
        }
    }
}
