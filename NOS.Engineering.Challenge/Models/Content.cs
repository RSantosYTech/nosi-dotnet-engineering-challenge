using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NOS.Engineering.Challenge.Models;

public class Content
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set;  }
    public string Description { get; set;  }
    public string ImageUrl { get; set;  }
    public int Duration { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    //Changing this property to now use 'ContentGenres' class in the IEnumerable, for Entity Framework when converting to SQL Server usage.
    //Adding anotation to prevent JSON from serializing this Navigation property. Instead what is serialized for displaying is below.
    [JsonIgnore]
    public List<ContentGenre> GenreList { get; set; }
    //public IEnumerable<string> GenreList { get; }

    //This string array is read-only and gets the values from the above (real) navigation property.
    //The anottation ensures the property name is respected when the Content records are serialized as JSON in the endpoint responses.
    [JsonPropertyName("GenreList")]
    public string[] GenreListJson 
    {
        get
        {
            return GenreList.Select(x => x.Genre).ToArray();
        }
    }

    //Adding default constructor for binding in EntityFramework.
    public Content()
    {
    }

    //Changing this for compatibily with EF implementation changes and previous mock data generation.
    public Content(Guid id, string title, string subTitle, string description, string imageUrl, int duration, DateTime startTime, DateTime endTime, IEnumerable<string> genreList)
    {
        Id = id;
        Title = title;
        SubTitle = subTitle;
        Description = description;
        ImageUrl = imageUrl;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;

        //Accept the string[] as param, and convert it into the new entity before assigning.
        GenreList = genreList.Select(x => new ContentGenre()
        {
            ID = new Guid(),
            ContentId = id,
            Genre = x
        }).ToList();
        //GenreList = genreList;
    }

    //Adding this constructor overload for compatibily with EF implementation changes in some previous operations.
    public Content(Guid id, string title, string subTitle, string description, string imageUrl, int duration, DateTime startTime, DateTime endTime, ICollection<ContentGenre> genreList)//IEnumerable<string> genreList)
    {
        Id = id;
        Title = title;
        SubTitle = subTitle;
        Description = description;
        ImageUrl = imageUrl;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        GenreList = genreList.ToList();
    }

    //
    /// <summary>
    /// Enables obtaining a "converted" version of the object as a 'ContentDto' for data operations.
    /// </summary>
    public static implicit operator ContentDto (Content toConvert)
    {
        return new ContentDto(
            toConvert.Title,
            toConvert.SubTitle,
            toConvert.Description,
            toConvert.ImageUrl,
            toConvert.Duration,
            toConvert.StartTime,
            toConvert.EndTime,
            toConvert.GenreList
        );
    }
}