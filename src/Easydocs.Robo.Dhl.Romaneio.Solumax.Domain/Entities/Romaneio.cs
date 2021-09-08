using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.DomainObjects;
using System;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Entities
{
    public class romaneio : IAggregateRoot
    {
        public romaneio()
        {

        }

        public romaneio(long id ,DateTime dt_Download, long nF, string cNPJ, string serie, string download, DateTime emissao, string nr_romaneio)
        {
            Id = id;
            Dt_Download = dt_Download;
            NF = nF;
            CNPJ = cNPJ;
            Serie = serie;
            Download = download;
            Emissao = emissao;
            Nr_romaneio = nr_romaneio;

        }

        public romaneio(DateTime dt_Download, long nF, string cNPJ, string serie, string nr_romaneio)
        {            
            Dt_Download = dt_Download;
            NF = nF;
            CNPJ = cNPJ;
            Serie = serie;
            Nr_romaneio = nr_romaneio;

        }

        public long Id { get; private set; }
        public DateTime Dt_Download { get; set; }
        public long NF { get; set; }
        public string CNPJ  { get; set; }
        public string Serie { get; set; }
        public string Download { get; set; }
        public DateTime Emissao { get; set; }
        public string DATA_PROXIMA_CAPTURA { get; set; }
        public string Nr_romaneio { get; set; }
        public int Nr_Paginas { get; set; }
        public void AssociateId(long id)
        {
            Id = id;
        }
    }
}
