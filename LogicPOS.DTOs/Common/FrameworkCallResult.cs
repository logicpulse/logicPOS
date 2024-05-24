using System;

namespace LogicPOS.DTOs.Common
{
    //Helper Class to Return Bool and Bool and Exception if Exists
    public class FrameworkCallResult
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
