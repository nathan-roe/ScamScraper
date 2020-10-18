using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
// For HashSet
using System.Collections.Generic;
 
namespace WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Input Url");
            string inputUrl = Console.ReadLine();
            WebDataScrape(inputUrl);
        }
 
        public static void WebDataScrape(string url)
        {
            string initialUrl = url;
            try
            {
                //Get the content of the URL from the Web
                // url = "http://3.138.153.76/";
                var web = new HtmlWeb();
                var doc = web.Load(url);

                HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//a[@href]").ToArray();  
                HashSet<string> hrefList = new HashSet<string>();
                HashSet<string> ignoreList = new HashSet<string>();
                // .Where(x=>x.InnerHtml.Contains("div2"))
                System.Console.WriteLine("Each href on this page is:");
                foreach(HtmlNode n in nodes)
                {
                    string href = n.Attributes["href"].Value;
                    System.Console.WriteLine($"{href}");
                }
                foreach (HtmlNode n in nodes)  
                {  
                    // sets href to one of the hrefs in nodes 
                    string href = n.Attributes["href"].Value;
                    System.Console.WriteLine($"{href}");
                    if(href != url && !ignoreList.Contains(href))
                    {
                        if(Char.IsLetter(href[0]))
                        {
                            System.Console.WriteLine("If");
                            System.Console.WriteLine("The url is:");
                            System.Console.WriteLine(href);
                            ignoreList.Add(href);
                            // WebDataScrape(href);
                            doc = web.Load(href);
                            var hasDownload = doc.DocumentNode.Descendants()
                                .Where(n => n.Attributes.Any(a => a.Value.Contains("div")));
                            if(hasDownload != null)
                            {
                                System.Console.WriteLine("This has a download link!!!!");
                            }
                            // nodes = doc.DocumentNode.SelectNodes("//a[@href]").ToArray();
                        }
                        else if(href[0] == '/')
                        {
                            System.Console.WriteLine("Else if");
                            var found = initialUrl.IndexOf(".com");
                            string root = initialUrl.Substring(0, found + 4);
                            System.Console.WriteLine("The root and url is:");
                            string urlVal = root + href;

                            System.Console.WriteLine(urlVal);
                            ignoreList.Add(urlVal);
                            // WebDataScrape(urlVal);
                            doc = web.Load(urlVal);
                            nodes = doc.DocumentNode.SelectNodes("//a[@href]").ToArray();
                            var hasDownload = doc.DocumentNode.Descendants()
                                .Where(n => n.Attributes.Any(a => a.Value.Contains("div")));
                            if(hasDownload != null)
                            {
                                System.Console.WriteLine("This has a download link!!!!");
                            }
                        }
                        // else
                        // {
                        //     System.Console.WriteLine("Else");
                        //     System.Console.WriteLine("The url is:");
                        //     System.Console.WriteLine(href);
                        //     ignoreList.Add(href);
                        //     WebDataScrape(href);
                        // }
                    }
                    
                    // hrefList.Add(href);
                } 
                Console.WriteLine("\r\nPlease press a key...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured:\r\n{ex.Message}");
            }
        }
    }
}
