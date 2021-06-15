using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using insta_scrape.Classes;
using insta_scrape.Models;
using System.Text;
namespace insta_scrape.Controllers
{
    public class ScrapeController : Controller
    {
        private DBController DB = new DBController();
        [Authorize]
        public ActionResult Scrape()
        {
            return View();
        }
        [Authorize]
        public ActionResult Main_Scrape()
        {
            ViewBag.Title = "Scraped data";
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult Main_Scrape(Search_Details sDetails)
        {
            if (!string.IsNullOrEmpty(sDetails.search_term))
            {
                var actualResult = DB.GetList<Searches>(x => x.Search.Equals(sDetails.search_term)).ToList();
                var scrapeResult = new Searches();
                var stras = sDetails.search_store.ToString();
                if(actualResult.FirstOrDefault<Searches>(x => x.Store.Contains(stras.ToString())) != null){
                    scrapeResult = actualResult.First<Searches>(x => x.Store.Contains(stras.ToString()));
                }

                UserSearches userSearch = new UserSearches { Data = DateTime.Now, SearchTerm = sDetails.search_term, User = HttpContext.User.Identity.Name };
                DB.Save(userSearch);

                TimeSpan tarpas = new TimeSpan();

                if (scrapeResult != null)
                {
                    tarpas = (DateTime.Now - scrapeResult.Data);
                }
                if (scrapeResult == null || (tarpas == TimeSpan.Zero || tarpas.Hours >= 2))
                {
                    //full pythono consolės outputas
                    string scrapeData = RunScrape(sDetails);
                    //jį sukapojam į dalis, kad gyvent lengviau būtų
                    var data = new List<FSpecs> { };
                    if (stras == "Topocentras")
                        data = RipStringApart(scrapeData, sDetails);
                    else if (stras == "Avitela")
                        data = RipStringApartAvitela(scrapeData, sDetails);
                    ViewBag.Specs = data;

                }
                else
                {
                    List<FSpecs> specsReturn = new List<FSpecs> { };
                    var searchData = DB.GetList<Searches>(x => x.Search.Equals(sDetails.search_term));
                    var test = searchData.SingleOrDefault<Searches>(x => x.Store.Equals(sDetails.search_store.ToString()));
                    var data = DB.GetList<Phone>(x => x.SearchId.Equals(test.Id)).ToList();
                    foreach (var ph in data)
                    {
                        List<string> linkai = ph.ImageLinks.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        List<string> Linkai1 = linkai.Select(x => x.Replace("https://picfit.topocentras.lt/cdn-cgi/image/fit=contain,format=auto,height=60,width=60,quality=80/", "")).ToList();
                        Linkai1 = Linkai1.Select(x => x.Replace("media/catalog/", "https://www.topocentras.lt/media/catalog/")).ToList();
                        FSpecs temp = new FSpecs { };
                        temp.links = Linkai1;
                        temp.phone = ph;
                        specsReturn.Add(temp);
                    }
                    ViewBag.Specs = specsReturn;

                }
            }
            return View("Main_Scrape", sDetails);
        }
        [HttpGet]
        [Authorize]
        public ActionResult Main_Scrape(string search_term, string sortOrder, string store)
        {

            List<FSpecs> specsReturn = new List<FSpecs> { };
            Search_Details sDetails = new Search_Details { search_term = search_term};
            if (!string.IsNullOrEmpty(search_term))
            {
                //gaunam visą console outputą vienam stringe
                var actualResult = DB.GetList<Searches>(x => x.Search.Equals(sDetails.search_term)).ToList();
                var scrapeResult = new Searches();


                if (actualResult.FirstOrDefault<Searches>(x => x.Store.Contains(store.ToString())) != null)
                {
                    scrapeResult = actualResult.First<Searches>(x => x.Store.Contains(store));
                }

                UserSearches userSearch = new UserSearches { Data = DateTime.Now, SearchTerm = search_term, User = HttpContext.User.Identity.Name };
                DB.Save(userSearch);

                TimeSpan tarpas = new TimeSpan();

                if (scrapeResult != null)
                {
                    tarpas = (DateTime.Now - scrapeResult.Data);
                }
                if (scrapeResult == null || (tarpas == TimeSpan.Zero || tarpas.Hours >= 2))
                {
                    //full pythono consolės outputas
                    string scrapeData = RunScrape(sDetails);
                    //jį sukapojam į dalis, kad gyvent lengviau būtų
                    var data = new List<FSpecs> { };
                    if (store == "Topocentras")
                        data = RipStringApart(scrapeData, sDetails);
                    else if (store == "Avitela")
                        data = RipStringApartAvitela(scrapeData, sDetails);
                    ViewBag.Specs = data;

                }
                else
                {
                    //List<FSpecs> specsReturn = new List<FSpecs> { };

                    var searchData = DB.GetList<Searches>(x => x.Search.Equals(sDetails.search_term));
                    var test = searchData.SingleOrDefault<Searches>(x => x.Store.Equals(store));
                    var data = DB.GetList<Phone>(x => x.SearchId.Equals(test.Id)).ToList();

                    foreach (var ph in data)
                    {
                        List<string> linkai = ph.ImageLinks.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        List<string> Linkai1 = linkai.Select(x => x.Replace("https://picfit.topocentras.lt/cdn-cgi/image/fit=contain,format=auto,height=60,width=60,quality=80/", "")).ToList();
                        Linkai1 = Linkai1.Select(x => x.Replace("media/catalog/", "https://www.topocentras.lt/media/catalog/")).ToList();
                        FSpecs temp = new FSpecs { };
                        temp.links = Linkai1;
                        temp.phone = ph;
                        specsReturn.Add(temp);
                    }
                    ViewBag.Specs = specsReturn;
                }
                if (sortOrder.Contains("Descending"))
                {
                    var test = specsReturn.OrderByDescending(x => x.phone.CurrentPrice).ToList();
                    ViewBag.Specs = test;
                }
                else if(sortOrder.Contains("Ascending"))
                {
                    var test = specsReturn.OrderBy(x => x.phone.CurrentPrice).ToList();
                    ViewBag.Specs = test;
                }
            }
            return View("Main_Scrape", sDetails);
        }
        private List<FSpecs> RipStringApart(string data, Search_Details sData)
        {
            List<string> skirtTelefonai = data.Split(new string[] { "EndOfQuery" }, StringSplitOptions.None).ToList();
            skirtTelefonai.RemoveAt(skirtTelefonai.Count - 1);
            List<FSpecs> specsReturn = new List<FSpecs> { };
            var searchUpdates = DB.GetList<Searches>(x => x.Search.Equals(sData.search_term));
            var searchUpdate = searchUpdates.SingleOrDefault(x => x.Store.Equals(sData.search_store.ToString()));
            foreach (string sTelef in skirtTelefonai)
            {
                FSpecs temp = new FSpecs { };
                int pFrom = sTelef.IndexOf("Query: ") + "Query: ".Length;
                int pTo = sTelef.LastIndexOf("Query close");
                string difQueries = sTelef.Substring(pFrom, pTo - pFrom);

                int priceFrom = sTelef.IndexOf("ItemPrice") + "ItemPrice".Length;
                int priceTo = sTelef.LastIndexOf("EndOfItemPrice");
                string PriceQueries = sTelef.Substring(priceFrom, priceTo - priceFrom);
                List<string> Kainos = PriceQueries.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                int linkFrom = sTelef.IndexOf("LinkToItem") + "LinkToItem".Length;
                int linkTo = sTelef.LastIndexOf("EndOfLinkToItem");
                string linkToItem = sTelef.Substring(linkFrom, linkTo - linkFrom).Replace("\r\n", "");

                int sFrom = sTelef.IndexOf("Info Start") + "Info Start".Length;
                int sTo = sTelef.LastIndexOf("Info Close");
                string specsd = sTelef.Substring(sFrom, sTo - sFrom);
                List<string> specsai = specsd.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                specsai.AddRange(Kainos);
                Phone specs = ConvToClass(specsai.ToList());
                
                int lFrom = sTelef.IndexOf("links: ") + "links: ".Length;
                int lTo = sTelef.LastIndexOf("links close");
                string links = sTelef.Substring(lFrom, lTo - lFrom);

                List<string> linkai = links.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> Linkai1 = linkai.Select(x => x.Replace("https://picfit.topocentras.lt/cdn-cgi/image/fit=contain,format=auto,height=60,width=60,quality=80/", "")).ToList();
                Linkai1 = Linkai1.Select(x => x.Replace("media/catalog/", "https://www.topocentras.lt/media/catalog/")).ToList();

                temp.links = Linkai1;
                temp.phone = specs;

                var entry = DB.Get<Phone>(x => x.PrekesKodas.Equals(specs.PrekesKodas));

                specs.ImageLinks = links;
                specs.LinkToItem = linkToItem;
                specs.SearchId = searchUpdate.Id;

                if (entry == null && specs.PrekesKodas != "")
                {
                    DB.Save(specs);
                    specsReturn.Add(temp);
                }
                else if(specs.PrekesKodas != "" && entry != null)
                {
                    specs.Id = entry.Id;
                    DB.Update(specs);
                    specsReturn.Add(temp);
                }
                
            }
            return (specsReturn);
        }
        private List<FSpecs> RipStringApartAvitela(string data, Search_Details sData)
        {
            List<string> skirtTelefonai = data.Split(new string[] { "EndOfQuery" }, StringSplitOptions.None).ToList();
            skirtTelefonai.RemoveAt(skirtTelefonai.Count - 1);
            List<FSpecs> specsReturn = new List<FSpecs> { };
            var searchUpdates = DB.GetList<Searches>(x => x.Search.Equals(sData.search_term));
            var searchUpdate = searchUpdates.SingleOrDefault(x => x.Store.Equals(sData.search_store.ToString()));
            foreach (string sTelef in skirtTelefonai)
            {
                FSpecs temp = new FSpecs { };
                int pFrom = sTelef.IndexOf("Query: ") + "Query: ".Length;
                int pTo = sTelef.LastIndexOf("Query close");
                string difQueries = sTelef.Substring(pFrom, pTo - pFrom);

                int priceFrom = sTelef.IndexOf("ItemPrice") + "ItemPrice".Length;
                int priceTo = sTelef.LastIndexOf("EndOfItemPrice");
                string PriceQueries = sTelef.Substring(priceFrom, priceTo - priceFrom);
                List<string> Kainos = PriceQueries.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                int linkFrom = sTelef.IndexOf("LinkToItem") + "LinkToItem".Length;
                int linkTo = sTelef.LastIndexOf("EndOfLinkToItem");
                string linkToItem = sTelef.Substring(linkFrom, linkTo - linkFrom).Replace("\r\n", "");

                int sFrom = sTelef.IndexOf("Info Start") + "Info Start".Length;
                int sTo = sTelef.LastIndexOf("Info Close");
                string specsd = sTelef.Substring(sFrom, sTo - sFrom);
                List<string> specsai = specsd.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                specsai.AddRange(Kainos);
                Phone specs = ConvToClassAvitela(specsai.ToList());

                int lFrom = sTelef.IndexOf("links: ") + "links: ".Length;
                int lTo = sTelef.LastIndexOf("links close");
                string links = sTelef.Substring(lFrom, lTo - lFrom);

                List<string> linkai = links.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> Linkai1 = linkai.Select(x => x.Replace("https://picfit.topocentras.lt/cdn-cgi/image/fit=contain,format=auto,height=60,width=60,quality=80/", "")).ToList();
                Linkai1 = Linkai1.Select(x => x.Replace("media/catalog/", "https://www.topocentras.lt/media/catalog/")).ToList();

                temp.links = Linkai1;
                temp.phone = specs;

                var entry = DB.Get<Phone>(x => x.PrekesKodas.Equals(specs.PrekesKodas));

                specs.ImageLinks = links;
                specs.LinkToItem = linkToItem;
                specs.SearchId = searchUpdate.Id;

                if (entry == null && specs.PrekesKodas != null)
                {
                    DB.Save(specs);
                    specsReturn.Add(temp);
                }
                else if (specs.PrekesKodas != null && entry != null)
                {
                    specs.Id = entry.Id;
                    DB.Update(specs);
                    specsReturn.Add(temp);
                }

            }
            return (specsReturn);
        }
        [Authorize]
        private Phone ConvToClass(List<string> specs)
        {
            Phone phone = new Phone { };
            foreach(string spec in specs)
            {
                string[] temp = spec.Split(new string[] { " : " }, StringSplitOptions.None);

                switch (temp[0])
                {
                    case string v when temp[0].Contains("Prekės kodas"):
                        phone.PrekesKodas = temp[1];
                        break;
                    case string v when temp[0].Contains("Barkodas"):
                        phone.Barkodas = Int64.Parse(temp[1]);
                        break;
                    case string v when temp[0].Contains("Gamintojo kodas"):
                        phone.GamintojoKodas = temp[1];
                        break;
                    case string v when temp[0].Contains("Garantija"):
                        phone.Garantija = Int32.Parse(temp[1]);
                        break;
                    case string v when temp[0].Contains("Gamintojas"):
                        phone.Gamintojas = temp[1];
                        break;
                    case string v when temp[0].Contains("Korpuso medžiagos"):
                        phone.KorpusoMedziagos = temp[1];
                        break;
                    case string v when temp[0].Contains("Svoris"):
                        phone.Svoris = float.Parse(temp[1]);
                        break;
                    case string v when temp[0].Contains("Papildomos funkcijos"):
                        phone.PapildomosFunkcijos = temp[1];
                        break;
                    case string v when temp[0].Contains("Komplektacija"):
                        phone.Komplektacija = temp[1];
                        break;
                    case string v when temp[0].Contains("SIM jungtis"):
                        phone.SimJungtis = temp[1];
                        break;
                    case string v when temp[0].Contains("Ekrano įstrižainė"):
                        phone.EkranoIstrizaine = temp[1];
                        break;
                    case string v when temp[0].Contains("Ekrano raiška"):
                        phone.EkranoRaiska = temp[1];
                        break;
                    case string v when temp[0].Contains("Galinė fotokamera"):
                        phone.GalineKamera = temp[1];
                        break;
                    case string v when temp[0].Contains("Priekinė fotokamera"):
                        phone.PriekineKamera = temp[1];
                        break;
                    case string v when temp[0].Contains("Autofokusavimas"):
                        phone.Autofokusavimas = temp[1];
                        break;
                    case string v when temp[0].Contains("Baterijos talpa"):
                        phone.BaterijosTalpa = int.Parse(temp[1]);
                        break;
                    case string v when temp[0].Contains("Pokalbio laikas"):
                        phone.PokalbioLaikas = int.Parse(temp[1]);
                        break;
                    case string v when temp[0].Contains("Vidinė atmintis"):
                        phone.VidAtmintis = temp[1];
                        break;
                    case string v when temp[0].Contains("Navigacija"):
                        phone.Navigacija = temp[1];
                        break;
                    case string v when temp[0].Contains("Operacinės sistemos"):
                        phone.OS = temp[1];
                        break;
                    case string v when temp[0].Contains("Branduoliai"):
                        phone.Branduoliai = temp[1];
                        break;
                    case string v when temp[0].Contains("Taktinis dažnis"):
                        phone.TaktinisDaznis = temp[1];
                        break;
                    case string v when temp[0].Contains("OldPrice"):
                        phone.OldPrice = float.Parse(temp[1].Replace(" €", "").Replace(" ", "").Replace(",", "."));
                        break;
                    case string v when temp[0].Contains("CurPrice"):
                        phone.CurrentPrice = float.Parse(temp[1].Replace(" €", "").Replace(" ", "").Replace(",", "."));
                        break;
                }
            }
            return phone;
        }
        [Authorize]
        private Phone ConvToClassAvitela(List<string> specs)
        {
            Phone phone = new Phone { };
            foreach (string spec in specs)
            {
                string[] temp = spec.Split(new string[] { " : " }, StringSplitOptions.None);

                switch (temp[0])
                {
                    case string v when temp[0].Contains("Prekės kodas"):
                        phone.PrekesKodas = temp[0].Replace("Prekės kodas", "");
                        break;
                    case string v when temp[0].Contains("Barkodas"):
                        phone.Barkodas = Int64.Parse(temp[0].Replace("Barkodas", ""));
                        break;
                    case string v when temp[0].Contains("Gamintojo kodas"):
                        phone.GamintojoKodas = temp[0].Replace("Gamintojo kodas", "");
                        break;
                    case string v when temp[0].Contains("Garantija"):
                        phone.Garantija = Int32.Parse(temp[0].Replace("Garantija", ""));
                        break;
                    case string v when temp[0].Contains("Gamintojas"):
                        phone.Gamintojas = temp[0].Replace("Gamintojas", "");
                        break;
                    case string v when temp[0].Contains("Korpuso medžiagos"):
                        phone.KorpusoMedziagos = temp[0].Replace("Korpuso Medžiagos", "");
                        break;
                    case string v when temp[0].Contains("Svoris"):
                        phone.Svoris = float.Parse(temp[0].Replace("Svoris", "").Replace("g",""));
                        break;
                    case string v when temp[0].Contains("Papildomos funkcijos"):
                        phone.PapildomosFunkcijos = temp[0].Replace("Papildomos funkcijos", "");
                        break;
                    case string v when temp[0].Contains("Komplektacija"):
                        phone.Komplektacija = temp[0].Replace("Komplektacija", "");
                        break;
                    case string v when temp[0].Contains("SIM jungtis"):
                        phone.SimJungtis = temp[0].Replace("SIM Jungtis", "");
                        break;
                    case string v when temp[0].Contains("Ekrano įstrižainė"):
                        phone.EkranoIstrizaine = temp[0].Replace("Ekrano įstrižainė", "");
                        break;
                    case string v when temp[0].Contains("Ekrano raiška"):
                        phone.EkranoRaiska = temp[0].Replace("Ekrano raiška", "");
                        break;
                    case string v when temp[0].Contains("Galinė fotokamera"):
                        phone.GalineKamera = temp[0].Replace("Galinė fotokamera", "");
                        break;
                    case string v when temp[0].Contains("Priekinė fotokamera"):
                        phone.PriekineKamera = temp[0].Replace("Priekinė fotokamera", "");
                        break;
                    case string v when temp[0].Contains("Autofokusavimas"):
                        phone.Autofokusavimas = temp[0].Replace("Autofokusavimas", "");
                        break;
                    case string v when temp[0].Contains("Baterijos talpa"):
                        phone.BaterijosTalpa = int.Parse(temp[0].Replace("Baterijos talpa", ""));
                        break;
                    case string v when temp[0].Contains("Pokalbio laikas"):
                        phone.PokalbioLaikas = int.Parse(temp[0].Replace("Pokalbio laikas", ""));
                        break;
                    case string v when temp[0].Contains("Vidinė atmintis"):
                        phone.VidAtmintis = temp[0].Replace("Vidinė atmintis", "");
                        break;
                    case string v when temp[0].Contains("Navigacija"):
                        phone.Navigacija = temp[0].Replace("Navigacija", "");
                        break;
                    case string v when temp[0].Contains("Operacinės sistemos"):
                        phone.OS = temp[0].Replace("Operacinės sistemos", "");
                        break;
                    case string v when temp[0].Contains("Branduoliai"):
                        phone.Branduoliai = temp[0].Replace("Branduoliai", "");
                        break;
                    case string v when temp[0].Contains("Taktinis dažnis"):
                        phone.TaktinisDaznis = temp[0].Replace("Taktinis dažnis", "");
                        break;
                    case string v when temp[0].Contains("OldPrice"):
                        phone.OldPrice = float.Parse(temp[1].Replace(" €", "").Replace(" ", "").Replace(",", "."));
                        break;
                    case string v when temp[0].Contains("CurPrice"):
                        phone.CurrentPrice = float.Parse(temp[1].Replace(" €", "").Replace(" ", "").Replace(",", "."));
                        break;
                }
            }
            return phone;
        }
        [Authorize]
        void proc_DataReceived(object sender, DataReceivedEventArgs e)
        {
            var ss = e.Data;
        }
        [Authorize]
        public string RunScrape(Search_Details sDetails)
        {
                //ProcessStartInfo start = new ProcessStartInfo();
                string cmd = HttpRuntime.AppDomainAppPath + "scrape\\google_img.py";
                string args = " \"" + sDetails.search_term + "\" 3 \"" + sDetails.search_store + "\"";

                // Argumentus suklijuojam
                string combArg = string.Format("{0}{1}", cmd, args);
                var proc = new Process();
                proc.StartInfo.FileName = "python";
                proc.StartInfo.Arguments = combArg;

                proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.EnableRaisingEvents = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                // startuojam savo python skriptą ir pasiimam visą console lango outputą.

                using (Process process = Process.Start(proc.StartInfo))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {

                        string result = reader.ReadToEnd();
                        Searches searchas = new Searches { };
                        searchas.Search = sDetails.search_term;
                        //searchas.Username = User.Identity.Name;
                        searchas.Data = DateTime.Now;
                        searchas.Store = sDetails.search_store.ToString();
                        var searchDatas = DB.GetList<Searches>(x => x.Search.Equals(sDetails.search_term));
                        var searchData = searchDatas.SingleOrDefault(x => x.Store.Equals(searchas.Store));
                        if (searchData == null)
                        {
                            DB.Save(searchas);
                        }else
                        {
                            searchas.Id = searchData.Id;
                            DB.Update(searchas);
                        }

                        return result;
                    }
                }
            }


    }
}
