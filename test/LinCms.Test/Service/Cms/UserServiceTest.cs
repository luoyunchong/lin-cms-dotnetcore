using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Cms.Groups;
using LinCms.Cms.Users;
using LinCms.Data;
using LinCms.Entities;
using LinCms.Extensions;
using LinCms.IRepositories;
using Xunit;

namespace LinCms.Test.Service.Cms
{
    public class UserServiceTest
    {
        private readonly IUserService _userService;
        private readonly IUserRepository userRepository;
        private readonly IMapper _mapper;

        public UserServiceTest(IUserService userService, IUserRepository userRepository, IMapper mapper) : base()
        {
            _userService = userService;
            this.userRepository = userRepository;
            _mapper = mapper;
        }

        [Fact]
        public async Task ChangePasswordAsync()
        {
            await _userService.ChangePasswordAsync(new ChangePasswordDto()
            {
                ConfirmPassword = "123qwe",
                NewPassword = "123qwe",
                OldPassword = "123qwe"
            });
        }

        [Fact]
        public async Task Delete()
        {
            await _userService.DeleteAsync(14);
        }


        [Fact]
        public void GetUserListByGroupId()
        {
            List<UserDto> linUsers = userRepository.Select
                .IncludeMany(r => r.LinGroups)
                .OrderByDescending(r => r.Id).ToPagerList(new PageDto(), out long totalCount)
                .Select(r =>
                {
                    UserDto userDto = _mapper.Map<UserDto>(r);
                    userDto.Groups = _mapper.Map<List<GroupDto>>(r.LinGroups);
                    return userDto;
                }).ToList();

        }


        [Fact]
        public async Task GetToList()
        {
            var linUsers = await userRepository.Select
                .IncludeMany(r => r.LinGroups.Select(u => new LinGroup { Id = u.Id, Name = u.Name }))
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        [Fact]
        public async Task GetSelectToList()
        {
            var linUsers = await userRepository.Select
                .IncludeMany(r => r.LinGroups.Select(u => new LinGroup { Id = u.Id, Name = u.Name }))
                .OrderByDescending(r => r.Id)
                .ToListAsync(r => new
                {
                    r.Username,
                    r.Email,
                    r.LinGroups
                });
        }

        [Fact]
        public async Task GetSelectDtoToList()
        {
            var linUsers = await userRepository.Select
                .IncludeMany(r => r.LinGroups.Select(u => new LinGroup { Id = u.Id, Name = u.Name }))
                .OrderByDescending(r => r.Id)
                .ToListAsync(r => new UserDto
                {
                    Username = r.Username,
                    Email = r.Email,
                    //Groups = r.LinGroups
                });
        }

    }
}
