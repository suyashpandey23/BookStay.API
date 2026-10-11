namespace BookStay.API.Filtering;

/// <summary>
/// Base class for filter parameters used in API queries.
/// 
/// General: This class provides common filtering and sorting parameters that
/// can be reused across different endpoints. It includes pagination support
/// and sorting options.
/// 
/// Technical: This base class allows consistent filtering behavior across
/// the application. Derived classes can add specific filters for their domain.
/// The properties are used to build dynamic LINQ queries for filtering and sorting.
/// </summary>
public class BaseFilterParameters
{
    /// <summary>
    /// The field to sort results by.
    /// 
    /// General: Specifies which property should be used for sorting the results.
    /// 
    /// Technical: This string is used in a switch expression to determine which
    /// LINQ OrderBy/OrderByDescending method to apply.
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Whether to sort in descending order.
    /// 
    /// General: If true, results are sorted from highest to lowest.
    /// If false, results are sorted from lowest to highest.
    /// 
    /// Technical: When true, OrderByDescending is used instead of OrderBy.
    /// </summary>
    public bool SortDescending { get; set; }
}
