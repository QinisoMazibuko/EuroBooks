using System;
using System.Linq;
using System.Threading.Tasks;
using EuroBooks.API.Models.Account;
using EuroBooks.Application.Common.Interfaces;
using EuroBooks.Application.Common.Models;
using EuroBooks.Application.User.Commands;
using EuroBooks.Application.User.Queries;
using EuroBooks.Domain.Enums;
using EuroBooks.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EuroBooks.API.Controllers
{
    /// <summary>
    /// Endpoints to Manage User Accounts
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    // TODO: See if this can be put into Application Layer
    public class AccountController : ApiController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDateTime dateTime;

        public AccountController(UserManager<ApplicationUser> userManager, IDateTime dateTime)
        {
            this.userManager = userManager;
            this.dateTime = dateTime;
        }


        /// <summary>
        /// Get list of all system users
        /// </summary>
        /// <param name="paging"></param>
        /// <returns>User list</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("user/list", Order = 1)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsers([FromBody] PagingInfo paging)
        {
            var users = userManager.Users;

            // Get users according to state
            if (paging.activeState != ActiveState.All)
            {
                bool state = paging.activeState == ActiveState.ActiveOnly ? true : false;
                users = users.Where(r => r.IsActive == state && (r.IsDeleted == null || r.IsDeleted == false));
            }
            else if (paging.activeState == ActiveState.DeletedOnly)
            {
                users = users.Where(r => r.IsDeleted == true);
            }

            // Filter according to search string
            if (!string.IsNullOrEmpty(paging.searchString))
                users = users.Where(r =>
                                        r.FirstName.Contains(paging.searchString, StringComparison.OrdinalIgnoreCase) ||
                                        r.LastName.Contains(paging.searchString, StringComparison.OrdinalIgnoreCase) ||
                                        r.Email.Contains(paging.searchString, StringComparison.OrdinalIgnoreCase) ||
                                        r.UserName.Contains(paging.searchString, StringComparison.OrdinalIgnoreCase)
                                    );

            #region SORTING
            if (!string.IsNullOrWhiteSpace(paging.sortBy))
            {
                switch (paging.sortBy)
                {
                    case "firstName":
                        {
                            if (paging.isSortAsc)
                                users = users.OrderBy(r => r.FirstName);
                            else
                                users = users.OrderByDescending(r => r.FirstName);
                        }
                        break;
                    case "lastName":
                        {
                            if (paging.isSortAsc)
                                users = users.OrderBy(r => r.LastName);
                            else
                                users = users.OrderByDescending(r => r.LastName);
                        }
                        break;
                    case "email":
                        {
                            if (paging.isSortAsc)
                                users = users.OrderBy(r => r.Email);
                            else
                                users = users.OrderByDescending(r => r.Email);
                        }
                        break;
                    case "userName":
                        {
                            if (paging.isSortAsc)
                                users = users.OrderBy(r => r.UserName);
                            else
                                users = users.OrderByDescending(r => r.UserName);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                users = users.OrderBy(r => r.Id);
            }
            #endregion SORTING

            var userList = await users.Skip(paging.skip).Take(paging.take)
                .Select(s => new UserModel
                {
                    Id = s.Id,
                    UserName = s.UserName,
                    Email = s.Email,
                    FirstName = s.FirstName,
                    IsActive = s.IsActive,
                    IsDeleted = s.IsDeleted,
                    LastName = s.LastName,
                    UserRoles = userManager.GetRolesAsync(s).Result
                }).ToListAsync();

            return Ok(userList);
        }

        /// <summary>
        /// Get list of  Admin and  Subscriber users 
        /// </summary>
        /// <param name="param"></param>
        /// <returns>User list</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("user/admin/list", Order = 1)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Get([FromForm] DataTableParams param)
        {
            var users = userManager.Users;
            PagingInfo paging = new PagingInfo(param);

            // Get users according to state
            if (paging.activeState != ActiveState.All)
            {
                bool state = paging.activeState == ActiveState.ActiveOnly ? true : false;
                users = users.Where(r => r.IsActive == state && (r.IsDeleted == null || r.IsDeleted == false));
            }
            else if (paging.activeState == ActiveState.DeletedOnly)
            {
                users = users.Where(r => r.IsDeleted == true);
            }

            // Filter according to search string
            if (!string.IsNullOrEmpty(paging.searchString))
                users = users.Where(r =>
                                        r.FirstName.Contains(paging.searchString) ||
                                        r.LastName.Contains(paging.searchString) ||
                                        r.Email.Contains(paging.searchString)
                                    );

            #region SORTING
            if (!string.IsNullOrWhiteSpace(paging.sortBy))
            {
                switch (paging.sortBy)
                {
                    case "firstName":
                        {
                            if (paging.isSortAsc)
                                users = users.OrderBy(r => r.FirstName).ThenBy(l => l.LastName);
                            else
                                users = users.OrderByDescending(r => r.FirstName).ThenBy(l => l.LastName);
                        }
                        break;
                    case "lastName":
                        {
                            if (paging.isSortAsc)
                                users = users.OrderBy(r => r.LastName);
                            else
                                users = users.OrderByDescending(r => r.LastName);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                users = users.OrderBy(r => r.Id);
            }
            #endregion SORTING


            try
            {
                var userList = await users.Select(s => new UserModel
                {
                    Id = s.Id,
                    UserName = s.UserName,
                    Email = s.Email,
                    FirstName = s.FirstName,
                    IsActive = s.IsActive,
                    IsDeleted = s.IsDeleted,
                    LastName = s.LastName,
                    UserRoles = userManager.GetRolesAsync(s).Result
                }).ToListAsync();


                var result = userList.Where(u => u.UserRoles.Contains("Admin") || u.UserRoles.Contains("Subscriber"));
                paging.resultCount = result.Count();
                var resultUsers = result.Skip(paging.skip).Take(paging.take).ToList();


                return Ok(new
                {
                    param.draw,
                    recordsTotal = paging.resultCount,
                    recordsFiltered = paging.resultCount,
                    data = resultUsers
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Add new user
        /// </summary>
        /// <param name="model"></param>
        [Authorize(Roles = "Admin")]
        [HttpPost("user", Order = 2)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Post([FromBody] UserModel model)
        {

            // Check if user exists
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                //Create new user
                user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsActive = true,
                    IsDeleted = false
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, model.UserRoles);
                }
                else
                    return BadRequest("Unable to add new user");
            }
            else
            {
                return Ok(new { isSuccess = false, Message = "A user with this email already exists" });
            }

            return Ok(user);

        }

        /// <summary>
        /// Get a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("user/{id}", Order = 3)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(long id)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            return Ok(new UserModel
            {
                Id = id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted,
                UserRoles = await userManager.GetRolesAsync(user)
            });
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="model"></param>
        [Authorize(Roles = "Admin")]
        [HttpPut("user", Order = 4)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromBody] UserModel model)
        {
            if (model == null || model.Id == 0)
                return BadRequest();

            var result = await Mediator.Send(new UpdateUserCommand { UserId = model.Id, FirstName = model.FirstName, LastName = model.LastName, Email = model.Email, IsActive = true, IsDeleted = false });

            if (result)
            {
                return Ok(result);
            }


            return BadRequest("Unable to add new user");
        }

        /// <summary>
        /// Update a user's password
        /// </summary>
        /// <param name="model"></param>
        /// <returns>No content</returns>
        [HttpPut("user/password/{id}", Order = 5)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePassword([FromBody] ResetPasswordModel model, long id)
        {
            var hashPwd = await Mediator.Send(new HashPasswordCommand { UserID = id, Password = model.Password });
            var result = await Mediator.Send(new UpdatePasswordCommand { UserID = id, PasswordHash = hashPwd });

            if (result) return NoContent();
            else return BadRequest("Unable to update password");
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <remarks>Will only mark user as deleted</remarks>
        /// <param name="model"></param>
        /// <returns>No content</returns>
        [HttpDelete("user/{id}", Order = 6)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(long id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound("Could not find user");

            user.IsActive = false;
            user.IsDeleted = true;
            user.LastModified = dateTime.Now;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded) return NoContent();
            else return BadRequest("Unable to delete user");
        }

        /// <summary>
        /// Get User Roles
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("user/roles", Order = 7)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            var response = await Mediator.Send(new GetUserRolesQuery());

            return Ok(response);
        }
    }
}