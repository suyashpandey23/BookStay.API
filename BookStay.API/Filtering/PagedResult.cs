namespace BookStay.API.Filtering;

/// <summary>
/// A generic wrapper for paginated query results.
/// 
/// General: This class wraps a collection of items along with pagination metadata
/// like total count, page number, and page size. It's used to return paginated
/// data from API endpoints.
/// 
/// Technical: This is a generic class that can wrap any type T. It provides
/// clients with both the data and the information needed to implement pagination
/// controls (total items, current page, items per page, total pages).
/// 
/// Why it's needed:
/// - Enables efficient data retrieval by returning only a page of results
/// - Reduces network bandwidth and improves performance
/// - Provides clients with metadata to build pagination UI
/// - Standardizes pagination responses across the API
/// </summary>
/// <typeparam name="T">The type of items in the result</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// The collection of items for the current page.
    /// 
    /// General: The actual data being returned for this page.
    /// 
    /// Technical: Generic list that can hold any type of entity or DTO.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = [];

    /// <summary>
    /// The total number of items across all pages.
    /// 
    /// General: The total count of items that match the query, not just this page.
    /// 
    /// Technical: Used to calculate total pages and display "showing X of Y items".
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The current page number (1-based).
    /// 
    /// General: Which page of results this is.
    /// 
    /// Technical: 1-based indexing (page 1, not page 0).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The number of items per page.
    /// 
    /// General: How many items are returned on each page.
    /// 
    /// Technical: Also known as page size or limit. Used to calculate offsets.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of pages available.
    /// 
    /// General: How many pages of results exist in total.
    /// 
    /// Technical: Calculated as Math.Ceiling(TotalCount / (double)PageSize).
    /// Used to display page navigation controls.
    /// 
    /// Line-by-line:
    /// - (double)PageSize: Casts PageSize to double to enable decimal division
    /// - TotalCount / (double)PageSize: Divides total items by items per page
    /// - Math.Ceiling(): Rounds up to the nearest whole number (e.g., 2.3 becomes 3)
    /// - (int): Casts the result back to int since pages must be whole numbers
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
