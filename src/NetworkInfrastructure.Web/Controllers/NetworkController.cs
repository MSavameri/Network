using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetworkInfrastructure.Web.Data.Services;
using NetworkInfrastructure.Web.Models;
using System.Security.Claims;
using AutoMapper;
using NetworkInfrastructure.Web.Data.Entities;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NetworkInfrastructure.Web.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class NetworkController : Controller
    {
        private readonly INetworkService _networkService;
        private readonly IUserService _userService;
        private readonly IValidator<UserDto> _userValidator;
        private readonly IValidator<NetworkAssetDto> _netValidator;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public NetworkController(INetworkService networkService,
            IUserService userService,
            IValidator<UserDto> validator,
            IConfiguration configuration,
            IMapper mapper,
            IValidator<NetworkAssetDto> netValidator)
        {
            _networkService = networkService;
            _userService = userService;
            _userValidator = validator;
            _configuration = configuration;
            _mapper = mapper;
            _netValidator = netValidator;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            //foreach (var cookie in HttpContext.Request.Cookies)
            //{
            //    Response.Cookies.Delete(cookie.Key);
            //}
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserDto user)
        {
            var domainName = _configuration["Domain:Name"];

            var validate = _userValidator.Validate(user);

            if (!validate.IsValid)
            {
                validate.AddToModelState(this.ModelState);
                return View(user);
            }

            if (await _userService.Validate(domainName, user))
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        //new Claim("FullName", user.UserName),
                        new Claim(ClaimTypes.Role, "Administrator"),
                    };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction(nameof(Index));
            }

            ViewData["ErrMesage"] = "Invalid login attempt.";

            return View(user);
        }


        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }

        public async Task<IActionResult> Index(string search)
        {

            ViewData["searchParam"] = search;

            var query = await _networkService.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.ServerName.Contains(search) ||
                x.Ip.Contains(search) ||
                x.Description.Contains(search) ||
                x.ServerPort.Contains(search) ||
                x.ServiceOwner.Contains(search)).ToList();
            }

            var mapper = _mapper.Map<List<NetworkAssetDto>>(query);

            return View(mapper);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NetworkAssetDto model)
        {
            var validate = _netValidator.Validate(model);

            if (!validate.IsValid)
            {
                validate.AddToModelState(this.ModelState);
                return View(model);
            }

            model.UserName = string.IsNullOrEmpty(ClaimTypes.Name)
                ? throw new ArgumentNullException(nameof(User))
                : User.FindFirstValue(claimType: ClaimTypes.Name);

            var mapper = _mapper.Map<NetworkAsset>(model);

            await _networkService.AddAsync(mapper);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _networkService.GetAsync(id);

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var mapper = _mapper.Map<NetworkAssetDto>(result);

            return View(mapper);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            await _networkService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _networkService.GetAsync(id);

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var mapper = _mapper.Map<NetworkAssetDto>(result);

            return View(mapper);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(NetworkAssetDto model)
        {
            var validate = _netValidator.Validate(model);

            if (!validate.IsValid)
            {
                validate.AddToModelState(this.ModelState);
                return View(model);
            }

            var mapper = _mapper.Map<NetworkAsset>(model);

            await _networkService.EditAsync(mapper);

            return RedirectToAction(nameof(Index));
        }
    }
}
