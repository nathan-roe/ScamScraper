using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScamScraper.Models;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

// For HashSet
namespace ScamScraper.Controllers
{
    public class HomeController : Controller
    {
        //-----VARIABLES-----
        private MyContext _context;
        private int count; //🖖 count edited to be global
        // I respect the emoji
        private int ExternalLinksCount;
        private List<string> ExternalLinksFromURL = new List<string>();


        public HomeController(MyContext context)
        {   
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.HasScript = HttpContext.Session.GetString("script");
            ViewBag.HasRickRoll = HttpContext.Session.GetString("rickroll");
            ViewBag.HasIFrame = HttpContext.Session.GetString("IFrame");
            ViewBag.HasError = HttpContext.Session.GetString("error");
            ViewBag.HasUrlScript = HttpContext.Session.GetString("UrlScript");
            ViewBag.HasUrlIFrame = HttpContext.Session.GetString("UrlIFrame");
            // ViewBag.ScreenShot = "wwwroot/Images/screenshot.png";
            // ViewBag.AHHH = HttpContext.Session.GetString("AHHH");
            ViewBag.Url = HttpContext.Session.GetString("Url");
            ViewBag.Score = HttpContext.Session.GetInt32("Score");
            if(HttpContext.Session.GetInt32("Score") <= 1)
            {
                HttpContext.Session.SetString("Response", "This link is safe!");
            }
            if(HttpContext.Session.GetInt32("Score") > 1 && HttpContext.Session.GetInt32("Score") <= 5)
            {
                HttpContext.Session.SetString("Response", "This link is suspicious");
            }
            if(HttpContext.Session.GetInt32("Score") > 5)
            {
                HttpContext.Session.SetString("Response", "This link is not safe!");
            }
            ViewBag.Response = HttpContext.Session.GetString("Response");
            
            ViewBag.ExternalLinksCount = HttpContext.Session.GetInt32("ExternalLink");
            return View("Index");
        }

        [HttpPost("checkUrl")]
        public IActionResult CheckUrl(string url)
        {
            HttpContext.Session.Clear();
            System.Console.WriteLine(url);
            ExternalLinksFromURL.Clear();
            // System.Console.WriteLine("This is the return of WebDataScrape");
            // System.Console.WriteLine(WebDataScrape(url));
            // ScreenShot(url);
            HttpContext.Session.SetString("Url", url);
            WebDataScrape(url);
            // HttpContext.Session.GetString("AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
            return RedirectToAction("Index");
        }

