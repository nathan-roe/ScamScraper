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
            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(url);

                HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//a[@href]").ToArray();  
                HashSet<string> hrefList = new HashSet<string>();
                HashSet<string> ignoreList = new HashSet<string>();
                foreach (HtmlNode n in nodes)  
                {  
                    // Gets the actual value of the link and sets it to href
                    string href = n.Attributes["href"].Value;
                    if(!ignoreList.Contains(href))
                    {
                        string root = "";
                        int count = 0;
                        string urlVal = "";
                        if(href.Contains("%3Cscript%3E"))
                        {
                            System.Console.WriteLine("This link contains JavaScript");
                        }
                        if(href.Substring(0, 4) == "http")
                        {
                            System.Console.WriteLine("This is a link to a different page");
                            urlVal = href;
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
                        ignoreList.Add(urlVal);
                        doc = web.Load(urlVal);
                        System.Console.WriteLine(urlVal);
                    }
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
