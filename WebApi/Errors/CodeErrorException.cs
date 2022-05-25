namespace WebApi.Errors
{
    public class CodeErrorException : CodeErrorResponse
    {
        public CodeErrorException(int statusCode, string mensaje = null, string details = null) : base(statusCode, mensaje)
        {
            Details = details;
        }

        public string Details { get; set; }
    }
}
