using System;
using System.Collections.Generic;

namespace LogicPOS.Finance.DocumentProcessing
{
    public class DocumentProcessingValidationException : Exception
    {
        public Exception Exception { get; set; }
        private SortedDictionary<DocumentValidationErrorType, object> _exceptionErrors = new SortedDictionary<DocumentValidationErrorType, object>();
        public SortedDictionary<DocumentValidationErrorType, object> ExceptionErrors
        {
            get { return _exceptionErrors; }
            set { _exceptionErrors = value; }
        }

        public string ExceptionErrorsHasString
        {
            get
            {
                string result = string.Empty;
                foreach (var item in _exceptionErrors)
                {
                    result += string.Format("{0}{1}", item, Environment.NewLine);
                }
                return result;
            }
        }

        public DocumentProcessingValidationException(Exception pException, SortedDictionary<DocumentValidationErrorType, object> pExceptionErrors)
        {
            Exception = new Exception(pException.Message, pException);
            _exceptionErrors = pExceptionErrors;
        }
    }
}