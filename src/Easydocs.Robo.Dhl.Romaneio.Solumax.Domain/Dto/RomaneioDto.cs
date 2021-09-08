using System.ComponentModel.DataAnnotations;


namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Dto
{
    public class RomaneioDto
    {
        
        //[Display(Name = "Nine-Digit Document Number")]
        public string NF { get; set; }
        
        public string Serie { get; set; }   
        
        //[Display(Name = "CNPJ Nr. Bus. Place")]
        public string CNPJ { get; set; }

        public string DOWNLOAD { get; set; }

        public string Nr_romaneio { get; set; }

    }
}
