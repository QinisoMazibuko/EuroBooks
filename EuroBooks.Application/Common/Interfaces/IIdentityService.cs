using EuroBooks.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EuroBooks.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<(long userId, string username, string email, string firstName, string lastName, string passwordHash, IList<string> roles, bool isActive, bool? isDeleted)> FindUserAsync(long userId);
        Task<(long userId, string username, string email, string firstName, string lastName, string passwordHash, IList<string> roles, bool isActive,  bool? isDeleted)> FindUserAsync(string email);
        Task<(long userId, string username, string email, string firstName, string lastName, string passwordHash, IList<string> roles, bool isActive, bool? isDeleted)> FindUserByUsernameAsync(string username);
        Task<(long userId, string username, string email, string firstName, string lastName, string passwordHash, IList<string> roles, bool isActive,  bool? isDeleted)> FindUserByRemoteIdAsync(long remoteUserId);
        Task<(Result Result, long UserId)> CreateUserAsync(string username, string email, string firstName, string lastName, bool isActive, bool? isDeleted);
        Task<Result> UpdateUserAsync(long userId, string email, string firstName, string lastName, bool isActive, bool? isDeleted);
        Task<Result> DeleteUserAsync(long userId);
        Task<string> HashPasswordAsync(long userId, string password);
        Task<Result> UpdatePasswordAsync(long userId, string passwordHash);
        List<string> GetRoles();
        Task<Result> AddUserToRoleAsync(long userId, string role);
        Task<Result> AddUserToRolesAsync(long userId, IList<string> roles);
    }
}
