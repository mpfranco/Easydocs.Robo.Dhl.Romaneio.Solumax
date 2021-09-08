using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.DomainObjects;
using System;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Interfaces.IRepository.Base
{
    public interface IRepositoryBase<TEntity> : IDisposable where TEntity : IAggregateRoot
    {

    }
}
