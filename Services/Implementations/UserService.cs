using url_shortener.Data;
using url_shortener.Entities;
using url_shortener.Models.Repository.Interface;

namespace url_shortener.Models.Repository.Implementations;
public class Userservice : IUserService
{
    private readonly UrlShortenerContext _context;
    
    public Userservice (UrlShortenerContext context)
    {
        _context = context;
    }


    public void DecreaseRemainingUrls(int userId)
    {
        var user = GetUser(userId);
        if (user.RemainingUrls > 0)
        {
            user.RemainingUrls--;
            _context.Update(user);
            _context.SaveChanges();
        }
        else
        {
            throw APIException.CreateException(APIException.Code.URL_04, "No more URL shortening available", APIException.Type.LIMIT_REACHED);
        }
    }

    // Método para obtener el número de URLs restantes de un usuario
    public int GetRemainingUrls(int userId)
    {
        var user = GetUser(userId);
        return user.RemainingUrls;
    }
    public List<User> GetUsers()
    {
        return _context.Users.ToList();
    }

    public void ResetRemainingUrls(int userId)
    {
        var user = GetUser(userId);
        user.RemainingUrls = 10;
        _context.Update(user);
        _context.SaveChanges();
    }
    public User GetUser(int id)
    {
        User? user = _context.Users.FirstOrDefault((users) => users.Id == id);

        if (user == null)
        {
           throw APIException.CreateException(
                           APIException.Code.US_01,
                           "User not found",
                           APIException.Type.NOT_FOUND);
        }

        return user;
    }

    public User GetUser(string email)
    {
        User? user = _context.Users.FirstOrDefault((users) => users.Email == email);
        if (user == null)
        {
            throw APIException.CreateException(
                APIException.Code.US_01,
                "User not found",
                APIException.Type.NOT_FOUND);
        }

        return user;
    }
    
    public void AddUser(UserForCreationDTO userForCreationDto)
    {
        User? userExist = _context.Users.FirstOrDefault((users) => users.Email == userForCreationDto.Email);
        if (userExist != null)
        {
            throw APIException.CreateException(
                APIException.Code.US_02,
                "User email already exists",
                APIException.Type.BAD_REQUEST);
        }
        
        User user = new()
        {
            Username = userForCreationDto.UserName,
            FirstName = userForCreationDto.FirstName,
            LastName = userForCreationDto.LastName,
            Email = userForCreationDto.Email,
        };
        
        Auth auth = new()
        {
            Password = userForCreationDto.Password,
            Role = "User",
            Id = user.Id
        };
        
        try
        {
            _context.Users.Add(user);
            _context.Auth.Add(auth);
        }
        catch (Exception e)
        {
            throw APIException.CreateException(
                APIException.Code.DB_01,
                "An error occurred while setting the data in the database",
                APIException.Type.INTERNAL_SERVER_ERROR);
        }
        
        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            throw APIException.CreateException(
                APIException.Code.DB_02,
                "An error occurred while saving the data in the database",
                APIException.Type.INTERNAL_SERVER_ERROR);
        }
    }

    public List<XYZ> GetUrls(int userId)
    {
        User? user = GetUser(userId);
        List<XYZ> urls = _context.Urls.Where(url => url.UserId == user.Id).ToList();
        return urls;
    }


    public void UpdateUser(UserForUpdateDTO userForUpdateDto)
    {
        User? toChange = GetUser(userForUpdateDto.UserToChangeID);
        
        User? userExist = _context.Users.FirstOrDefault((users) => users.Email == userForUpdateDto.Email);
        if (userExist != null)
        {
            throw APIException.CreateException(
                APIException.Code.US_02,
                "User email already exists",
                APIException.Type.BAD_REQUEST);
        }
        
        toChange.FirstName = userForUpdateDto.FirstName;
        toChange.LastName = userForUpdateDto.LastName;
        toChange.Email = userForUpdateDto.Email;

        try
        { 
            _context.Users.Update(toChange);
        }
        catch (Exception e)
        {
            throw APIException.CreateException(
                APIException.Code.DB_01,
                "An error occurred while setting the data in the database",
                APIException.Type.INTERNAL_SERVER_ERROR);
        }
        
        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            throw APIException.CreateException(
                APIException.Code.DB_02,
                "An error occurred while saving the data in the database",
                APIException.Type.INTERNAL_SERVER_ERROR);
        }
    }

    public void DeleteUser(int userId)
    {
        User? toRemove = GetUser(userId);

        try
        { 

            _context.Users.Remove(toRemove);
        }
        catch (Exception e)
        {
            throw APIException.CreateException(
                APIException.Code.DB_01,
                "An error occurred while setting the data in the database",
                APIException.Type.INTERNAL_SERVER_ERROR);
        }
        
        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            throw APIException.CreateException(
                APIException.Code.DB_02,
                "An error occurred while saving the data in the database",
                APIException.Type.INTERNAL_SERVER_ERROR);
        }
    }


}