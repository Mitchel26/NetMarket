namespace WebApi.Errors
{
    public class CodeErrorResponse
    {
        public CodeErrorResponse(int statusCode, string mensaje = null)
        {
            StatusCode = statusCode;
            Mensaje = mensaje ?? GetDefaultMessageStatusCode(statusCode);
        }

        private string GetDefaultMessageStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "EL request tiene errores",
                401 => "No tine autorización para este recurso",
                404 => "El recurso no se encuentra disponible",
                500 => "Se producierón errores en el servidor",
                _ => null
            };
        }

        public int StatusCode { get; }
        public string Mensaje { get; }
    }
}
