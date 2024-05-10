using System.ComponentModel.DataAnnotations.Schema;

namespace NOS.Engineering.Challenge.Models;

//Adding NotMapped annotation as this class is meant for use in data operations in server side only.
[NotMapped]
public class ContentDto
{
    public ContentDto(
        string? title,
        string? subTitle,
        string? description,
        string? imageUrl,
        int? duration,
        DateTime? startTime,
        DateTime? endTime,

        //Changing this for compatibily with EF implementation changes.
        ICollection<ContentGenre> genreList)
        //IEnumerable<string> genreList)
    {
        Title = title;
        SubTitle = subTitle;
        Description = description;
        ImageUrl = imageUrl;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        //
        GenreList = genreList.ToList();
    }

    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? Duration { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    //Changing this property to now use 'ContentGenres' class in the IEnumerable, for Entity Framework when converting to SQL Server usage.
    public List<ContentGenre> GenreList { get; set; }
    //public IEnumerable<string> GenreList { get; set; }
    
}