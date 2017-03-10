using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;


namespace WebScraper
{
    class Program
    {
        public static List<string> rewrite_list = new List<string> { };

        static void Main(string[] args)
        {
            Console.WriteLine("Please type the page url.");
            Console.Write("> ");
            string url = Console.ReadLine();

            Console.Clear();

            Console.WriteLine("Please choose a location in which you wish to save the file(s).");
            Console.Write("> ");
            string location = Console.ReadLine();

            Console.Clear();

            Console.WriteLine("Do you want to start scraping? [Y/N]");
            Console.Write("> ");
            string begin = Console.ReadLine();
            begin = begin.ToLower();

            if (begin == "y")
            {
                downloadPageSource(url, location);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public static void downloadPageSource(string page, string location)
        {
            string htmlCode = "";

            using (WebClient client = new WebClient())
            {
                /*File.WriteAllText(@location + "file0.html", "Stuffs");
                client.DownloadFile(@page, @location + "file0.html");*/

                htmlCode = client.DownloadString(@page);

                File.WriteAllText(@location + "source.txt", htmlCode);
            }

            string rewrite = "";
            List<string> subPages = new List<string> { };

            while (htmlCode.IndexOf("http://") != -1)
            {
                int urlStart = htmlCode.IndexOf("http://");
                if (htmlCode[urlStart - 1] == '\"')
                {
                    int urlEnd = htmlCode.Remove(0, htmlCode.IndexOf("http://")).IndexOf("\"");
                    subPages.Add(htmlCode.Substring(urlStart, urlEnd));
                    Console.WriteLine(htmlCode.Substring(urlStart, urlEnd));
                    rewrite += htmlCode.Substring(urlStart, urlEnd) + System.Environment.NewLine;
                    htmlCode = htmlCode.Remove(urlStart, urlEnd + 1);
                }
                else if (htmlCode[urlStart - 1] == '\'')
                {
                    int urlEnd = htmlCode.Remove(0, htmlCode.IndexOf("http://")).IndexOf("\'");
                    subPages.Add(htmlCode.Substring(urlStart, urlEnd));
                    Console.WriteLine(htmlCode.Substring(urlStart, urlEnd));
                    rewrite += htmlCode.Substring(urlStart, urlEnd) + System.Environment.NewLine;
                    htmlCode = htmlCode.Remove(urlStart, urlEnd + 1);
                }
                else if (htmlCode[urlStart - 1] == ' ')
                {
                    int urlEnd = htmlCode.Remove(0, htmlCode.IndexOf("http://")).IndexOf("\"");
                    subPages.Add(htmlCode.Substring(urlStart, urlEnd));
                    Console.WriteLine(htmlCode.Substring(urlStart, urlEnd));
                    rewrite += htmlCode.Substring(urlStart, urlEnd) + System.Environment.NewLine;
                    htmlCode = htmlCode.Remove(urlStart, urlEnd + 1);
                }
                else
                {
                    //int urlEnd = htmlCode.Remove(0, htmlCode.IndexOf("https://")).IndexOf("\"");
                    Console.WriteLine("SKIPPED ITEM AT " + urlStart);
                    //rewrite += htmlCode.Substring(urlStart, urlEnd) + System.Environment.NewLine;
                    htmlCode = htmlCode.Remove(urlStart, 1);
                }

            }
            while (htmlCode.IndexOf("https://") != -1)
            {
                int urlStart = htmlCode.IndexOf("https://");
                if (htmlCode[urlStart - 1] == '\"')
                {
                    int urlEnd = htmlCode.Remove(0, htmlCode.IndexOf("https://")).IndexOf("\"");
                    subPages.Add(htmlCode.Substring(urlStart, urlEnd));
                    Console.WriteLine(htmlCode.Substring(urlStart, urlEnd));
                    rewrite += htmlCode.Substring(urlStart, urlEnd) + System.Environment.NewLine;
                    htmlCode = htmlCode.Remove(urlStart, urlEnd + 1);
                }
                else if (htmlCode[urlStart - 1] == '\'')
                {
                    int urlEnd = htmlCode.Remove(0, htmlCode.IndexOf("https://")).IndexOf("\'");
                    subPages.Add(htmlCode.Substring(urlStart, urlEnd));
                    Console.WriteLine(htmlCode.Substring(urlStart, urlEnd));
                    rewrite += htmlCode.Substring(urlStart, urlEnd) + System.Environment.NewLine;
                    htmlCode = htmlCode.Remove(urlStart, urlEnd + 1);
                }
                else if (htmlCode[urlStart - 1] == ' ')
                {
                    int urlEnd = htmlCode.Remove(0, htmlCode.IndexOf("https://")).IndexOf("\"");
                    subPages.Add(htmlCode.Substring(urlStart, urlEnd));
                    Console.WriteLine(htmlCode.Substring(urlStart, urlEnd));
                    rewrite += htmlCode.Substring(urlStart, urlEnd) + System.Environment.NewLine;
                    htmlCode = htmlCode.Remove(urlStart, urlEnd + 1);
                } else
                {
                    //int urlEnd = htmlCode.Remove(0, htmlCode.IndexOf("https://")).IndexOf("\"");
                    Console.WriteLine("SKIPPED ITEM AT " + urlStart);
                    //rewrite += htmlCode.Substring(urlStart, urlEnd) + System.Environment.NewLine;
                    htmlCode = htmlCode.Remove(urlStart, 1);
                }
                //Console.WriteLine("NEXT");
            }

            Console.WriteLine("DONE");

            rewrite = removeDuplicates(rewrite);

            File.WriteAllText(@location + "subs.txt", rewrite);

            // Download files
            using (WebClient client = new WebClient())
            {
                int id = 0;

                Console.WriteLine("Processing downloadable files.");

                foreach (string url in rewrite_list)
                {
                    string newUrl = url;
                    if(newUrl.IndexOf(@"\n") == 0) {
                        newUrl = newUrl.Remove(0, 2);
                    }
                    if (newUrl.LastIndexOf(@"\n") == newUrl.Length-2)
                    {
                        newUrl = newUrl.Remove(newUrl.Length - 2, 2);
                    }

                    Uri uriResult;
                    bool result = Uri.TryCreate(newUrl, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    if (result)
                    {
                        if (newUrl.LastIndexOf(".png") != -1)
                        {
                            client.DownloadFile(newUrl, @location + "file" + id + ".png");
                            id += 1;
                        }
                        else if (newUrl.LastIndexOf(".jpg") != -1)
                        {
                            client.DownloadFile(newUrl, @location + "file" + id + ".jpg");
                            id += 1;
                        }
                        else if (newUrl.LastIndexOf(".json") != -1)
                        {
                            Console.WriteLine("SKIPPED JSON FILE.");
                        }
                        else if (newUrl.LastIndexOf(".js") != -1)
                        {
                            client.DownloadFile(newUrl, @location + "file" + id + ".js");
                            id += 1;
                        }
                        else if (newUrl.LastIndexOf(".css") != -1)
                        {
                            client.DownloadFile(newUrl, @location + "file" + id + ".css");
                            id += 1;
                        }
                        else if (newUrl.LastIndexOf(".txt") != -1)
                        {
                            client.DownloadFile(newUrl, @location + "file" + id + ".txt");
                            id += 1;
                        }
                        else if (newUrl.LastIndexOf(".ini") != -1)
                        {
                            client.DownloadFile(newUrl, @location + "file" + id + ".ini");
                            id += 1;
                        }
                        else if (newUrl.LastIndexOf(".cp") != -1)
                        {
                            client.DownloadFile(newUrl, @location + "file" + id + ".cp");
                            id += 1;
                        }
                        else if (newUrl.LastIndexOf(".gifv") != -1)
                        {
                            client.DownloadFile(newUrl, @location + "file" + id + ".gifv");
                            id += 1;
                        }
                        else if (newUrl.LastIndexOf(".gif") != -1)
                        {
                            client.DownloadFile(newUrl, @location + "file" + id + ".gif");
                            id += 1;
                        }
                        else if (newUrl.LastIndexOf(".svg") != -1)
                        {
                            client.DownloadFile(newUrl, @location + "file" + id + ".svg");
                            id += 1;
                        }
                        else if (newUrl.LastIndexOf(".swf") != -1)
                        {
                            client.DownloadFile(newUrl, @location + "file" + id + ".swf");
                            id += 1;
                        }
                    }

                    Console.WriteLine(url);
                }
            }
        }

        public static string removeDuplicates(string String)
        {
            string write = String;
            string rewrite = "";
            bool used = false;

            Console.WriteLine("START!");

            while (write.IndexOf(Environment.NewLine) != -1)
            {
                Console.WriteLine("OUT!");
                foreach (string re in rewrite_list)
                {
                    Console.WriteLine(re.ToString());
                    if (rewrite_list.Contains(write.Substring(0, write.IndexOf(Environment.NewLine))))
                    {
                        used = true;
                        break;
                    }
                }

                if (used == false)
                {
                    rewrite_list.Add(write.Substring(0, write.IndexOf(Environment.NewLine)));
                }
                write = write.Remove(0, write.IndexOf(Environment.NewLine) +1);
                used = false;
            }

            foreach (string re in rewrite_list)
            {
                rewrite += re + "@@@";
                Console.WriteLine(re.ToString());
            }
            
            rewrite = rewrite.Replace("@@@", System.Environment.NewLine);

            return rewrite;
        }
    }
}