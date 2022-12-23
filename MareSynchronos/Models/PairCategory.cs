using System;
using System.Collections.Generic;

namespace MareSynchronos.Models
{
    public class PairCategory
    {
        /// <summary>
        /// UUIDv4 to unique track it across name changes
        /// </summary>
        public string CategoryId { get; }
        /// <summary>
        /// The name of the category, to be rendered in the UI
        /// </summary>
        public string CategoryName { get; set; }
        
        /// <summary>
        /// The pair partner UIDs that are to be rendered in this category.
        /// </summary>
        public List<string> PairPartnerUids { get; }

        public PairCategory(string categoryName)
        {
            CategoryName = categoryName;
            CategoryId = Guid.NewGuid().ToString();
            PairPartnerUids = new();
        }
    }
}