        [HttpPost("reset")]
        public IActionResult Reset()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        //-----------METHODS --------------
        private void LinkCountSeverity() //ADDED LINK COUNT SEVERITY METHOD
        {

            if(ExternalLinksCount==0)
            {
                System.Console.WriteLine("Either there were no links in this page or this method was accessed incorrectly.");
                HttpContext.Session.SetInt32("LinkCountSeveritySession", ExternalLinksCount);
            }
            else if(ExternalLinksCount <= 7)
            {
                System.Console.WriteLine("Links are 7 or less. You gucci");
                HttpContext.Session.SetInt32("LinkCountSeveritySession", ExternalLinksCount);
            }
            else if(ExternalLinksCount <= 30)
            {
                System.Console.WriteLine("Links are 30 or less.");
                HttpContext.Session.SetInt32("LinkCountSeveritySession", ExternalLinksCount);
            }
            else if(ExternalLinksCount <= 50)
            {
                System.Console.WriteLine("Links are 50 or less.");
                HttpContext.Session.SetInt32("LinkCountSeveritySession", ExternalLinksCount);
            }
            else
            {
                System.Console.WriteLine("There are over 50 links.");
                HttpContext.Session.SetInt32("LinkCountSeveritySession", ExternalLinksCount);
            }

        }//end calculateseverity
    //-----------------------------------------------------------------------------------------------
    // public void ScreenShot(string url)
    // {
    //     IWebDriver driver = new ChromeDriver();
    //     driver.Navigate().GoToUrl(url);
    //     Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();   
    //     screenshot.SaveAsFile("wwwroot/Images/screenshot.png", OpenQA.Selenium.ScreenshotImageFormat.Png);
    // }
    public int WebDataScrape(string url) //Edited REMOVED WORD static
    {
        if(url.Contains("%3Cscript%3E"))
        {
            HttpContext.Session.SetString("UrlScript", "This link could be malicious");
            // HttpContext.Session.SetString("Response", "This link may be malicious.");
            System.Console.WriteLine("This link could be malicious");
            count += 5;
        }
        if(url.Contains("<script>"))
        {
            count+=5;
        }
        if(url.Contains("%3Ciframe%3E"))
        {
            System.Console.WriteLine("This link could be malicious");
            HttpContext.Session.SetString("UrlIFrame", "This link could be malicious");
            // HttpContext.Session.SetString("Response", "This link may be malicious.");
            count += 3;
        }
        if(url.Contains("<iframe>"))
        {
            count += 3;
        }
        
        try
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            // Remove AdSense ads 
            // doc.DocumentNode.Descendants()
            // .Where(n => n.Name == "script" && n.Attributes.Contains("adsbygoogle"))
            // .ToList()
            // .ForEach(n => n.Remove());
            HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//a[@href]").ToArray();  
            HashSet<string> hrefList = new HashSet<string>();
            HashSet<string> ignoreList = new HashSet<string>();
            HashSet<string> rickList = new HashSet<string>();
            // int count = 0; // 🖖 changed int count to be within the scope of this class
            // count = 0;
            foreach (HtmlNode n in nodes)  
            {   
                // Gets the actual value of the link and sets it to href
                string href = n.Attributes["href"].Value;
                if(!ignoreList.Contains(href))
                {
                    string root = "";
                    string urlVal = "";
                    if(href.Contains("%3Cscript%3E"))
                    {
                        System.Console.WriteLine("This link contains JavaScript");
                        HttpContext.Session.SetString("script", "This page contains links that may be malicious");
                        // HttpContext.Session.SetString("Response", "This links' page may contain malicious links.");
                        
                    }
                    if(href.Contains("%3Ciframe%3E"))
                    {
                        System.Console.WriteLine("This link contains an IFrame");
                        HttpContext.Session.SetString("script", "This link contains an IFrame");
                        // HttpContext.Session.SetString("Response", "This link contains an IFrame");
                        
                    }
                    foreach(string rick in rickList)
                    {
                        if(href == rick)
                        {
                            System.Console.WriteLine("This is a rickroll");
                            HttpContext.Session.SetString("rickroll", "This links to a RickRoll");
                            // HttpContext.Session.SetString("Response", "This link is a RickRoll");
                            // count++;
                            
                        }
                    }
                        if(href.StartsWith("http"))
                        {
                            System.Console.WriteLine("This is a link to a different page");
                            ExternalLinksCount++; //count how many external links

                            urlVal = href;
                            System.Console.WriteLine("-----------------------URLVAL-----------------------");
                            System.Console.WriteLine($"{urlVal}\n\n\n");

                            ExternalLinksFromURL.Add(urlVal);
                            

                            
                        }
                        else if(href[0] == '/')
                        {
                            System.Console.WriteLine("This link is on the same page");
                            for(var c = 0; c < url.Length;c++)
                            {
                                if(url[c] == '/')
                                {
                                    count++;
                                }
                                if(count < 3)
                                {
                                    root = url;
                                }
                                else if(count == 3)
                                {
                                    root = url.Substring(0, c);
                                    break;
                                }
                            }
                            urlVal = root + href;
                        }  
                        else{
                            urlVal = href;
                        }
                        ignoreList.Add(urlVal);
                        // doc = web.Load(urlVal);
                        System.Console.WriteLine(urlVal);
                }
            } 
            // Console.WriteLine("\r\nPlease press a key...");
            // Console.ReadKey();
            count ++;
            // HttpContext.Session.SetString("Response", "This link is safe!");
            HttpContext.Session.SetInt32("Score", count);
            HttpContext.Session.SetInt32("ExternalLink", ExternalLinksCount);
            Console.WriteLine(count);
            return count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured:\r\n{ex.Message}");  
            HttpContext.Session.SetString("error", "I'm sorry, something went wrong. Please re-enter the url or try another one");
            // HttpContext.Session.SetString("Response", "An error has occured, please try again");
            return -1;

        }
    }



        //THIS FUNCTION RETURNS A LIST OF EXTERNAL URLS THAT HAVE ENCODINGS
        private List<string> CheckExternalLinks(List<string> urlValues)
        {
            List<string> URLsCouldBeBad = new List<string>();
            List<string> AllflaggedKeywords = _context.FlaggedKeywords.Select(f => f.FlaggedKeywordWord).ToList();
            
            foreach(var urlVal in urlValues)
            {
                
                if(AllflaggedKeywords.Any(f => urlVal.Contains(f)))
                {
                    // var theKeyword = AllflaggedKeywords.FirstOrDefault(f => urlVal.Contains(urlVal));
                    System.Console.WriteLine($"External links from URL contains some flagged keywords.");
                    URLsCouldBeBad.Add(urlVal);
                }
                else
                {
                    System.Console.WriteLine("This external link has no encoding");
                }
                
            }

            //ADDS ALL URLS IN LIST TO A STRING TO ADD TO SESSION
            string AllURLsCouldBeBad = "";
            foreach (var singleURL in URLsCouldBeBad)
            {
                AllURLsCouldBeBad += $">>>{singleURL}\n";
            }

            HttpContext.Session.SetString("UrlScript", $"External links include some flagged keywords. URL:{AllURLsCouldBeBad}");
            return URLsCouldBeBad;
        }


    }
}
