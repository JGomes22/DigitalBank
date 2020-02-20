using AutoMapper;
using DigitalBank.Domain.AutoMapper;
using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.DTO.Response;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Enums;
using DigitalBank.Domain.Interfaces.Repositories;
using DigitalBank.Domain.Interfaces.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalBank.Domain.Services
{
    public class ServiceBase<T> : IServiceBase<T> where T : Entity
    {
        private readonly IBaseMongoRepository<T> _baseMongoRepository;
        protected readonly IMapper _mapper;

        public ServiceBase(IBaseMongoRepository<T> baseMongoRepository)
        {
            var config = AutoMapperConfig.RegisterMappings();
            _mapper = new Mapper(config);
            _baseMongoRepository = baseMongoRepository;
        }

        public async Task<HttpResult<string>> CreateAsync<TRequest>(TRequest obj, CancellationToken cancellationToken) where TRequest : RequestBase
        {
            try
            {
                if (obj == null)
                    return
                        new HttpResult<string>(null, HttpStatusCode.BadRequest, Error.GenerateFailure("Objeto não pode ser nulo."));
                T objModel = _mapper.Map<TRequest, T>(obj);

                objModel.Created = DateTime.UtcNow;
                if (!objModel.IsValid(EValidationStage.Create))
                    return
                        new HttpResult<string>(null, HttpStatusCode.BadRequest, objModel.ValidationErrors);
                if (string.IsNullOrWhiteSpace(objModel.Id))
                    objModel.Id = Guid.NewGuid().ToString();

                objModel.Created = DateTime.UtcNow;
                string insert = await _baseMongoRepository.InsertAsync(objModel, cancellationToken);
                if (string.IsNullOrWhiteSpace(insert))
                    return
                        new HttpResult<string>(null, HttpStatusCode.BadRequest, Error.GenerateFailure($"Não foi possível criar {typeof(T).Name}."));
                return
                    new HttpResult<string>(insert, HttpStatusCode.Created, null);
            }
            catch (Exception ex)
            {
                return new HttpResult<string>(null, HttpStatusCode.InternalServerError, Error.GenerateFailure(ex.Message));
            }
        }

        public async Task<HttpResult<long>> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new HttpResult<long>(0, HttpStatusCode.BadRequest, Error.GenerateFailure("Id não pode ser nulo."));

                var delete = await _baseMongoRepository.DeleteAsync(id, cancellationToken);
                if (delete <= 0)
                    return new HttpResult<long>(delete, HttpStatusCode.NotFound, Error.GenerateFailure($"{typeof(T).Name} não encontrado."));

                return new HttpResult<long>(delete, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return new HttpResult<long>(0, HttpStatusCode.InternalServerError, Error.GenerateFailure(ex.Message));
            }
        }

        public async Task<HttpResult<ICollection<TResponse>>> GetAsync<TResponse>(Expression<Func<T, bool>> predicate) where TResponse : ResponseBase
        {
            try
            {
                var obj = await _baseMongoRepository.GetAsync(predicate);
                if (obj == null)
                    return new HttpResult<ICollection<TResponse>>(default, HttpStatusCode.NotFound, Error.GenerateFailure($"{typeof(T).Name} não encontrado."));

                ICollection<TResponse> objRequest = _mapper.Map<ICollection<T>, ICollection<TResponse>>(obj);
                var objResult = new HttpResult<ICollection<TResponse>>(objRequest, HttpStatusCode.OK, null);
                return objResult;
            }
            catch (Exception ex)
            {
                return new HttpResult<ICollection<TResponse>>(default, HttpStatusCode.InternalServerError, Error.GenerateFailure(ex.Message));
            }
        }

        public async Task<HttpResult<TResponse>> GetByIdAsync<TResponse>(string id) where TResponse : ResponseBase
        {
            try
            {
                var obj = await _baseMongoRepository.GetByIdAsync(id);
                if (obj == null)
                    return new HttpResult<TResponse>(default(TResponse), HttpStatusCode.NotFound, Error.GenerateFailure($"{typeof(T).Name} não encontrado."));
                TResponse objRequest = this._mapper.Map<T, TResponse>(obj);
                var objResult = new HttpResult<TResponse>(objRequest, HttpStatusCode.OK, null);
                return objResult;
            }
            catch (Exception ex)
            {
                return new HttpResult<TResponse>(default, HttpStatusCode.InternalServerError, Error.GenerateFailure(ex.Message));
            }
        }

        public async Task<HttpResult<string>> UpdateAsync<TRequest>(Expression<Func<T, bool>> filterDefinition, TRequest obj, ReplaceOptions options) where TRequest : RequestBase
        {
            try
            {
                if (filterDefinition == null)
                    return
                        new HttpResult<string>(null, HttpStatusCode.BadRequest, Error.GenerateFailure("Filtro não pode ser nulo."));
                if (obj == null)
                    return
                        new HttpResult<string>(null, HttpStatusCode.BadRequest, Error.GenerateFailure("Objeto é nulo."));

                T objModel = this._mapper.Map<TRequest, T>(obj);

                if (!objModel.IsValid(EValidationStage.Update))
                    return
                        new HttpResult<string>(string.Empty, HttpStatusCode.BadRequest, objModel.ValidationErrors);

                string replace = await _baseMongoRepository.UpdateAsync(filterDefinition, objModel, options);

                if (string.IsNullOrWhiteSpace(replace))
                    return
                        new HttpResult<string>(string.Empty, HttpStatusCode.BadRequest, Error.GenerateFailure($"{typeof(T).Name} não encontrado."));

                return
                        new HttpResult<string>(replace, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return new HttpResult<string>(null, HttpStatusCode.InternalServerError, Error.GenerateFailure(ex.Message));
            }
        }

    }
}
