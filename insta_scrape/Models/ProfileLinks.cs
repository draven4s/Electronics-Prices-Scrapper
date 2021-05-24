using insta_scrape.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace insta_scrape.Models
{
    public class ProfileLinks
    {
        public ProfileLinks()
        {
        }

        public ProfileLinks(ProfileLinks o)
        {
            
        }
        public List<string> Name { get; set; }
        public List<DateTime> Date { get; set; }
    }
}