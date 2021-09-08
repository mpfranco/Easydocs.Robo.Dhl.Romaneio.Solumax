using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Constants;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.DomainObjects
{
    public class ResultCommand
    {
        public ResultCommand()
        {

        }
        public ResultCommand(object data)
        {
            Data = data;
        }

        public ResultCommand(object data, StatusCode? statusCode)
        {
            Data = data;
            StatusCode = statusCode;
        }

        public object Data { get; set; }
        public StatusCode? StatusCode { get; set; }
    }

    public class ResultCommand<T>
    {
        public ResultCommand()
        {

        }
        public ResultCommand(T data)
        {
            Data = data;
        }
        public T Data { get; set; }
    }
}
