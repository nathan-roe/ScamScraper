ScamScraper is a website that allows users to imput a url, and then have that url rated on how potentially dangerous it is. Using an MVC pattern, the website has 
a comments page and an information page, as well as the main page. The algorithm is looking for JavaScript code within the url, which could contain malicious information, as well 
as potentially harmful links on the page of the link itself. To do this, it uses a web scraper that will parse through the HTML on the page on the other side of the url. 
This project uses C#, ASP.NET, Razor, MySQL and HTMLAgilityPack.
C#: Primary language used for logic and functionality
ASP.NET: Used to create the website and use the MVC format
Razor: Allows C# to be used within the .cshtml files
MySQL: Stores data on websites that have been detected as malicious, or deemed maicious previous to detection.
HTMLAgilityPack: Makes it possible to scrape through a website's HTML and gather any malicious code that the algorithms detect.
