using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.Interfaces
{
    public interface IUserProfileRepository
    {
        // Add a new user profile
        Task AddAsync(UserProfileDto userProfile); // Changed to asynchronous method

        // Get user profile by ID
        Task<UserProfileDto> GetAsync(Guid id);

        // Get all user profiles
        Task<IEnumerable<UserProfileDto>> GetAllAsync();

        // Remove user profile by ID
        void Remove(Guid id);

        // Edit (update) user profile
        void Edit(UserProfileDto userProfile);

        // Get user profile based on a condition
        Task<UserProfileDto> GetByConditionAsync(Expression<Func<UserProfileDto, bool>> predicate);

        // Get user profile by User ID
        Task<UserProfileDto> GetByUserIdAsync(Guid userId); // New method to fetch by user ID

        // Create user profile asynchronously
        Task CreateAsync(UserProfileDto userProfileDto); // New method for creating a profile asynchronously

        // Update user profile asynchronously
        Task UpdateAsync(UserProfileDto userProfileDto); // New method for updating a profile asynchronously

        // Delete user profile asynchronously
        Task DeleteAsync(UserProfileDto userProfile); // New method for deleting a profile asynchronously

        // Save changes
        Task SaveAsync();

        // Check if a profile picture exists for a given user
        Task<bool> IsProfilePictureExistsAsync(Guid userId);
        Task<int> GetUserCountAsync();
    }
}
