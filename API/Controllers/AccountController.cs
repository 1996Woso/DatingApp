using API.Interfaces;
using API.Models.Domain;
using API.Models.DTOs;
using API.Repositories.Account;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly IAccountRepository accountRepository;
    private readonly IMapper mapper;
    private readonly IUsersRepository usersRepository;
    private readonly ITokenService tokenService;
    private readonly UserManager<AppUser> userManager;

    public AccountController(IAccountRepository accountRepository
    , IMapper mapper
    , IUsersRepository usersRepository
    ,ITokenService tokenService
    ,UserManager<AppUser> userManager)
    {
        this.accountRepository = accountRepository;
        this.mapper = mapper;
        this.usersRepository = usersRepository;
        this.tokenService = tokenService;
        this.userManager = userManager;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {
        if (await usersRepository.UserExistsAsync(registerDTO.Username))
        {
            return BadRequest($"Username '{registerDTO.Username}' already exists.");
        }
        var appUserDM = mapper.Map<AppUser>(registerDTO);
        appUserDM.UserName = registerDTO.Username.ToLower();
        var result = await userManager.CreateAsync(appUserDM, registerDTO.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        var appUserDTO = mapper.Map<AppUserDTO>(appUserDM);
        appUserDTO.Token = await tokenService.CreateTokenAsync(appUserDM);
        return Ok(appUserDTO);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO loginDTO)
    {
        var appUserDM = await accountRepository.LoginAsync(loginDTO);
        if (appUserDM == null || appUserDM.UserName == null)
        {
            return Unauthorized("Invalid username.");
        }
        if (!await accountRepository.CorrectPasswordAsync(loginDTO))
        {
            return Unauthorized("Invalid password.");
        }
        
        var appUserDTO = mapper.Map<AppUserDTO>(appUserDM);
        appUserDTO.Token = await tokenService.CreateTokenAsync(appUserDM);
        return Ok(appUserDTO);
    }
}