using System.ComponentModel.DataAnnotations;

namespace BookStay.API.DTOs.Booking;

/// <summary>
/// Data Transfer Object for updating an existing booking.
/// 
/// General: This DTO is used to receive booking update requests from API clients.
/// It allows users to modify the dates and guest count of an existing booking.
/// 
/// Technical: Implements IValidatableObject to add custom validation logic.
/// The record type provides immutability. Only modifiable fields are included
/// (StayId cannot be changed after booking creation).
/// </summary>
/// <param name="CheckIn">The updated check-in date</param>
/// <param name="CheckOut">The updated check-out date</param>
/// <param name="Guests">The updated number of guests (1-10)</param>
public record UpdateBookingDto(
    DateOnly CheckIn,
    DateOnly CheckOut,
    [Required][Range(minimum: 1, maximum: 10)] int Guests
) : IValidatableObject
{
    /// <summary>
    /// Custom validation logic for the booking update.
    /// 
    /// General: Ensures that the updated booking dates are logically valid.
    /// 
    /// Technical: IValidatableObject.Validate ensures check-out is after check-in.
    /// This validation runs after standard data annotation validation.
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
