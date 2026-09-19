namespace Hyper.SDK.Errors;

public class BasalamError : Exception
{
    public string ErrorMessage { get; }
    public string? Code { get; }

    public BasalamError(string message, string? code = null)
        : base(message)
    {
        ErrorMessage = message;
        Code = code;
    }

    protected BasalamError(string message, string? code, Exception? innerException)
        : base(message, innerException)
    {
        ErrorMessage = message;
        Code = code;
    }

    public override string ToString() => Code != null ? $"{Code}: {ErrorMessage}" : ErrorMessage;
}

public class BasalamAPIError : BasalamError
{
    public int StatusCode { get; }
    public string? ResponseBody { get; }

    public BasalamAPIError(string message, int statusCode, string? responseBody = null, string? code = null)
        : base(message, code)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}

public class BasalamAuthError : BasalamError
{
    public BasalamAuthError(string message, string? responseBody = null)
        : base(message, "auth_error")
    {
        ResponseBody = responseBody;
    }

    public string? ResponseBody { get; }
}

public class BasalamValidationError : BasalamError
{
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Errors { get; }

    public BasalamValidationError(Dictionary<string, IReadOnlyList<string>> errors)
        : base("Validation failed", "validation_error")
    {
        Errors = errors;
    }
}
