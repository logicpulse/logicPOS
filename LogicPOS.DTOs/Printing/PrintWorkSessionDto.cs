using System;

namespace LogicPOS.DTOs.Printing
{
    public class PrintWorkSessionDto
    {
        public Guid Id { get; set; }
        public string PeriodType { get; set; }
        public string SessionStatus { get; set; }
        public string TerminalDesignation { get; set; }
        public DateTime StartDate { get; set; }

        public bool PeriodTypeIsDay => PeriodType == "Day";
        public bool PeriodTypeIsTerminal => PeriodType == "Terminal";
        public bool SessionStatusIsOpen => SessionStatus == "Open";
    }
}
