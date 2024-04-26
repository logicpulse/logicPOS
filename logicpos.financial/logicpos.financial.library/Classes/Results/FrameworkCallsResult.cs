using System;

namespace logicpos.financial.library.Results
{
    //Helper Class to Return Bool and Bool and Exception if Exists
    public class FrameworkCallsResult
    {
        public bool Result { get; set; } = false;

        public Exception Exception { get; set; }
        private string _output;
        public string Output
        {
            get { return _output; }
            set { _output = value; }
        }
    }
}
