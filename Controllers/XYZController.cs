using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using url_shortener.Models;
using url_shortener.Models.Repository.Interface;

namespace url_shortener.Controllers;

[ApiController]
[Route("api/[controller]")]
public class XYZController : ControllerBase
{
    private readonly IXYZService _urlsContext;
    private readonly APIException _apiException;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public XYZController(IXYZService urlsContext, APIException apiException, IAuthService authService, IUserService userService)
    {
        _urlsContext = urlsContext;
        _apiException = apiException;
        _authService = authService;
        _userService = userService;
    }

    [Route("all")]
    [Authorize(Roles = "admin")]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_urlsContext.GetAll());
    }

    [Route("getLong")]
    [HttpGet]
    public IActionResult GetUrlLongByShort(string urlShort)
    {
        try
        {
            var urlLongByShort = _urlsContext.getUrlLongByShort(urlShort);
        
            return Ok(urlLongByShort);            
        } 
        catch (Exception e)
        {
            Enum.TryParse(e.Data["type"].ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }
    }
    
    [Route("getById")]
    [HttpGet]
    public IActionResult GetById(int id)
    {
        if (_authService.getCurrentUser() == null)
        {
            return Unauthorized("You are not logged in");
        }
        
        if (!_authService.isSameUserRequest(_urlsContext.getById(id).UserId))
        {
            return Unauthorized("You are not allowed to get a url for another user");
        }
        
        try
        {
            var url = _urlsContext.getById(id);
        
            return Ok(url);            
        } 
        catch (Exception e)
        {
            Enum.TryParse(e.Data["type"].ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }
    }

    [Route("create")]
    [HttpPost]
    public IActionResult CreateUrl(XYZForCreationDto creationDto)
    {
        if (_authService.getCurrentUser() == null)
        {
            return Unauthorized("You are not logged in");
        }
        
        if (!_authService.isSameUserRequest(creationDto.UserId))
        {
            return Unauthorized("You are not allowed to create a url for another user");
        }
        
        try
        {
            var url = _urlsContext.createUrl(creationDto);

            return Ok(url);   
        } 
        catch (Exception e)
        {
            Enum.TryParse(e.Data["type"].ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }
    }
    
    [Route("deleteById")]
    [HttpDelete]
    public IActionResult DeleteUrl(int id)
    {
        if (_authService.getCurrentUser() == null)
        {
            return Unauthorized("You are not logged in");
        }
        
        if (!_authService.isSameUserRequest(_urlsContext.getById(id).UserId))
        {
            return Unauthorized("You are not allowed to delete a url for another user");
        }
        
        try
        {
            _urlsContext.deleteUrl(id);
            return Ok("Url " + id + " deleted");
        }catch (Exception e)
        {
            Enum.TryParse(e.Data["type"].ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }
    }
    
    [Route("deleteByShort")]
    [HttpDelete]
    public IActionResult DeleteUrl(string urlShort)
    {
        if (_authService.getCurrentUser() == null)
        {
            return Unauthorized("You are not logged in");
        }
        
        if (!_authService.isSameUserRequest(_urlsContext.getUrlLongByShort(urlShort).UserId))
        {
            return Unauthorized("You are not allowed to delete a url for another user");
        }
        
        try
        {
            _urlsContext.deleteUrl(urlShort);
            return Ok("Url " + urlShort + " deleted");   
        }catch (Exception e)
        {
            Enum.TryParse(e.Data["type"].ToString(), out APIException.Type type);

            return _apiException.getResultFromError(type, e.Data);
        }
    }
}