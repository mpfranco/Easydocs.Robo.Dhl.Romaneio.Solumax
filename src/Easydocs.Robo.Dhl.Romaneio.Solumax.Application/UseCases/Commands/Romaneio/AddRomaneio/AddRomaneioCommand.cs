using Easydocs.Robo.Dhl.Romaneio.Solumax.Application.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Commands.Romaneio.AddRomaneio
{
    public class AddRomaneioCommand : CustomValidation
    {

        public AddRomaneioCommand(DateTime dt_Download, long nF, string cNPJ, string serie, string nr_romaneio)
        {
            Dt_Download = dt_Download;
            NF = nF;
            CNPJ = cNPJ;
            Serie = serie;
            Nr_romaneio = nr_romaneio;
        }

        public long Id { get; private set; }
        public DateTime Dt_Download { get; private set; }
        public long NF { get; set; }
        public string CNPJ { get; set; }
        public string Serie { get; set; }
        public string Nr_romaneio { get; set; }
        public void AssociateId(long id)
        {
            Id = id;
        }
    }
}
