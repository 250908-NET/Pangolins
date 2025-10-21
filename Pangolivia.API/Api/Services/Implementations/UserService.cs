using AutoMapper;
using Pangolivia.API.DTOs;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserSummaryDto>> getAllUsersAsync()
    {
        var users = await _userRepository.getAllUserModels();
        return _mapper.Map<IEnumerable<UserSummaryDto>>(users);
    }

    public async Task<UserDetailDto?> getUserByIdAsync(int id)
    {
        var user = await _userRepository.getUserModelById(id);
        return _mapper.Map<UserDetailDto>(user);
    }
    
    public async Task<UserDetailDto?> findUserByUsernameAsync(string username)
    {
        var user = await _userRepository.getUserModelByUsername(username);
        return _mapper.Map<UserDetailDto>(user);
    }
    
    public async Task deleteUserAsync(int id)
    {
        await _userRepository.removeUserModel(id);
    }
}