namespace Application.Models
{
    public class GenericResponse
    {
        public bool HasError => ErrorCode != null;
        public string? ErrorCode { get; protected set; }

        protected GenericResponse()
        {
        }

        public static GenericResponse Success()
        {
            return new GenericResponse();
        }

        public static GenericResponse Failure(string errorCode)
        {
            return new GenericResponse() { ErrorCode = errorCode };
        }
    }

    public class GenericResponse<T> : GenericResponse
    {
        public T Value { get; private set; }

        public static GenericResponse<T> Success(T value)
        {
            return new GenericResponse<T> { Value = value };
        }

        public new static GenericResponse<T> Failure(string errorCode)
        {
            return new GenericResponse<T> { ErrorCode = errorCode };
        }
    }
}
