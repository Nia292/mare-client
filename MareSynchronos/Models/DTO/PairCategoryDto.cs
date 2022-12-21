using System;
using System.Collections.Generic;

namespace MareSynchronos.Models.DTO
{
    public class PairCategoryDto
    {
        /// <summary>
        /// UUIDv4 to unique track it across name changes
        /// </summary>
        public string CategoryID { get; set; }
        /// <summary>
        /// The name of the category, to be rendered in the UI
        /// </summary>
        public string CategoryName { get; set; }
        
        /// <summary>
        /// The pair partner UIDs that are to be rendered in this category.
        /// </summary>
        public List<string> PairPartnerUIDs { get; set; }

        public PairCategoryDto(string categoryName)
        {
            CategoryName = categoryName;
            CategoryID = Guid.NewGuid().ToString();
            PairPartnerUIDs = new();
        }
    }
}