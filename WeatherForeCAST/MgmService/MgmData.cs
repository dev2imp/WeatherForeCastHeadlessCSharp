using System;
using System.Collections.Generic;
using System.Timers;
using System.Net;
using MySql.Data.MySqlClient;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
namespace WindowsService
{
	public class MgmData
	{
		private readonly Timer _timer;
		public MgmData()
		{
			_timer = new Timer(300000) { AutoReset=true};
			_timer.Elapsed += TimerOver;
		}
		public static void Getdata(string url,string province ,string district,string MysqlconectionsString , IWebDriver driver)
		{
			driver.Navigate().GoToUrl(url);//connect to the url
			string path = "C:/MgmService/downloaded/";
			var temperature = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[1]/div[1]"));
			// Console.WriteLine(temperature.Text);
			//5
			IWebElement fallTypeImg = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[1]/div[2]/div[1]/img"));
			string fallTypeImg_url;
			fallTypeImg_url = path + province + district + "fallTypeImg.svg";
			try
			{
				using (WebClient client = new WebClient())
				{
					client.DownloadFile(new Uri(fallTypeImg.GetAttribute("src")), fallTypeImg_url);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			//6
			var fallType_name = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[1]/div[2]/div[2]"));
		   // Console.WriteLine(fallType_name.Text);
			//7
			var fall = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[1]/div[3]/div[2]/div[1]"));
		   // Console.WriteLine(fall.Text);
			//8
			var fallValue = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[1]/div[3]/div[2]/div[2]"));
		   // Console.WriteLine(fallValue.Text);
			//9
			var humudity_name = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[2]/div[1]/div[2]/div[1]"));
			//Console.WriteLine(humudity_name.Text);
			//10
			var humudity = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[2]/div[1]/div[2]/div[2]"));
			// Console.WriteLine(humudity.Text);
			//12
			var wind_name = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[2]/div[2]/div[2]/div[1]"));
		   // Console.WriteLine(wind_name.Text);
			//13
			IWebElement windrot = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[2]/div[2]/div[1]/img"));
		  //  Console.WriteLine(windrot.GetAttribute("style"));
			string windrot_url;
			windrot_url = path + province + district + "windrotation.svg";
			try
			{
				using (WebClient windimg = new WebClient())
				{
					windimg.DownloadFile(new Uri(windrot.GetAttribute("src")), windrot_url);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			//14
			var windspeed = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[2]/div[2]/div[2]/div[2]"));
			// Console.WriteLine(windspeed.Text);
			var winrotdegree = driver.FindElement(By.XPath(" //*[@id=\"pages\"]/div/section/div[5]/div[2]/div[2]/div[1]/img")).GetAttribute("style");
			//  Console.WriteLine(winrotdegree);
			string tmp = string.Empty;
			int degree=0;
			for (int i = 0; i < winrotdegree.Length; i++)
			{
				if(Char.IsDigit(winrotdegree[i]))
				{
					tmp += tmp;
				}
			}
			if(tmp.Length>0)
			{
				degree = int.Parse(tmp);
			}
			//15
			var LowPressureName = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[2]/div[3]/div[2]/div[1]"));
			
		  //  Console.WriteLine(LowPressureName.Text);
			//16
			var LowPressure = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[2]/div[3]/div[2]/div[2]"));
		  //  Console.WriteLine(LowPressure.Text);
			//17
			var seaRedPressureName = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[2]/div[4]/div[2]/div[1]"));
		   // Console.WriteLine(seaRedPressureName.Text);
			//18
			var seaRedPressure = driver.FindElement(By.XPath("//*[@id=\"pages\"]/div/section/div[5]/div[2]/div[4]/div[2]/div[2]"));
			//   Console.WriteLine(seaRedPressure.Text);
			MySqlConnection databaseconnection = new MySqlConnection(MysqlconectionsString);
			string queryProv = "SELECT `Name` FROM `provinces`";//if I hve name of provnce I also have all cities
			//that is why only check if I have put this province to database
			var  prvns = driver.FindElement(By.Id("cmbIl")).GetAttribute("value");
			
			databaseconnection.Open();
			MySqlCommand commandDatabase = new MySqlCommand(queryProv, databaseconnection);
			MySqlDataReader myreader = commandDatabase.ExecuteReader();
			int checkflag = 1;
			if (myreader.HasRows)
			{
				while (myreader.Read())
				{
				   if(myreader.GetString(0)== prvns)
					{
						if(prvns!=null)
						{
							checkflag = 0;
						}
					}
				}
			}
			if(checkflag==1)
			{
				databaseconnection.Close();
				databaseconnection.Open();
				queryProv = "INSERT INTO `provinces`(`id`, `Name`) VALUES (NULL,'"+prvns+"')";
				commandDatabase = new MySqlCommand(queryProv, databaseconnection);
				commandDatabase.ExecuteReader();//add province
				databaseconnection.Close();
				databaseconnection.Open();
				var distrs = driver.FindElement(By.Id("cmbIlce"));///we gets distrct
			
				foreach (var ds in distrs.Text.Split('\n'))
				{
					string cleands = System.Text.RegularExpressions.Regex.Replace(ds, @"\s+", " ");
					if (cleands.Length>1)
					{
						queryProv = "INSERT INTO `districts`(`id`, `Nam`, `ProvName`) VALUES (NULL,'" + cleands + "','" + prvns + "')";
						commandDatabase = new MySqlCommand(queryProv, databaseconnection);
						myreader = commandDatabase.ExecuteReader();
						databaseconnection.Close();
						databaseconnection.Open();
						Console.WriteLine(cleands,ds);
					}
				}
			}
			string queryget = "SELECT `id` FROM `themoment`";
			string delquery = "DELETE FROM `themoment`";
			databaseconnection.Close();
			databaseconnection.Open();
			commandDatabase = new MySqlCommand(queryget, databaseconnection);
			myreader = commandDatabase.ExecuteReader();
			int id = 1;
			if (myreader.HasRows)
			{
				while(myreader.Read())
				{
					id++;
				}
			}
			databaseconnection.Close();
			databaseconnection.Open();
			if (id>14)
			{
				commandDatabase = new MySqlCommand(delquery, databaseconnection);
				commandDatabase.ExecuteReader();
				id = 1;
			}
			databaseconnection.Close();
			databaseconnection.Open();
			string query = "INSERT INTO `themoment`(`id`, `Province`, `district`, `temperature`, `fallTypeImg_url`, `fallType_name`, `fall`, `fallValue`, `humudity_name`, `humudity`, `wind_name`, `windrot_url`, `windspeed`,`windotationDegree`, `LowPressureName`, `LowPressure`, `seaRedPressureName`, `seaRedPressure`)" +
		   " VALUES ('" + id + "','" + province + "','" + district + "','" + temperature.Text + "','" + fallTypeImg_url + "','" + fallType_name.Text + "','" + fall.Text + "','" + fallValue.Text + "','" + humudity_name.Text + "','" + humudity.Text + "','" + wind_name.Text + "','" + windrot_url + "','" + windspeed.Text + "','"+ degree + "','" + LowPressureName.Text + "','" + LowPressure.Text + "','" + seaRedPressureName.Text + "','" + seaRedPressure.Text + "')";
			commandDatabase = new MySqlCommand(query, databaseconnection);
			commandDatabase.ExecuteReader();
			databaseconnection.Close();
		}
		private void TimerOver(object sender, ElapsedEventArgs e)
		{           
			//below we connect to database to get province and district.
			string query = "SELECT `Province` ,`district` FROM `last5city`";
			string MysqlconectionsString = "datasource=127.0.0.1;port=3306;username=root;password=;database=mgmweather";
			string url;
			MySqlConnection databaseconnection = new MySqlConnection(MysqlconectionsString); 
			MySqlCommand commandDatabase = new MySqlCommand(query, databaseconnection);
			try
				{
				ChromeOptions options = new ChromeOptions();
				options.AddArguments(new List<string>()
				{
				"--silent-launch",
			  "--no-startup-window",
			  "no-sandbox",
			  "headless"
				});
			   
				databaseconnection.Open();
				  MySqlDataReader myreader = commandDatabase.ExecuteReader();
				
				if (myreader.HasRows)
					{
						while (myreader.Read())
						{

						IWebDriver driver = new ChromeDriver(options);
						url = "https://www.mgm.gov.tr/tahmin/il-ve-ilceler.aspx?il=" + myreader.GetString(0) + "&ilce=" + myreader.GetString(1);
						
						
						Getdata(url, myreader.GetString(0), myreader.GetString(1), MysqlconectionsString,driver);
						driver.Quit();
						}
				   
					}
				
			}
			catch (Exception ex)
				{
					Console.WriteLine("-------------------"+ ex.Message);
				}
		}
		public void Start()
		{
			_timer.Start();
		}
		public void Stop()
		{
			_timer.Stop();
		}
	}
}
