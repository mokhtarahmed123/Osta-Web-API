namespace Osta.Notification.DTOs
{
    public record Emaildto(
        string Email,
        string Massage,
        string? reason
    );


}
