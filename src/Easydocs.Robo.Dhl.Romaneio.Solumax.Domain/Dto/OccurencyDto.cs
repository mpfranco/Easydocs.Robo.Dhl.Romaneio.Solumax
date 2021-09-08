using FileHelpers;
using System;
using System.Globalization;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Dto
{
    [DelimitedRecord(";")]
    public class OccurencyDto
    {
        public OccurencyDto()
        {

        }
        public OccurencyDto(long protocolo, string protocoloExterno, string cliente, string valor, string dataTransacao, string descricao)
        {
            Protocolo = protocolo;
            ProtocoloExterno = protocoloExterno;
            Cliente = cliente;
            Valor = valor;
            DataTransacao = dataTransacao;
            Descricao = descricao;
        }


        public long Protocolo { get; set; }
        public string ProtocoloExterno { get; set; }
        public string Cliente { get; set; }

        [FieldConverter(typeof(CurrencyConverter))]
        public string Valor { get; set; }
        
        public string DataTransacao { get; set; }
        public string Descricao { get; set; }

    }
    public class CurrencyConverter : ConverterBase
    {
        private NumberFormatInfo nfi = new NumberFormatInfo();

        public CurrencyConverter()
        {
            nfi.NegativeSign = "-";
            nfi.NumberDecimalSeparator = ".";
            nfi.NumberGroupSeparator = ",";
            nfi.CurrencySymbol = "R$";
        }

        public override object StringToField(string from)
        {
            return decimal.Parse(from, NumberStyles.Currency, nfi);
        }
    }
}
