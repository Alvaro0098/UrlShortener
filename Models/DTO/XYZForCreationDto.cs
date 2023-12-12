namespace url_shortener.Models;

public record XYZForCreationDto(
    string Name,
    string UrlLong,
    string CategoryName,
    int UserId
);
