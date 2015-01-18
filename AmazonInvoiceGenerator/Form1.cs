using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net;
using System.IO;
namespace AmazonInvoiceGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
           richTextBox1.Clear();
           string longdate = e.End.ToLongDateString();
           string[] date = longdate.Split(',');
           richTextBox1.Text = date[1] + "," + date[2];
           monthCalendar1.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            monthCalendar1.Show();
           
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> ProductInfo = ScrapeLink(richTextBox4.Text);
            string Receipt = AmazonInvoiceGenerator.Properties.Resources.AmazonOrderTemplate;
            

            //user-defined values
            string OrderNumber = GenerateOrderNumber();
            string Date = richTextBox1.Text;
            string ProductName = ProductInfo[0];
            string Price = ProductInfo[1];
            string name = richTextBox3.Text;
            string Address = richTextBox2.Text;
            string City = richTextBox5.Text;
            string LastFourCC = Last4CC();
            //end user-defined values

            //Replace all the skeleton-values
            Receipt = Receipt.Replace("[DATE]", Date);
            Receipt = Receipt.Replace("[ORDERNUMBER]", OrderNumber);
            Receipt = Receipt.Replace("[PRODUCTNAME]", ProductName);
            Receipt = Receipt.Replace("[FULLNAME]", name);
            Receipt = Receipt.Replace("[STREETADDRESS]", Address);
            Receipt = Receipt.Replace("[CITYANDSTATE]", City);
            Receipt = Receipt.Replace("[LASTFOUR]", LastFourCC);
            Receipt = Receipt.Replace("[PRICE]", Price);

            //Write-Out to HTML
            using(FileStream stream = new FileStream("invoice.htm", FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.WriteLine(Receipt);
                }
            }
            MessageBox.Show("Success! Your invoice has been saved as 'invoice.htm' in the directory from which you ran this application.");
        }

        public string Last4CC()
        {
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            for(int i = 0; i < 4; i++)
            {
                sb.Append(rnd.Next(1, 10).ToString());
            }
            return sb.ToString();
        }
        public string GenerateOrderNumber()
        {
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            for(int i = 0; i < 19; i++)
            {
                if (i == 3)
                {
                    sb.Append("-");
                }
                else if (i == 11)
                {
                    sb.Append("-");
                }
                else
                {
                    sb.Append(rnd.Next(1, 10).ToString());
                }

               
            }
            return sb.ToString();
            
        }
        public List<string> ScrapeLink(string url)
        {
            List<string> ProductInfo = new List<string>();
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string AmazonHtml = wc.DownloadString(url);
                    HtmlAgilityPack.HtmlDocument amazon = new HtmlAgilityPack.HtmlDocument();
                    amazon.LoadHtml(AmazonHtml);
                    HtmlNode productname = amazon.GetElementbyId("btasintitle");
                    HtmlNode price = amazon.GetElementbyId("actualpricevalue");
                    ProductInfo.Add(productname.InnerText);
                    ProductInfo.Add(price.InnerText);
                }
                
            }
            catch(Exception e)
            {
                MessageBox.Show("An error occured-  " + e.ToString());
            }
            return ProductInfo;
        }

        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by t72, no warranties expressed or implied.\n ");
        }
    }
}
