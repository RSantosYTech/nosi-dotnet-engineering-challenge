using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Models
{
    /// <summary>
    /// Entity descriptor used to represent records in an auxiliary table 'ContentGenres' that relates 'Content's to their respective 'Genres'.
    /// </summary>
    public class ContentGenre
    {
        /// <summary>
        /// Unique ID for this record in the auxiliary table.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Foreign Key ID of the corresponding record in the Contents table.
        /// </summary>
        public Guid ContentId { get; set; }
        /// <summary>
        /// Navigation property to allow easy access to parent of this entity.
        /// </summary>
        public Content Content { get; set; }

        /// <summary>
        /// String containing actual genre information of the specific 'Content'.
        /// </summary>
        public string Genre { get; set; }
    }
}
