using System;

namespace Meticulos.Api.App
{
    public class OperationResult
    {
        public Exception Exception { get; set; }
        public string ErrorMessage { get; set; }

        public string DisplayMessage
        {
            get
            {
                return Exception != null
                    ? Exception.Message
                    : ErrorMessage;
            }
        }

        public bool IsSuccess
        {
            get
            {
                return Exception == null
                    && string.IsNullOrEmpty(ErrorMessage);
            }
        }

        public bool IsFailure
        {
            get
            {
                return !IsSuccess;
            }
        }

        public OperationResult()
        {
        }

        public OperationResult(Exception ex)
        {
            Exception = ex;
        }
    }

    public class OperationResult<T> : OperationResult
    {
        public OperationResult()
            : base()
        {
        }

        public OperationResult(T value)
            : base()
        {
            Value = value;
        }

        public OperationResult(Exception ex)
            : base(ex)
        {
        }

        public T Value { get; set; }
    }
}
