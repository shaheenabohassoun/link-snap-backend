using AutoMapper;
using FluentValidation;
using LinkSnap.Application.DTOs;
using LinkSnap.Application.Interfaces;
using LinkSnap.Domain.Entities;

namespace LinkSnap.Application.Services
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IClickRepository _clickRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ShortenRequest> _validator;

        public ShortUrlService(
            ILinkRepository linkRepository,
            IClickRepository clickRepository,
            IMapper mapper,
            IValidator<ShortenRequest> validator)
        {
            _linkRepository = linkRepository;
            _clickRepository = clickRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<LinkDto> CreateShortUrlAsync(ShortenRequest request, string? userId = null)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var shortCode = request.CustomAlias ?? GenerateShortCode();

            if (await _linkRepository.ExistsByShortCodeAsync(shortCode))
                throw new InvalidOperationException("Short code already exists");

            var link = new Link
            {
                OriginalUrl = request.OriginalUrl,
                ShortCode = shortCode,
                UserId = userId,
                ExpiresAt = request.ExpiresAt
            };

            await _linkRepository.AddAsync(link);

            var dto = _mapper.Map<LinkDto>(link);
            dto.CustomAlias = request.CustomAlias;
            return dto;
        }

        public async Task<LinkDto?> GetLinkByShortCodeAsync(string shortCode)
        {
            var link = await _linkRepository.GetByShortCodeAsync(shortCode);
            return link == null ? null : _mapper.Map<LinkDto>(link);
        }

        public async Task<IEnumerable<LinkDto>> GetUserLinksAsync(string userId)
        {
            var links = await _linkRepository.GetUserLinksAsync(userId);
            return _mapper.Map<List<LinkDto>>(links);
        }

        public async Task<string> ResolveShortCodeAsync(
            string shortCode,
            string? ipAddress = null,
            string? userAgent = null,
            string? referrer = null)
        {
            var link = await _linkRepository.GetByShortCodeAsync(shortCode);
            if (link == null || !link.IsActive || (link.ExpiresAt.HasValue && link.ExpiresAt < DateTime.UtcNow))
                throw new KeyNotFoundException("Link not found or expired");

            link.ClickCount++;
            await _linkRepository.UpdateAsync(link);

            await _clickRepository.AddAsync(new Click
            {
                LinkId = link.Id,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                Referrer = string.IsNullOrWhiteSpace(referrer) ? null : referrer,
                ClickedAt = DateTime.UtcNow
            });

            return link.OriginalUrl;
        }

        public async Task<LinkDto?> GetLinkByIdAsync(Guid id)
        {
            var link = await _linkRepository.GetByIdAsync(id);
            return link == null ? null : _mapper.Map<LinkDto>(link);
        }

        public async Task<LinkDto> UpdateLinkAsync(Guid id, UpdateLinkDto update)
        {
            var link = await _linkRepository.GetByIdAsync(id);
            if (link == null)
                throw new KeyNotFoundException("Link not found");

            link.ExpiresAt = update.ExpiresAt;
            if (update.IsActive.HasValue)
                link.IsActive = update.IsActive.Value;

            link.UpdatedAt = DateTime.UtcNow;
            await _linkRepository.UpdateAsync(link);
            return _mapper.Map<LinkDto>(link);
        }

        public async Task DeleteLinkAsync(Guid id)
        {
            var link = await _linkRepository.GetByIdAsync(id);
            if (link == null)
                throw new KeyNotFoundException("Link not found");
            await _linkRepository.DeleteAsync(link);
        }

        private string GenerateShortCode() =>
            Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-")
                .Substring(0, 7);
    }
}
