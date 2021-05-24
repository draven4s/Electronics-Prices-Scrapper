using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace insta_scrape.Classes
{
    public class UserSearches : BaseModel
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string SearchTerm { get; set; }
        public DateTime Data { get; set; }

    }
}
