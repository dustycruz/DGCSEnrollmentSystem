// File: EnrollmentSystem.BLL/Common/ServiceResult.cs
namespace EnrollmentSystem.BLL.Common;

public class ServiceResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public static ServiceResult Ok(string? message = null) => new() { Success = true, Message = message };
    public static ServiceResult Fail(string message) => new() { Success = false, Message = message };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static new ServiceResult<T> Fail(string message)
        => new() { Success = false, Message = message };
}