using System;

namespace logicpos.financial.library.Results
{
    //Helper Class to Return Bool and Bool and Exception if Exists
    public class FrameworkCallsResult
    {
        private bool _result = false;
        public bool Result
        {
            get { return _result; }
            set { _result = value; }
        }
        private Exception _exception;
        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }
        private string _output;
        public string Output
        {
            get { return _output; }
            set { _output = value; }
        }
    }
}
