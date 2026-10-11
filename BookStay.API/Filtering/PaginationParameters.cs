namespace BookStay.API.Filtering;

/// <summary>
/// Parameters for controlling pagination in API queries.
/// 
/// General: This class allows clients to specify which page of results they want
/// and how many items per page. It's used to implement pagination in API endpoints.
/// 
/// Technical: These parameters are typically passed as query string parameters
/// (e.g., ?pageNumber=1&pageSize=10). They are used to calculate the
/// SQL OFFSET and LIMIT clauses for efficient database queries.
/// 
/// Why it's needed:
/// - Enables efficient data retrieval by fetching only needed records
/// - Reduces database load and network bandwidth
/// - Improves response times for large datasets
/// - Provides a consistent pagination interface across the API
/// </summary>
public class PaginationParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// The page number to retrieve (1-based).
    /// 
    /// General: Which page of results the client wants.
    /// 
    /// Technical: Defaults to 1. Used with PageSize to calculate OFFSET:
    /// OFFSET = (PageNumber - 1) * PageSize.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// The number of items per page.
    /// 
    /// General: How many items should be returned on each page.
    /// 
    /// Technical: Defaults to 10. Capped at MaxPageSize (100) to prevent
    /// clients from requesting too much data at once. Used to calculate
    /// the SQL LIMIT clause.
    /// 
    /// Line-by-line:
    /// - get => _pageSize: Returns the backing field value
    /// - set => _pageSize = value > MaxPageSize ? MaxPageSize : value: 
    ///   Ternary operator that caps the value at MaxPageSize. If the
    ///   requested value exceeds 100, it uses 100; otherwise, it uses
    ///   the requested value.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
