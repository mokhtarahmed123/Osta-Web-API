using FluentValidation;
using Osta.Core.Feature.MediaBooking.Command.Model;

namespace Osta.Core.Feature.MediaBooking.Command.Validation
{
    public class AddMediaBookingCommandValidation
        : AbstractValidator<AddMediaBookingCommand>
    {

        private const long MaxFileSize = 30 * 1024 * 1024; // 30 MB

        public AddMediaBookingCommandValidation()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage("Booking Id must be greater than 0.");

            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("File is required.")
                .Must(file => file is not null && file.Length > 0)
                .WithMessage("File cannot be empty.")
                .Must(file =>
                    file is not null &&
                    file.Length <= MaxFileSize)
                .WithMessage("File size must not exceed 30 MB.");
            RuleFor(x => x.FileType)
                .IsInEnum()
                .WithMessage("Invalid file type.");

            RuleFor(x => x.RepairMediaType)
                .IsInEnum()
                .WithMessage("Invalid media type.");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage(
                    "Description cannot exceed 1000 characters.");
        }
    }
}