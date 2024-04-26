namespace logicpos.shared.Classes.Others
{
    //Response Objects
    public class ValidateMaxLenghtMaxWordsResult
    {
        public int Length { get; set; } = 0;

        public int Words { get; set; } = 0;

        public string LabelText { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
    }
}
