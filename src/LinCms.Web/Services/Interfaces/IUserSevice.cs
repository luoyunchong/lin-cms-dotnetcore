using LinCms.Web.Domain;

namespace LinCms.Web.Services.Interfaces
{
    public interface IUserSevice
    {
        LinUser Authorization(string username, string password);
    }
}