using System;
using System.Collections.Generic;

namespace logicpos.financial.library.Classes.Finance
{
    public class ProcessFinanceDocumentValidationException : Exception
    {
        private Exception _exception;
        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }
        private SortedDictionary<FinanceValidationError, object> _exceptionErrors = new SortedDictionary<FinanceValidationError, object>();
        public SortedDictionary<FinanceValidationError, object> ExceptionErrors
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

        public ProcessFinanceDocumentValidationException(Exception pException, SortedDictionary<FinanceValidationError, object> pExceptionErrors)
        {
            _exception = new Exception(pException.Message, pException);
            _exceptionErrors = pExceptionErrors;
        }
    }
}