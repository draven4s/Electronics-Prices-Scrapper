using insta_scrape.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace insta_scrape.Models
{
    public class Search_Details
    {
        public Search_Details()
        {
        }

        public Search_Details(Search o)
        {

        }
        public string search_term { get; set; }
        public int search_amount { get; set; }
    }
}