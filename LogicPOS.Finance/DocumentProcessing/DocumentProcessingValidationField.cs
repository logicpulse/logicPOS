using System;

namespace LogicPOS.Finance.DocumentProcessing
{
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Inner Enum ProcessFinanceDocumentValidationField

    public class DocumentProcessingValidationField
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Name { get; }
        private object _value;
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Rule { get; }
        private readonly bool _required;
        public bool Required
        {
            get { return _required; }
        }

        public Action Action { get; }

        public DocumentProcessingValidationField(string pName, object pValue, string pRule, bool pRequired)
            : this(pName, pValue, pRule, pRequired, null)
        {
        }

        public DocumentProcessingValidationField(string pName, object pValue, string pRule, bool pRequired, Action pAction)
        {
            Name = pName;
            _value = pValue;
            Rule = pRule;
            _required = pRequired;
            Action = pAction;
        }

        public bool Validate()
        {
            bool result = false;

            try
            {

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
