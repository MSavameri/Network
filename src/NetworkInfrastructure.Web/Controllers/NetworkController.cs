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
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<NetworkController> _logger;

        public NetworkController(INetworkService networkService,
            IUserService userService,
            IValidator<UserDto> validator,
            IConfiguration configuration,
            IMapper mapper,
            IValidator<NetworkAssetDto> netValidator,
            ILogger<NetworkController> logger)
        {
            _networkService = networkService;
            _userService = userService;
            _userValidator = validator;
            _configuration = configuration;
            _mapper = mapper;
            _netValidator = netValidator;
            _logger = logger;
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
                _logger.LogWarning("Login validation failed for user {UserName}.", user.UserName);
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

                _logger.LogInformation("User {UserName} logged in successfully.", user.UserName);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Invalid login attempt for user {UserName}.", user.UserName);
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
            _logger.LogInformation("Attempting to create network asset with ServerName: {ServerName}", model.ServerName);
            try
            {
                var validate = _netValidator.Validate(model);

                if (!validate.IsValid)
                {
                    _logger.LogWarning("Validation failed for creating network asset with ServerName: {ServerName}", model.ServerName);
                    validate.AddToModelState(this.ModelState);
                    return View(model);
                }

                model.UserName = string.IsNullOrEmpty(ClaimTypes.Name)
                    ? throw new ArgumentNullException(nameof(User))
                    : User.FindFirstValue(claimType: ClaimTypes.Name);

                var mapper = _mapper.Map<NetworkAsset>(model);

                await _networkService.AddAsync(mapper);
                _logger.LogInformation("Successfully created network asset with ServerName: {ServerName}, ID: {Id}", model.ServerName, mapper.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException while creating network asset {ServerName}: User claim for UserName was null or empty.", model.ServerName);
                // Potentially return a specific error view or message
                throw; // Re-throwing as it's a critical issue for this action
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating network asset {ServerName}.", model.ServerName);
                // Handle appropriately, maybe return an error view
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier }); // Assuming you have an ErrorViewModel
            }
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
                _logger.LogWarning("DeleteAsync called with null or empty ID.");
                throw new ArgumentNullException(nameof(id));
            }
            _logger.LogInformation("Attempting to delete network asset with ID: {Id}", id);
            try
            {
                await _networkService.DeleteAsync(id);
                _logger.LogInformation("Successfully deleted network asset with ID: {Id}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting network asset with ID: {Id}", id);
                // Handle appropriately, maybe return an error view
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier }); // Assuming you have an ErrorViewModel
            }
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
            _logger.LogInformation("Attempting to edit network asset with ID: {Id}", model.Id);
            try
            {
                var validate = _netValidator.Validate(model);

                if (!validate.IsValid)
                {
                    _logger.LogWarning("Validation failed for editing network asset with ID: {Id}", model.Id);
                    validate.AddToModelState(this.ModelState);
                    return View(model);
                }

                var mapper = _mapper.Map<NetworkAsset>(model);

                await _networkService.EditAsync(mapper);
                _logger.LogInformation("Successfully edited network asset with ID: {Id}", model.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while editing network asset with ID: {Id}", model.Id);
                // Handle appropriately, maybe return an error view
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier }); // Assuming you have an ErrorViewModel
            }
        }
    }
}
