namespace logicpos.shared.Classes.Others
{
    //Response Objects
    public class ValidateMaxLenghtMaxWordsResult
    {
        int _length = 0;
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }
        int _words = 0;
        public int Words
        {
            get { return _words; }
            set { _words = value; }
        }
        string _labelText = string.Empty;
        public string LabelText
        {
            get { return _labelText; }
            set { _labelText = value; }
        }
        string _text = string.Empty;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}
