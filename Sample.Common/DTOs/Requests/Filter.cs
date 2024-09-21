namespace Sample.Common.DTOs.Requests
{
    public abstract class Filter : ISortable
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = int.MaxValue;
        public virtual SortingType SortingType { get; set; } = SortingType.Ascending;
        public virtual string SortingProperty { get; set; } = string.Empty;
    }
}