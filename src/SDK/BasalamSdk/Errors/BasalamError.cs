namespace Basalam.SDK.Errors;

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

public class BasalamAPIError(string message, int statusCode, string? responseBody = null, string? code = null) : BasalamError(message, code)
{
    public int StatusCode { get; } = statusCode;
    public string? ResponseBody { get; } = responseBody;
}

public class BasalamAuthError(string message, string? responseBody = null) : BasalamError(message, "auth_error")
{
    public string? ResponseBody { get; } = responseBody;
}

public class BasalamValidationError(Dictionary<string, IReadOnlyList<string>> errors) : BasalamError("Validation failed", "validation_error")
{
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Errors { get; } = errors;
}
