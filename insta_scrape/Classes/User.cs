using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace insta_scrape.Classes
{
    public class User : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public string identifier { get; set; }
        public string license { get; set; }
        public string name { get; set; }
        public string skin { get; set; }
        public string job { get; set; }
        public int job_grade { get; set; }
        public string position { get; set; }
        public int permission_level { get; set; }
        public string group { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string dateofbirth { get; set; }
        public string sex { get; set; }
        public int height { get; set; }
        public string status { get; set; }
        public int is_dead { get; set; }
        public string last_property { get; set; }
        public string phone_number { get; set; }
        public int jail { get; set; }
        public string tattoos { get; set; }
        public string last_motel { get; set; }
        public string last_motel_room { get; set; }
        public int bank { get; set; }
        public int money { get; set; }
        public int last_house { get; set; }
        public string loadout { get; set; }
        public int pokerchips { get; set; }
        public int active { get; set; }
        public string model { get; set; }
        public string drawables { get; set; }
        public string props { get; set; }
        public string drawtextures { get; set; }
        public string proptextures { get; set; }
        public string hairColor { get; set; }
        public string headBlend { get; set; }
        public string headOverlay { get; set; }
        public string headStructure { get; set; }
        public int armour { get; set; }
        public string paycheck { get; set; }
        public string jailitems { get; set; }
        public string metaData { get; set; }
        public string bones { get; set; }
        public string emotes { get; set; }
        public int hunger { get; set; }
        public int health { get; set; }
        public int thirst { get; set; }
        public string phone { get; set; }
        public string profilepicture { get; set; }
        public string background { get; set; }
        public string iban { get; set; }
        public int xp { get; set; }
        public int admin { get; set; }
    }
}