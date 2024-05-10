using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.Data;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Repository
{
    /// <summary>
    /// Class that is injected via DI into needed controllers, etc. and is used as an access layer to the SQL Server database and EF.
    /// </summary>
    public class ContentRepository : IContentRepository
    {
        /// <summary>
        /// Dependency Injection of database context to allow access and operations.
        /// </summary>
        private readonly ContentDbContext _context;

        public ContentRepository(ContentDbContext context) 
        {
            _context = context;
        }


        /// <summary>
        /// New method to replace the below 'GetManyContents, allowing for a param string filter for Content Titles and Genres.
        /// </summary>
        public IEnumerable<Content> GetContentsByFilter(string filter = "")
        {
            //If no filter was given in param, return all, as was the function of the deprecated method below.
            if(string.IsNullOrEmpty(filter))
                return _context.Contents.Include(x => x.GenreList).ToList();

            return _context.Contents.Include(x => x.GenreList)
                //        Looking for filter string in title, and among any of the record's genres.
                .Where(x => x.Title.Contains(filter) || x.GenreList.Any(y => y.Genre.Contains(filter))).ToList();
        }

        /// <summary>
        /// Return all Content records in DB.
        /// </summary>
        public IEnumerable<Content> GetManyContents()
        {
            return _context.Contents.Include(x => x.GenreList).ToList();
        }

        /// <summary>
        /// Creates a new instance of 'Content' from param data and inserts it as a record in DB.
        /// </summary>
        public Content CreateContent(ContentDto content)
        {
            //Create a new instance of Content based on param data, with a unique ID.
            Content toInsert = new Content()
            {
                Id = Guid.NewGuid(),
                //
                Title = content.Title,
                SubTitle = content.SubTitle,
                Description = content.Description,
                Duration = (int)content.Duration,
                ImageUrl = content.ImageUrl,
                StartTime = (DateTime)content.StartTime,
                EndTime = (DateTime)content.EndTime,
                GenreList = content.GenreList
            };

            //Perform the operation and save changes to DB, returning required data.
            _context.Contents.Add(toInsert);

            _context.SaveChanges();

            return toInsert;
        }

        /// <summary>
        /// Returns a specific 'Content' record from DB.
        /// </summary>
        public Content GetContent(Guid id)
        {
            //Find match by param ID and return it.
            return _context.Contents.Include(x => x.GenreList).Where(x => x.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Performs update on a given record in DB based on param ID. Optional parameter determines whether Content<->ContentGeneres relations should de recreated based on param data.
        /// </summary>
        public Content UpdateContent(Guid id, ContentDto content, bool updateGenres = false)
        {
            //First find a match on param ID.
            Content matchFound = _context.Contents.Include(x => x.GenreList).Where(x => x.Id == id).FirstOrDefault();

            //If no match is found, stop method.
            if (matchFound == null)
                return null;

            //Update match data from param data.
            matchFound.Title = content.Title;
            matchFound.SubTitle = content.SubTitle;
            matchFound.Description = content.Description;
            matchFound.Duration = (int)content.Duration;
            matchFound.ImageUrl = content.ImageUrl;
            matchFound.StartTime = (DateTime)content.StartTime;
            matchFound.EndTime = (DateTime)content.EndTime;

            //If optionally relation records are also being re-created.
            if(updateGenres)
            {
                //Delete existing relation records.
                _context.ContentGenres.RemoveRange(_context.ContentGenres.Where(x => x.ContentId == id));
                
                //Build new records with unique IDs to represent the Content-Genre relations.
                _context.ContentGenres.AddRange(content.GenreList.Select(x => new ContentGenre()
                {
                    ID = new Guid(),
                    ContentId = x.ContentId,
                    Genre = x.Genre
                }));
            }

            //Perform the operation and save changes to DB, returning required data.
            //_context.Contents.Update(matchFound);
            _context.Contents.Entry(matchFound).State = EntityState.Modified;

            _context.SaveChanges();

            return matchFound;
        }

        /// <summary>
        /// Deletes a specific record from DB on param ID.
        /// </summary>
        public Guid? DeleteContent(Guid id)
        {
            //First find a match on param ID.
            Content matchFound = _context.Contents.Include(x => x.GenreList).Where(x => x.Id == id).FirstOrDefault();

            //If no match is found, stop method.
            if (matchFound == null)
                return null;

            //Perform the operation and save changes to DB, returning required data.
            _context.Remove(matchFound);

            _context.SaveChanges();
            //
            return matchFound.Id;
        }

    }
}
