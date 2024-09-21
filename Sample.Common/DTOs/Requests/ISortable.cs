namespace Sample.Common.DTOs.Requests
{
    public interface ISortable
    {
        SortingType SortingType { get; set; }
        string SortingProperty { get; set; }
    }

    public enum SortingType
    {
        Ascending,
        Descending
    }
}