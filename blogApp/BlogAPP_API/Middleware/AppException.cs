//public abstract class AppException : Exception
//{
//    public int StatusCode { get; }
//    public string Code { get; }

//    protected AppException(int statusCode, string code, string message) : base(message)
//    {
//        StatusCode = statusCode;
//        Code = code;
//    }
//}

//public sealed class NotFoundAppException : AppException
//{
//    public NotFoundAppException(string message = "Не найдено")
//        : base(StatusCodes.Status404NotFound, "not_found", message) { }
//}

//public sealed class BadRequestAppException : AppException
//{
//    public BadRequestAppException(string message = "Некорректный запрос")
//        : base(StatusCodes.Status400BadRequest, "bad_request", message) { }
//}

//public sealed class UnauthorizedAppException : AppException
//{
//    public UnauthorizedAppException(string message = "Не авторизован")
//        : base(StatusCodes.Status401Unauthorized, "unauthorized", message) { }
//}

//public sealed class ConflictAppException : AppException
//{
//    public ConflictAppException(string message = "Конфликт")
//        : base(StatusCodes.Status409Conflict, "conflict", message) { }
//}