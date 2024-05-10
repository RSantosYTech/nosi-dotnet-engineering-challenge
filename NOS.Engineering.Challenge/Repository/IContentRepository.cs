using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Repository
{
    /// <summary>
    /// Interface representing the repository class for the 'Content' entity, serving as an access layer to DB and EF.
    /// </summary>
    public interface IContentRepository
    {
        public IEnumerable<Content> GetContentsByFilter(string filter = "");

        //Below was deprecated with Task 6, replaced by above.
        public IEnumerable<Content> GetManyContents();

        public Content CreateContent(ContentDto content);

        public Content GetContent(Guid id);

        public Content UpdateContent(Guid id, ContentDto content, bool updateGenres = false);

        public Guid? DeleteContent(Guid id);

    }
}
