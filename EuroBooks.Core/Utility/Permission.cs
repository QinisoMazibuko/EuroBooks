using Microsoft.AspNetCore.Http;

namespace EuroBooks.Infrastructure.Utility
{
    public class Permission : IPermission
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Permission(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public bool IsInRole(string role)
        {
            if (!_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey("roles"))
                return false;


            if (_httpContextAccessor.HttpContext.Request.Cookies["roles"] == role)
                return true;


            return false;
        }
    }
}
