using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain.Interfaces;

namespace DevSkill.Inventory.Application.Services
{
    public class UserProfileManagementService : IUserProfileManagementService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;

        public UserProfileManagementService(IUserProfileRepository userProfileRepository, IMapper mapper)
        {
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
        {
            var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
            return _mapper.Map<UserProfileDto>(userProfile);
        }

        public async Task CreateUserProfileAsync(UserProfileDto userProfileDto)
        {
            await _userProfileRepository.CreateAsync(userProfileDto);
        }

        public async Task UpdateUserProfileAsync(UserProfileDto userProfileDto)
        {
            var userProfile = await _userProfileRepository.GetByUserIdAsync(userProfileDto.ApplicationUserId);
            if (userProfile != null)
            {
                await _userProfileRepository.UpdateAsync(userProfileDto);
            }
            else
            {
                throw new ArgumentException("User profile not found.");
            }
        }

        public async Task DeleteUserProfileAsync(Guid userId)
        {
            var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (userProfile != null)
            {
                await _userProfileRepository.DeleteAsync(userProfile);
            }
            else
            {
                throw new ArgumentException("User profile not found.");
            }
        }

        public async Task<IList<UserProfileDto>> GetAllUserProfilesAsync()
        {
            var userProfiles = await _userProfileRepository.GetAllAsync();
            return _mapper.Map<IList<UserProfileDto>>(userProfiles);
        }

        public async Task<bool> ProfileExists(Guid userId)
        {
            var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
            return userProfile != null;
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _userProfileRepository.GetUserCountAsync();
        }

        bool IUserProfileManagementService.ProfileExists(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
