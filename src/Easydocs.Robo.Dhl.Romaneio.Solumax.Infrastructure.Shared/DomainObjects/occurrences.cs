using System;
using System.Collections.Generic;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.DomainObjects
{
    public class occurrences
    {
        public DateTime date { get; set; }
        public DateTime createDate { get; set; }
        public string descriptionMotivo { get; set; }
        public string externalNSU { get; set; }
        public long transactionId { get; set; }
        public string externalTerminal { get; set; }
        public string linhaDigitavel { get; set; }
        public decimal value { get; set; }

    }
}
