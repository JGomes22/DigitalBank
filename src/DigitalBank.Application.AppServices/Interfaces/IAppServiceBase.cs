using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.DTO.Response;
using DigitalBank.Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalBank.Application.AppServices.Interfaces
{
    public interface IAppServiceBase<T> where T : Entity
    {
        Task<HttpResult<long>> DeleteAsync(string id, CancellationToken cancellationToken);
        Task<HttpResult<TResponse>> GetByIdAsync<TResponse>(string id) where TResponse : ResponseBase;
        Task<HttpResult<ICollection<TResponse>>> GetAsync<TResponse>(Expression<Func<T, bool>> predicate) where TResponse : ResponseBase;
        Task<HttpResult<string>> CreateAsync<TRequest>(TRequest obj, CancellationToken cancellationToken) where TRequest : RequestBase;
        Task<HttpResult<string>> UpdateAsync<TRequest>(Expression<Func<T, bool>> filterDefinition, TRequest obj, ReplaceOptions options) where TRequest : RequestBase;
    }
}
