using DigitalBank.Domain.Enums;
using System.Net;
using System.Threading.Tasks;

namespace DigitalBank.Domain.Entities
{
    public class Conta : Entity
    {
        public Conta()
        {

        }

        public Conta(decimal saldo)
        {
            Saldo = saldo;
        }

        public decimal Saldo { get; private set; }
        public string Numero { get; set; }

        public override bool IsValid(EValidationStage eValidationStage)
        {
            bool result = true;

            if (string.IsNullOrWhiteSpace(Numero))
            {
                result = false;
                ValidationErrors.Add(new Error("Numero", "Numero não pode ser nulo."));
            }

            if (eValidationStage == EValidationStage.Update || eValidationStage == EValidationStage.Delete)
            {
                if (string.IsNullOrWhiteSpace(Id))
                {
                    result = false;
                    ValidationErrors.Add(new Error("Id", "Id não pode ser nulo."));
                }
            }

            return result;

        }

        public Task<HttpResult<decimal>> Depositar(decimal valor)
        {
            Saldo += valor;
            return Task.FromResult(new HttpResult<decimal>(Saldo, HttpStatusCode.OK, null));
        }

        public Task<HttpResult<decimal>> Sacar(decimal valor)
        {
            if (Saldo - valor < 0)
                return Task.FromResult(new HttpResult<decimal>(0, HttpStatusCode.BadRequest, Error.GenerateFailure("Saldo insulficiente.")));

            Saldo -= valor;
            return Task.FromResult(new HttpResult<decimal>(Saldo, HttpStatusCode.OK, null));
        }
    }
}
