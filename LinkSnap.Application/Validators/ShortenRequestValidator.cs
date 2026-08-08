using FluentValidation;
using LinkSnap.Application.DTOs;

namespace LinkSnap.Application.Validators
{
    public class ShortenRequestValidator : AbstractValidator<ShortenRequest>
    {
        public ShortenRequestValidator()
        {
            RuleFor(x => x.OriginalUrl)
                .NotEmpty().WithMessage("URL is required")
                .Must(BeAValidUrl).WithMessage("Invalid URL format");
        }

        private bool BeAValidUrl(string url) => Uri.IsWellFormedUriString(url, UriKind.Absolute);
    }
}