using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace insta_scrape.Classes
{
    public class Searches: BaseModel
    {
        public int Id { get; set; }
        public string Search { get; set; }
        public string Store { get; set; }
        public DateTime Data { get; set; }
    }
}
