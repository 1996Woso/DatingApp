using API.Models;
using API.Models.DTOs;
using API.Repositories.Account;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly IAccountRepository accountRepository;
    private readonly IMapper mapper;
    private readonly IUsersRepository usersRepository;
    private readonly ITokenService tokenService;

    public AccountController(IAccountRepository accountRepository
    , IMapper mapper
    , IUsersRepository usersRepository
    ,ITokenService tokenService)
    {
        this.accountRepository = accountRepository;
        this.mapper = mapper;
        this.usersRepository = usersRepository;
        this.tokenService = tokenService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO registerDTO)
    {
        if (await usersRepository.UserExistsAsync(registerDTO.Username))
        {
            return BadRequest("User already exists.");
        }
        //var appUserDM = mapper.Map<AppUser>(registerDTO);
        var appUserDM = await accountRepository.RegisterAsync(registerDTO);
        var appUserDTO = mapper.Map<AppUserDTO>(appUserDM);
        appUserDTO.Token = await tokenService.CreateTokenAsync(appUserDM);
        return Ok(appUserDTO);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO loginDTO)
    {
        if (await usersRepository.GetUserByUsernameAsync(loginDTO.Username) == null)
        {
            return Unauthorized("Invalid username.");
        }
        if (!await accountRepository.CorrectPasswordAsync(loginDTO))
        {
            return Unauthorized("Invalid password.");
        }
        var appUserDM = await accountRepository.LoginAsync(loginDTO);
        var appUserDTO = mapper.Map<AppUserDTO>(appUserDM);
        appUserDTO.Token = await tokenService.CreateTokenAsync(appUserDM);
        return Ok(appUserDTO);
    }
}