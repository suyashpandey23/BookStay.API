namespace BookStay.API.Services;

public interface IApiKeyValidatorService {
    Task<bool> IsValidAsync(string apiKey, CancellationToken cancellationToken);
}