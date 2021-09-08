using System;
using System.Collections.Generic;
using System.Text;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.DomainObjects
{
    public class Token
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string errorCode { get; set; }
        public string message { get; set; }
    }
}
