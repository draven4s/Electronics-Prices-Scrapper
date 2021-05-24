using insta_scrape.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace insta_scrape.Models
{
    public class Links
    {
        public Links()
        {
        }

        public Links(Link o)
        {
            
        }
        public IEnumerable<string> Link { get; set; }
    }
}