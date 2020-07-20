using System.Threading.Tasks;
using LinCms.Cms.Users;
using Xunit;

namespace LinCms.Test.Service.Cms
{
    public class UserServiceTest : BaseLinCmsTest
    {
        private readonly IUserService _userService;

        public UserServiceTest() : base()
        {
            _userService = GetService<IUserService>();
        }

        [Fact]
        public async  Task ChangePasswordAsync()
        {
          await  _userService.ChangePasswordAsync(new ChangePasswordDto()
            {
                ConfirmPassword = "123qwe",
                NewPassword = "123qwe",
                OldPassword = "123qwe"
            });
        }
        
        [Fact]
        public async  Task Delete()
        {
            await  _userService.DeleteAsync(14);
        }


    }
}
