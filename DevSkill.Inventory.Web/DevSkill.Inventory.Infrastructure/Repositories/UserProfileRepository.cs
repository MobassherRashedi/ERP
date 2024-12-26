using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.Interfaces;
using DevSkill.Inventory.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevSkill.Inventory.Infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserProfileRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(UserProfileDto userProfileDto)
        {
            var userProfile = _mapper.Map<UserProfile>(userProfileDto);
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(UserProfileDto userProfileDto)
        {
            var userProfile = _mapper.Map<UserProfile>(userProfileDto);
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserProfileDto userProfileDto)
        {
            var userProfile = _mapper.Map<UserProfile>(userProfileDto);
            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();
        }

        public void Edit(UserProfileDto userProfileDto)
        {
            var userProfile = _mapper.Map<UserProfile>(userProfileDto);
            _context.UserProfiles.Update(userProfile);
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllAsync()
        {
            var userProfiles = await _context.UserProfiles.ToListAsync();
            return _mapper.Map<IEnumerable<UserProfileDto>>(userProfiles);
        }

        public async Task<UserProfileDto> GetAsync(Guid id)
        {
            var userProfile = await _context.UserProfiles.FindAsync(id);
            return _mapper.Map<UserProfileDto>(userProfile);
        }

        public async Task<UserProfileDto> GetByConditionAsync(Expression<Func<UserProfile, bool>> predicate)
        {
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(predicate);
            return _mapper.Map<UserProfileDto>(userProfile);
        }

        public async Task<UserProfileDto> GetByUserIdAsync(Guid userId)
        {
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.ApplicationUserId == userId);
            return _mapper.Map<UserProfileDto>(userProfile);
        }

        public async Task<bool> IsProfilePictureExistsAsync(Guid userId)
        {
            return await _context.UserProfiles.AnyAsync(up => up.ApplicationUserId == userId && up.ProfilePicturePath != null);
        }

        public void Remove(Guid id)
        {
            var userProfile = new UserProfile { Id = id };
            _context.UserProfiles.Remove(userProfile);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserProfileDto userProfileDto)
        {
            var userProfile = _mapper.Map<UserProfile>(userProfileDto);
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync(); // return user count instead of UserProfiles
        }

        public Task<UserProfileDto> GetByConditionAsync(Expression<Func<UserProfileDto, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
