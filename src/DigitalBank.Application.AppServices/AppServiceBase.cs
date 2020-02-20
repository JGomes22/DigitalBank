using DigitalBank.Application.AppServices.Interfaces;
using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.DTO.Response;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Interfaces.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalBank.Application.AppServices
{
    public class AppServiceBase<T> : IAppServiceBase<T> where T : Entity
    {
        private readonly IServiceBase<T> _serviceBase;
        public AppServiceBase(IServiceBase<T> serviceBase)
        {
            _serviceBase = serviceBase;
        }

        public async Task<HttpResult<string>> CreateAsync<TRequest>(TRequest obj, CancellationToken cancellationToken) where TRequest : RequestBase
        {
            return await _serviceBase.CreateAsync(obj, cancellationToken);
        }

        public async Task<HttpResult<long>> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            return await _serviceBase.DeleteAsync(id, cancellationToken);
        }

        public async Task<HttpResult<ICollection<TResponse>>> GetAsync<TResponse>(Expression<Func<T, bool>> predicate) where TResponse : ResponseBase
        {
            return await _serviceBase.GetAsync<TResponse>(predicate);
        }

        public async Task<HttpResult<TResponse>> GetByIdAsync<TResponse>(string id) where TResponse : ResponseBase
        {
            return await _serviceBase.GetByIdAsync<TResponse>(id);
        }

        public async Task<HttpResult<string>> UpdateAsync<TRequest>(Expression<Func<T, bool>> filterDefinition, TRequest obj, ReplaceOptions options) where TRequest : RequestBase
        {
            return await _serviceBase.UpdateAsync(filterDefinition, obj, options);
        }

    }
}
