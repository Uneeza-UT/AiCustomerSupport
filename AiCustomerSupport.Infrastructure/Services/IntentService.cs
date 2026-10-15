using AiCustomerSupport.Application.Contracts.Persistence;
using AiCustomerSupport.Application.Contracts.Services;
using AiCustomerSupport.Application.DTOs.Common;
using AiCustomerSupport.Application.DTOs.Intent;
using AiCustomerSupport.Application.Enums;
using AiCustomerSupport.Application.Exceptions;
using AiCustomerSupport.Domain;
using AutoMapper;
using System.Linq.Expressions;

namespace AiCustomerSupport.Infrastructure.Services
{
    public class IntentService : IIntentService
    {
        private readonly IMapper _mapper;
        private readonly IIntentRepository _intentRepository;

        public IntentService(IMapper mapper, IIntentRepository intentRepository)
        {
            _mapper = mapper;
            _intentRepository = intentRepository;
        }

        public async Task<List<GetIntentDto>> GetAsync(PaginationDto dto)
        {
            var intents = await _intentRepository.GetPagedAsync(dto.PageNumber, dto.PageSize);
            return _mapper.Map<List<GetIntentDto>>(intents);
        }



        public async Task<GetIntentDto> GetByIdAsync(int id)
        {
            var intent = await _intentRepository.GetByIdAsync(id);

            if (intent == null)
            {
                throw new NotFoundException(nameof(intent), id);
            }

            return _mapper.Map<GetIntentDto>(intent);
        }



        public async Task<List<GetIntentDto>> SortAsync(SortDto dto)
        {
            bool descending;
            Expression<Func<Intent, object>> sortExpression;

            switch (dto.SortBy)
            {
                case SortOption.Ascending:
                    descending = false;
                    sortExpression = i => i.Name;
                    break;

                case SortOption.Descending:
                    descending = true;
                    sortExpression = i => i.Name;
                    break;

                case SortOption.Latest:
                    descending = true;
                    sortExpression = i => i.CreatedAt;
                    break;

                case SortOption.Oldest:
                    descending = false;
                    sortExpression = i => i.CreatedAt;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(dto.SortBy),
                        dto.SortBy,
                        "Invalid sort option.");
            }

            var paginationDto = new PaginationDto();
            var intents = await _intentRepository
                .SortAsync(
                    descending,
                    sortExpression,
                    paginationDto.PageNumber,
                    paginationDto.PageSize);

            return _mapper.Map<List<GetIntentDto>>(intents);
        }



        public async Task<List<GetIntentDto>> SearchAsync(SearchDto dto)
        {
            string searchTerm = dto.SearchTerm?.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAsync(new PaginationDto());
            }

            var intents = await _intentRepository.SearchAsync(searchTerm);
            return _mapper.Map<List<GetIntentDto>>(intents);
        }

        
    }
}
