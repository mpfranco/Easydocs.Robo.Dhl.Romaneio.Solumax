using Easydocs.Robo.Dhl.Romaneio.Solumax.Application.Validations;
using System;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Commands.Romaneio.UpdateRomaneio
{
    public class UpdateRomaneioCommand : CustomValidation
    {
        public UpdateRomaneioCommand()
        {

        }
        public UpdateRomaneioCommand(DateTime dt_Download, long nF, string cNPJ, string serie, string download)
        {
            Dt_Download = dt_Download;
            NF = nF;
            CNPJ = cNPJ;
            Serie = serie;
            Download = download;
        }

        public UpdateRomaneioCommand(long id, DateTime dt_Download, long nF, string cNPJ, string serie, string download, DateTime emissao, int nr_Paginas)
        {
            Id = id;
            Dt_Download = dt_Download;
            NF = nF;
            CNPJ = cNPJ;
            Serie = serie;
            Download = download;
            Emissao = emissao;
            Nr_Paginas = nr_Paginas;

        }

        public long Id { get; set; }
        public DateTime Dt_Download { get; set; }
        public long NF { get; set; }
        public string CNPJ { get; set; }
        public string Serie { get; set; }
        public string Download { get; set; }
        public DateTime Emissao { get; set; }
        public string Nr_romaneio { get; set; }
        public int Nr_Paginas { get; set; }
        public void AssociateId(long id)
        {
            Id = id;
        }
    }
}
