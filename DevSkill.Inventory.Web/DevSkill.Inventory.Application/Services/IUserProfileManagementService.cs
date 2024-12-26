using DevSkill.Inventory.Domain.Dtos;

namespace DevSkill.Inventory.Application.Services
{
    public interface IUserProfileManagementService
    {
        Task<UserProfileDto> GetUserProfileAsync(Guid userId);
        Task CreateUserProfileAsync(UserProfileDto userProfileDto);
        Task UpdateUserProfileAsync(UserProfileDto userProfileDto);
        Task DeleteUserProfileAsync(Guid userId);
        bool ProfileExists(Guid userId);
        Task<int> GetUserCountAsync();

        Task<IList<UserProfileDto>> GetAllUserProfilesAsync();
    }
}
