using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using NOS.Engineering.Challenge.Repository;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;

    //Injecting Dependencies.

    /// <summary>
    /// DI for the repository class that serves as the access layer to SQL Server and Entity Framework.
    /// </summary>
    private readonly IContentRepository _contentRepository;

    /// <summary>
    /// Logger reference for this controller.
    /// </summary>
    private readonly ILogger<ContentController> _logger;

    public ContentController(IContentsManager manager, IContentRepository contentRepos, ILogger<ContentController> logger)
    {
        _manager = manager;
        //
        _contentRepository = contentRepos;
        //
        _logger = logger;
    }
    
    //Commented out for Task 6 (Deprecate GET /Content and replace with new filter-able method below)

    //[HttpGet]
    ////Added in annotation to enable response caching for this endpoint method.
    ////Enabling the cache for 10min as a default, the server will fetch new data when requested every 10 min (content releases?)
    //[ResponseCache(Duration = 600)]
    //public async Task<IActionResult> GetManyContents()
    //{
    //    //Change from pre-exisiting database to SQL Server and EF.
    //    //var contents = await _manager.GetManyContents().ConfigureAwait(false);
    //    var contents = _contentRepository.GetManyContents();

    //    //Adding in logging functionality.
    //    _logger.LogInformation("[{0}]: GET Call to \"/Content\".", DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss"));

    //    if (!contents.Any())
    //        return NotFound();
        
    //    return Ok(contents);
        
    //}

    //The async modifier keywords and the Task<> return types have been removed below after
    // transitioning from the pre-existing database to SQL Server and EF.

    [HttpGet]
    //Added in annotation to enable response caching for this endpoint method.
    //Enabling the cache for 10min as a default, the server will fetch new data when requested every 10 min (content releases?)
    //Also added VaryByQueryKeys to the new version of the deprecated method to ensure the response will change based on the query filter string.
    [ResponseCache(Duration = 600, VaryByQueryKeys = new string[] { "filterString" })]
    public IActionResult GetContentsByFilter(string filterString = "")
    {
        //Change from pre-exisiting database to SQL Server and EF.
        //var contents = await _manager.GetManyContents().ConfigureAwait(false);
        var contents = _contentRepository.GetContentsByFilter(filterString);

        //Adding in logging functionality.
        _logger.LogInformation("[{0}]: GET Call to \"/Content\".", DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss"));

        if (!contents.Any())
            return NotFound();

        return Ok(contents);

    }

    [HttpGet("{id}")]
    //Added in annotation to enable response caching for this endpoint method.
    //Enabling the cache for 5min, 
    //This one also does not use a cached response if the query parameter (Content ID) changes.
    [ResponseCache(Duration = 300, VaryByQueryKeys = new string[] { "id" })]
    public IActionResult GetContent(Guid id)
    {
        //Change from pre-exisiting database to SQL Server and EF.
        //var content = await _manager.GetContent(id).ConfigureAwait(false);
        var content = _contentRepository.GetContent(id);


        //Adding in logging functionality.
        _logger.LogInformation("[{0}]: GET Call to \"/Content/{1}\". Success: {2}.", DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss"), id.ToString(), (content != null).ToString());

        if (content == null)
            return NotFound();
        
        return Ok(content);
    }
    
    [HttpPost]
    public IActionResult CreateContent(
        [FromBody] ContentInput content
        )
    {
        //Change from pre-exisiting database to SQL Server and EF.
        //var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);
        Content createdContent = _contentRepository.CreateContent(content.ToDto());


        //Adding in logging functionality.
        _logger.LogInformation("[{0}]: POST Call to \"/Content\". Success: {1}.", DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss"), (createdContent != null).ToString());

        return createdContent == null ? Problem() : Ok(createdContent);
    }
    
    [HttpPatch("{id}")]
    public IActionResult UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        //Change from pre-exisiting database to SQL Server and EF.
        //var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);
        var updatedContent = _contentRepository.UpdateContent(id, content.ToDto());


        //Adding in logging functionality.
        _logger.LogInformation("[{0}]: PATCH Call to \"/Content/{1}\". Success: {2}.", DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss"), id.ToString(), (updatedContent != null).ToString());

        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteContent(
        Guid id
    )
    {
        //Change from pre-exisiting database to SQL Server and EF.
        //var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
        var deletedId = _contentRepository.DeleteContent(id);

        //Adding in logging functionality.
        _logger.LogInformation("[{0}]: DELETE Call to \"/Content/{1}\".", DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss"), id.ToString());


        return Ok(deletedId);
    }

    //
    //Implementing Exercise Endpoint Methods
    //
    /// <summary>
    /// POST to Genre, adds new Genres.
    /// </summary>
    /// <param name="id">Content ID of the record whose Genres will be changed.</param>
    /// <param name="genre">Enumerable strings to be added to the corresponding Content's </param>
    [HttpPost("{id}/genre")]
    public IActionResult AddGenres(Guid id, [FromBody] IEnumerable<string> genre)
    {
        //First, try to find a matching Content record for the given ID param.

        //Change from pre-exisiting database to SQL Server and EF.
        //var content = await _manager.GetContent(id).ConfigureAwait(false);
        var content = _contentRepository.GetContent(id);

        //Adding in logging functionality.
        _logger.LogInformation("[{0}]: POST Call to \"/Content/{1}/genre\". Success: {2}.", DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss"), id.ToString(), (content != null).ToString());

        //If no match was found, return an error message.
        if (content == null)
            return NotFound();

        //Get a Dto object from the content using implicit operator, to be passed onto update function.
        ContentDto updateData = content;

        //Build the aggregated list, ensuring there are no duplicate entries in the genres for the selected Content record.
        updateData.GenreList = updateData.GenreList.Where(x => !genre.Contains(x.Genre))
            //Add a new built Enumerable of the supporting entities, to be added as records into the respective auxiliary table in DB.
            .Concat(genre.Select(y => new ContentGenre()
            {
                //Foreign key ID of the corresponding 'Content' record.
                ContentId = content.Id,
                //Genre string.
                Genre = y,
            })).ToList();


        //Attempt to update the data for the record matching param ID, if it is found.

        //Change from pre-exisiting database to SQL Server and EF.
        //var updatedContent = await _manager.UpdateContent(id, updateData).ConfigureAwait(false);
        var updatedContent = _contentRepository.UpdateContent(id, updateData, true);

        //At this point, the 'updatedContent' should never be null, 
        //If no match was found, return error message, otherwise send an "OK" message, with the updated record's data.
        return updatedContent == null ? NotFound() : Ok(updatedContent);

    }

    /// <summary>
    /// DELETE to Genre, deletes Genres from specified ID Content.
    /// </summary>
    [HttpDelete("{id}/genre")]
    public IActionResult RemoveGenres(Guid id, [FromBody] IEnumerable<string> genre)
    {
        //Change from pre-exisiting database to SQL Server and EF.
        //var content = await _manager.GetContent(id).ConfigureAwait(false);
        var content = _contentRepository.GetContent(id);

        //Adding in logging functionality.
        _logger.LogInformation("[{0}]: DELETE Call to \"/Content/{1}/genre\". Success: {2}.", DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss"), id.ToString(), (content != null).ToString());

        //If no match was found, return an error message.
        if (content == null)
            return NotFound();

        //Get a Dto object from the content using implicit operator, to be passed onto update function.
        ContentDto updateData = content;

        //Remove all existing genres from param in update data object.
        updateData.GenreList = content.GenreList.Where(x => !genre.Contains(x.Genre)).ToList();

        //Attempt to update the data for the record matching param ID, if it is found.

        //Change from pre-exisiting database to SQL Server and EF.
        //var updatedContent = await _manager.UpdateContent(id, updateData).ConfigureAwait(false);
        var updatedContent = _contentRepository.UpdateContent(id, updateData, true);

        //At this point, the 'updatedContent' should never be null, 
        //If no match was found, return error message, otherwise send an "OK" message, with the updated record's data.
        return updatedContent == null ? NotFound() : Ok(updatedContent);

    }

}