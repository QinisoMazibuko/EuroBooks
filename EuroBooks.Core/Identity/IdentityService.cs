using EuroBooks.Application.Common.Interfaces;
using EuroBooks.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EuroBooks.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IDateTime dateTime;

        public IdentityService(UserManager<ApplicationUser> userManager
            , RoleManager<ApplicationRole> roleManager
            , IDateTime dateTime)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dateTime = dateTime;
        }

        public async Task<Result> AddUserToRoleAsync(long userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            // Remove all roles and add new roles
            var userRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, userRoles);
            var result = await userManager.AddToRoleAsync(user, role);

            return result.ToApplicationResult();
        }

        public async Task<Result> AddUserToRolesAsync(long userId, IList<string> roles)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            // Remove all roles and add new roles
            var userRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, userRoles);
            var result = await userManager.AddToRolesAsync(user, roles);

            return result.ToApplicationResult();
        }

        public async Task<(Result Result, long UserId)> CreateUserAsync(string username, string email, string firstName, string lastName, bool isActive, bool? isDeleted)
        {
            //Create new user
            var user = new ApplicationUser
            {
                NormalizedUserName = username.ToUpper(),
                UserName = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                IsActive = isActive,
                IsDeleted = isDeleted,
             
            };

            var result = await userManager.CreateAsync(user);

            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<Result> DeleteUserAsync(long userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);

                return result.ToApplicationResult();
            }

            return Result.Failure();
        }

        public async Task<(long userId, string username, string email, string firstName, string lastName, string passwordHash, IList<string> roles, bool isActive, bool? isDeleted)> FindUserAsync(long userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            return ReturnUserInfo(user);
        }

        public async Task<(long userId, string username, string email, string firstName, string lastName, string passwordHash, IList<string> roles, bool isActive, bool? isDeleted)> FindUserAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            return ReturnUserInfo(user);
        }

        public async Task<(long userId, string username, string email, string firstName, string lastName, string passwordHash, IList<string> roles, bool isActive, bool? isDeleted)> FindUserByUsernameAsync(string username)
        {
            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.UserName.Equals(username) && u.IsActive);
            return ReturnUserInfo(user);
        }

        public async Task<(long userId, string username, string email, string firstName, string lastName, string passwordHash, IList<string> roles, bool isActive, bool? isDeleted)> FindUserByRemoteIdAsync(long remoteUserId)
        {
            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.IsActive);

            return ReturnUserInfo(user);
        }

        public List<string> GetRoles()
        {
            var roles = roleManager.Roles.Select(r => r.Name).ToList();
            return roles;
        }

        public async Task<string> HashPasswordAsync(long userId, string password)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            var hashPwd = userManager.PasswordHasher.HashPassword(user, password);

            return hashPwd;
        }

        public async Task<Result> UpdatePasswordAsync(long userId, string passwordHash)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.PasswordHash = passwordHash;
                user.LastModified = dateTime.Now;

                var result = await userManager.UpdateAsync(user);

                return result.ToApplicationResult();
            }

            throw new NullReferenceException("Unable to update password");
        }

        public async Task<Result> UpdateUserAsync(long userId, string email, string firstName, string lastName, bool isActive, bool? isDeleted)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.Email = email;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.IsActive = isActive;
                user.IsDeleted = isDeleted;
                user.LastModified = dateTime.Now;
             
                var result = await userManager.UpdateAsync(user);

                return result.ToApplicationResult();
            }

            throw new NullReferenceException("Unable to update user");
        }

        (long userId, string username, string email, string firstName, string lastName, string passwordHash, IList<string> roles, bool isActive, bool? isDeleted) ReturnUserInfo(ApplicationUser user)
        {
            if (user == null)
                return (0, "", "", "", "", "", null, false, null);

            return (user.Id
                , user.UserName
                , user.Email
                , user.FirstName
                , user.LastName
                , user.PasswordHash
                , userManager.GetRolesAsync(user).Result
                , user.IsActive
                , user.IsDeleted);
        }
    }
}
