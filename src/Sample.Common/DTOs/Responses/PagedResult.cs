namespace Sample.Common.DTOs.Responses
{
    public record PagedResult<T>(IEnumerable<T> Data, int RealRows, int LimitRows);
}