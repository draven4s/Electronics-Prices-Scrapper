using insta_scrape.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace insta_scrape.Models
{
    public class UserViewModel
    {
        public UserViewModel()
        {
        }

        public UserViewModel(User o)
        {
            Id = o.Id;
            name = o.name;
            firstname = o.firstname;
            lastname = o.lastname;
            license = o.license;
            money = o.money;
            bank = o.bank;
            Admin = o.admin;
        }
        public int Id { get; set; } = 0;
        public string name { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string license { get; set; }
        public int money { get; set; }
        public int bank { get; set; }
        public int Admin { get; set; }

        public IEnumerable<SelectListItem> Ranks { get; set; }
    }
}