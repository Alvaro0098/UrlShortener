using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using url_shortener.Entities;
using url_shortener.Models;
using url_shortener.Models.Repository.Implementations;
using url_shortener.Models.Repository.Interface;

namespace url_shortener.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly APIException _apiException;
    private readonly IAuthService _authService;
    
    public UserController(IUserService _userService, APIException apiException, IAuthService authService)
    {
        this._userService = _userService;
        this._apiException = apiException;
        this._authService = authService;
    }

    [HttpGet("{userId}/remaining-urls")]
    public IActionResult GetRemainingUrls(int userId)
    {
        try
        {
            var remainingUrls = _userService.GetRemainingUrls(userId);
            return Ok(remainingUrls);
        }
        catch (Exception e)
        {
            Enum.TryParse(e.Data["type"].ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }
    }

    [HttpPost("{userId}/reset-urls")]
    public IActionResult ResetRemainingUrls(int userId)
    {
        try
        {
            _userService.DecreaseRemainingUrls(userId);
            return Ok("URL count reset successfully.");
        }
        catch (Exception e)
        {
            Enum.TryParse(e.Data["type"].ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }
    }

    [Route("all")]
    [Authorize(Roles = "admin")]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_userService.GetUsers());
    }
    
    [HttpGet("urls/{userId}")]
    public IActionResult getUrls(int userId)
    {
        try
        {
            if (_authService.getCurrentUser() == null)
            {
                return Unauthorized();
            }

            if (_authService.getCurrentUser().Id == userId || _authService.getCurrentUser().Role == "admin")
            {
                var urls = _userService.GetUrls(userId);
                return Ok(urls);
            }
        
            return Unauthorized();
        } catch (Exception e)
        {
            if (e.Data == null)
            {
                return Unauthorized();
            }
            
            if (e.Data["type"] == null || e.Data["type"].ToString() == "NOT_FOUND")
            {
                return Unauthorized();
            }
            
            Enum.TryParse(e.Data["type"].ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }
    }


    [HttpGet("{userId}")]
    public ActionResult<User> GetUser(int userId)
    {
        try
        {
            if (_authService.getCurrentUser() == null)
            {
                return Unauthorized();
            }
            
            if (_authService.getCurrentUser().Id == userId || _authService.getCurrentUser().Role == "admin")
            {
                return Ok(_userService.GetUser(userId));
            }

            return Unauthorized();
        }
        catch (Exception e)
        {
            if (e.Data == null)
            {
                return Unauthorized();
            }
            
            if (e.Data["type"] == null || e.Data["type"].ToString() == "NOT_FOUND")
            {
                return Unauthorized();
            }
            
            Enum.TryParse(e.Data["type"].ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }
    }

    [HttpPost]
    public ActionResult<User> PostUser(UserForCreationDTO userForCreationDto)
    {
        try
        {
            _userService.AddUser(userForCreationDto);
            return Ok("User created successfully");
        }catch (Exception e)
        {
            Enum.TryParse(e.GetType().ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }

    }
    
    [HttpPut]
    [Authorize(Roles = "admin")]
    public ActionResult<User> PutUser(UserForUpdateDTO userForUpdateDto)
    {
        try
        {
            _userService.UpdateUser(userForUpdateDto);
            return Ok("User updated successfully");
        }catch (Exception e)
        {
            Enum.TryParse(e.Data["type"].ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = "admin")]
    public ActionResult<User> DeleteUser(int userId)
    {
        try
        {
            _userService.DeleteUser(userId);
            return Ok("User deleted successfully");
        }catch (Exception e)
        {
            Enum.TryParse(e.Data["type"].ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }
    }
}