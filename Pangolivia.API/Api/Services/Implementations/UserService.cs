using System.Collections.Generic;
using System.Threading.Tasks;
using Auth0.ManagementApi.Models;
using AutoMapper;
using Microsoft.Identity.Client;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly Auth0Service _auth0service;

    public UserService(IUserRepository userRepository, IMapper mapper, Auth0Service auth0service)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _auth0service = auth0service;
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

    public async Task<UserModel> getOrCreateUser(string userSub)
    {
        var foundUserModel = await _userRepository.getUserModelByAuth0Sub(userSub);
        if (foundUserModel is null)
        {
            Auth0User userInfo = await _auth0service.GetUsersInfo(userSub);
            foundUserModel = new UserModel()
            {
                Username = userInfo.Nickname,
                Auth0Sub = userInfo.Sub,
                ProfileImageUrl = userInfo.ProfilePictureUrl
            };
            foundUserModel = await _userRepository.createUserModel(foundUserModel);
        }

        return foundUserModel;
    }
}
