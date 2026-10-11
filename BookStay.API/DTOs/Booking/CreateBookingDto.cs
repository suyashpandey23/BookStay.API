using System.ComponentModel.DataAnnotations;

namespace BookStay.API.DTOs.Booking;

/// <summary>
/// Data Transfer Object for creating a new booking.
/// 
/// General: This DTO is used to receive booking creation requests from API clients.
/// It validates the input data before creating a booking in the database.
/// 
/// Technical: Implements IValidatableObject to add custom validation logic beyond
/// standard data annotations. The record type provides immutability.
/// Validation ensures CheckOut is after CheckIn and guest count is within range.
/// </summary>
/// <param name="StayId">The ID of the stay/accommodation to book</param>
/// <param name="CheckIn">The desired check-in date</param>
/// <param name="CheckOut">The desired check-out date</param>
/// <param name="Guests">The number of guests (1-10)</param>
public record CreateBookingDto(
    [Required] int StayId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    [Required] [Range(minimum: 1, maximum: 10)] int Guests) : IValidatableObject
{
    /// <summary>
    /// Custom validation logic for the booking.
    /// 
    /// General: Ensures that the booking dates are logically valid.
    /// 
    /// Technical: IValidatableObject.Validate allows complex validation that
    /// involves multiple properties. Here we validate that check-out must be
    /// after check-in, which cannot be expressed with simple data annotations.
    /// </summary>
    /// <param name="validationContext">The validation context</param>
    /// <returns>Collection of validation results (empty if validation passes)</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOut <= CheckIn)
        {
            yield return new ValidationResult(
                "Check-out must be after check-in.",
                [nameof(CheckOut), nameof(CheckIn)]);
        }
    }
}
