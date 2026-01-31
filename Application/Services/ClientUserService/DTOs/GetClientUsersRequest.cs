using Application.Generic_DTOs;

namespace Application.Services.ClientUserService.DTOs
{
    public class GetClientUsersRequest : PaginationRequest
    {
        public string? SearchTerm { get; set; }
    }
}
