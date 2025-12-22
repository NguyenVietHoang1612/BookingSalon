public class ServiceResult<T>
{
    public bool Succeeded { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ServiceResult<T> Success(T data) => new()
    {
        Succeeded = true,
        Data = data
    };

    public static ServiceResult<T> Failed(params string[] errors) => new()
    {
        Succeeded = false,
        Errors = errors.ToList()
    };
}