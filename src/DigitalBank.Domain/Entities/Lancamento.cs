using DigitalBank.Domain.Enums;
using System;

namespace DigitalBank.Domain.Entities
{
    public class Lancamento : Entity
    {
        public EOperacao Operacao { get; set; }
        public decimal Valor { get; set; }
        public string IdContaOrigem { get; set; }
        public string IdContaDestino { get; set; }

        private static bool IsNullOrWhiteSpace(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public override bool IsValid(EValidationStage eValidationStage)
        {
            bool result = true;

            if (!Enum.IsDefined(typeof(EOperacao), Operacao))
            {
                result = false;
                ValidationErrors.Add(new Error("Operacao", "Operacao inválida."));
            }

            if (Valor <= 0)
            {
                result = false;
                ValidationErrors.Add(new Error("Valor", "Valor precisa ser maior que R$ 0,00."));
            }

            if (Operacao == EOperacao.Saque)
            {
                if (IsNullOrWhiteSpace(IdContaOrigem))
                {
                    result = false;
                    ValidationErrors.Add(new Error("IdContaOrigem", "IdContaOrigem não pode ser nulo."));
                }
            }

            if (Operacao == EOperacao.Deposito)
            {
                if (IsNullOrWhiteSpace(IdContaDestino))
                {
                    result = false;
                    ValidationErrors.Add(new Error("IdContaDestino", "IdContaDestino não pode ser nulo."));
                }
            }

            if (Operacao == EOperacao.Transferencia)
            {
                if (IsNullOrWhiteSpace(IdContaOrigem))
                {
                    result = false;
                    ValidationErrors.Add(new Error("IdContaOrigem", "IdContaOrigem não pode ser nulo."));
                }
                if (IsNullOrWhiteSpace(IdContaDestino))
                {
                    result = false;
                    ValidationErrors.Add(new Error("IdContaDestino", "IdContaDestino não pode ser nulo."));
                }
                if (!IsNullOrWhiteSpace(IdContaOrigem) && !IsNullOrWhiteSpace(IdContaDestino))
                {
                    if (IdContaOrigem == IdContaDestino)
                    {
                        result = false;
                        ValidationErrors.Add(new Error("IdContaOrigem", "IdContaOrigem não pode ser o mesmo que IdContaDestino."));
                    }
                }
            }

            return result;
        }
    }
}
