namespace MTGApplication.General.Services.ConfirmationService;

public enum ConfirmationResult { Yes, No, Failure, Cancel }

public record Confirmation<TArgs>(string Title, string Message, TArgs Data);
public record Confirmation(string Title, string Message);
