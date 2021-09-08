using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.DomainObjects;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Interfaces.IRepository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Settings;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Data.Repository.Base
{
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : IAggregateRoot
    {
        //private readonly SqlConnection _connection;
        protected readonly NpgsqlConnection _connection;


        protected RepositoryBase(IConfiguration configuration)
        {
            //_connection = new SqlConnection(configuration.GetSection("ConnectionStrings:RoboVazFiel").Value);
            _connection = new NpgsqlConnection(configuration.GetSection("ConnectionStrings:RoboVazFiel").Value);

            if (_connection.State != ConnectionState.Open && _connection.State != ConnectionState.Connecting)
                _connection.Open();
        }

        protected int TimeOut { get; set; } = 200;

        protected virtual async Task<T> ExecuteProcedureScalarAsync<T>(string name, object parameters)
        {
            var retorno = _connection.ExecuteScalarAsync<T>(name, parameters, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut);
            return await retorno;

        }
        protected virtual async Task<T> ExecuteScalarAsync<T>(string name, object parameters)
        {
            var retorno = _connection.ExecuteScalarAsync<T>(name, parameters);
            return await retorno;

        }
        protected virtual Task ExecuteProcedureAsync(string name, object parameters)
        {
            return _connection.ExecuteAsync(name, parameters, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut);
        }
        protected virtual async Task<IEnumerable<T>> ProcedureAsync<T>(string name, object parameters)
        {
            var entidade = _connection.QueryAsync<T>(name, parameters, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut).ConfigureAwait(false);
            return await entidade;
        }
        public virtual async Task<TEntity> FindAsync(long id)
        {
            var entidade = await _connection.QueryAsync<TEntity>(PrepareParametroSelect<TEntity>(new { Id = id })).ConfigureAwait(false);
            return entidade.SingleOrDefault();
        }
        protected virtual async Task<T> FindAsync<T>(long id)
        {
            var entidade = await _connection.QueryAsync<T>(PrepareParametroSelect<TEntity>(new { Id = id })).ConfigureAwait(false);
            return entidade.SingleOrDefault();
        }
        protected virtual async Task<IEnumerable<TEntity>> GetAsync(object parameters, string tableName = null)
        {
            var entidade = await _connection.QueryAsync<TEntity>(PrepareParametroSelect<TEntity>(parameters, tableName));

            return entidade;
        }
        protected virtual async Task<IEnumerable<TEntity>> GetExecuteQueryAsync(string query)
        {

            var entidade = await _connection.QueryAsync<TEntity>(query, commandTimeout: TimeOut).ConfigureAwait(false);
            return (IList<TEntity>)entidade;
        }
        protected virtual async Task<IEnumerable<TEntity>> QueryAsync(string sql, object param = null)
        {
            var entidade = await _connection.QueryAsync<TEntity>(sql, param).ConfigureAwait(false);
            return entidade;
        }
        protected virtual async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            var entidade = await _connection.QueryAsync<T>(sql, param);
            return entidade;
        }



        protected virtual void Execute(string sql, object parameters)
        {
            _connection.Execute(sql, parameters);
        }
        protected virtual Task ExecuteAsync(string sql, object parameters)
        {
            return _connection.ExecuteAsync(sql, parameters);
        }
        protected virtual Task<int> ExecuteUpdateAsync(string sql, object parameters)
        {
            return _connection.ExecuteAsync(sql, parameters);
        }
        private object AjustaValor(object valor)
        {


            if (valor.GetType().Name == "Boolean")
            {
                if (valor.ToString() == "False")
                {
                    return "0";
                }
                else if (valor.ToString() == "True")
                {
                    return "1";
                }

            }

            return valor;
        }
        private string PrepareParametroSelect<T>(object parameters = null, string tableName = null)
        {
            StringBuilder sb = new StringBuilder();
            tableName = tableName ?? typeof(T).Name;



            sb.AppendFormat("select * from {0}", tableName);
            if (parameters != null)
            {
                var fisrtItem = parameters.GetType().GetProperties().FirstOrDefault();
                foreach (PropertyInfo item in parameters.GetType().GetProperties())
                {
                    var value = AjustaValor((item.GetValue(parameters, null).GetType()).Name == "String" ? "'" + item.GetValue(parameters, null) + "'" : item.GetValue(parameters, null));
                    sb.AppendFormat(" {0} {1} = {2}", (item == fisrtItem ? "WHERE" : "AND"), item.Name, value);
                }
            }

            return sb.ToString();
        }
        protected virtual Task<TKey> SaveAsync<TKey, T>(T entity)
        {

            return _connection.InsertAsync<TKey, T>(entity);
        }
        protected virtual async Task<int> UpdateAsync<T>(T entity)
        {
            return await _connection.UpdateAsync<T>(entity);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_connection.State != ConnectionState.Closed)
                _connection.Close();

            _connection.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}