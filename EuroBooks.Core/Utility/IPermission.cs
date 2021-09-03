namespace EuroBooks.Infrastructure.Utility
{
    public interface IPermission
    {
        bool IsInRole(string role);
    }
}
