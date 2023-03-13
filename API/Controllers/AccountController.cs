using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class AccountController : BaseApiController
  {
      private readonly UserManager<User> _userManager;
      private readonly TokenService _tokenService;
      private readonly StoreContext _context;
      public AccountController(UserManager<User> userManager, TokenService tokenService, StoreContext context)
      {
          _context = context;
          _tokenService = tokenService;
          _userManager = userManager;

      }
      
      [HttpPost("login")]
      public async Task<ActionResult<UserDto>> Login(LoginDto loginDTO)
      {
          // when we log in, we're going to need to check if we have a basket on a server for that user, to check if we've got a basket for the anonymous user and to take appropriate action. If we have a basket for the user, then we're going to add it to our userDto and return it with the email and token we're already sending back. 
          var user = await _userManager.FindByNameAsync(loginDTO.Username);
          if ((user == null) || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
              return Unauthorized();
            
          var userBasket = await RetrieveBasket(loginDTO.Username);
          var anonBasket = await RetrieveBasket(Request.Cookies["buyerId"]);

          if (anonBasket != null)
          {
              if (userBasket != null) _context.Baskets.Remove(userBasket);
              anonBasket.BuyerId = user.UserName;
              Response.Cookies.Delete("buyerId");
              await _context.SaveChangesAsync();
          }
          
          return new UserDto
          {
              Email = user.Email,
              Token = await _tokenService.GenerateToken(user),
              Basket = anonBasket != null ? anonBasket.MapBasketToDto() : userBasket?.MapBasketToDto()
          };
      }

      [HttpPost("register")]
      public async Task<ActionResult> Register(RegisterDto registerDto)
      {
          var user = new User{UserName = registerDto.Username, Email = registerDto.Email};

          var result = await _userManager.CreateAsync(user, registerDto.Password);

          if (!result.Succeeded)
          {
              foreach (var error in result.Errors)
              {
                  ModelState.AddModelError(error.Code, error.Description);
              }

              return ValidationProblem();
          }

          await _userManager.AddToRoleAsync(user, "Member");

          return StatusCode(201);
      }

      [Authorize] // this will protect this individual endpoint 
      [HttpGet("currentUser")] // we're going to use our token to get the user from the database and return a userDTO with the token back from this method.
      public async Task<ActionResult<UserDto>> GetCurrentUser()
      // no parameters because we want to get the user info from the token we're going to send up with our request
      {
          var user = await _userManager.FindByNameAsync(User.Identity.Name); // User.Identity.Name is going to go and get our name claim from our token just by using it.
          var userBasket = await RetrieveBasket(User.Identity.Name);

          return new UserDto
          {
              Email = user.Email,
              Token = await _tokenService.GenerateToken(user),
              Basket = userBasket?.MapBasketToDto()
          };
      }
      // in order for this to work, we need to tell our application how we are authenticating to our API in Program.cs 

      private async Task<Basket> RetrieveBasket(string buyerId)
        {
          if (string.IsNullOrEmpty(buyerId))
          {
              Response.Cookies.Delete("buyerId");
              return null;
          }
          
          return await _context.Baskets
              .Include(i => i.Items)
              .ThenInclude(p => p.Product)
              .FirstOrDefaultAsync(x => x.BuyerId == buyerId);

        }
  }
}