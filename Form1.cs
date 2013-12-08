using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net;
using System.IO;
namespace NedroidDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                DownloadComics(GetComicPageList(), folderBrowserDialog1.SelectedPath);
            }
        }


        public List<string> GetComicPageList()
        {
            string ArchivePage = @"http://nedroid.com/archive/";

            HtmlAgilityPack.HtmlWeb oHtmlWeb = new HtmlAgilityPack.HtmlWeb();

            HtmlAgilityPack.HtmlDocument oHtmlDocument = oHtmlWeb.Load(ArchivePage);

            List<string> links = ExtractAllAHrefTags(oHtmlDocument);
            links.RemoveRange(0, 9); //remove various other links
            links.RemoveRange(links.Count - 13, 13); //remove various other links
            return links;



            /*HtmlNode oRootNode = oHtmlDocument.DocumentNode;

            var div = oRootNode.ChildNodes[3].ChildNodes[3]; //SelectSingleNode("//div[@class='entry']");
            if (div != null)
            {
                var links = div.Descendants("a")
                               .Select(a => a.InnerText)
                               .ToList();
            }
            */
        }

        public void DownloadComics(List<string> links, string folder)
        {
            textBox1.AppendText("Links found: " + links.Count.ToString() + "\r\n");
            textBox1.AppendText("Unable to download links:\r\n");

            string filename, ComicImgUrl;
            WebClient webClient = new WebClient();
            HtmlAgilityPack.HtmlWeb oHtmlWeb = new HtmlAgilityPack.HtmlWeb();

            progressBar1.Maximum = links.Count;

            foreach (string link in links)
            {

                try
                {
                    HtmlAgilityPack.HtmlDocument ComicPage = oHtmlWeb.Load(link); // try-catch target
                    HtmlNode oRootNode = ComicPage.GetElementbyId("comic");
                    ComicImgUrl = oRootNode.ChildNodes[1].Attributes["src"].Value; //try-catch target
                    filename = folder + "\\" + Path.GetFileName(ComicImgUrl);
                    if (File.Exists(filename) == false)
                    {
                        webClient.DownloadFile(ComicImgUrl, filename);
                    }
                }
                catch
                {
                    textBox1.AppendText(link + "\r\n");
                }
                progressBar1.Value = links.IndexOf(link);
            }

            textBox1.AppendText("Download complete.");

        }

        public List<string> ExtractAllAHrefTags(HtmlAgilityPack.HtmlDocument htmlSnippet)
        {
            List<string> hrefTags = new List<string>();

            foreach (HtmlNode link in htmlSnippet.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                hrefTags.Add(att.Value);
            }

            return hrefTags;
        }
    }
}
