using Microsoft.Extensions.Configuration;
using System.Text;
using System.Xml;
using XmlDocument = System.Xml.XmlDocument;

namespace DClock;

public partial class MainPage : ContentPage
{
    IConfiguration configuration;
	string nwsUrl = "";
    public MainPage(IConfiguration config)
	{
		InitializeComponent();
		configuration = config;
		ClockTick();
	}
    /// <summary>
    /// Runs a thread to call NWS API for temperature data on the tenth minute of every hour
    /// </summary>
    private async void ClockTick()
	{
		while (true)
		{
			await Task.Run(async () =>
			{
				Dispatcher.Dispatch(() => CurrentDate.Text = DateTime.Now.ToString("ddd dd MMM"));
				Dispatcher.Dispatch(() => MainClock.Text = DateTime.Now.ToString("hh:mm"));
				if (DateTime.Now.ToString("mm").Substring(1).Equals("0")) 
				{
					//get temp every 10 minutes
					Dispatcher.Dispatch(() => Temperature.Text = $"--°");
					var t = await GetTemp();
					Dispatcher.Dispatch(() => Temperature.Text = $"{t}°");
				}
				Thread.Sleep(20000);
			});
		}
	}

    /// <summary>
    /// Calls NWS API to get weather data and extracts temperature value
    /// </summary>
    private async Task<string> GetTemp() 
	{
		string temperature = "--";
        nwsUrl = configuration.GetValue<string>("nwsLocalWeatherURL");
		string userAgent = configuration.GetValue<string>("userAgentContactString"); 
        try
		{
			HttpClient client = new HttpClient();
			//set client information to your specific contact details, in case the National Weather Service needs to contact you
			client.DefaultRequestHeaders.Add("User-Agent", $"LocalClockApp/v0.1 ({userAgent})");
			var resp = await client.GetAsync(nwsUrl);
			//parse xml from NWS
			XmlDocument doc = new XmlDocument();
			Stream receiveStream = await resp.Content.ReadAsStreamAsync();
			StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
			var result = readStream.ReadToEnd();
			
			doc.LoadXml(result);
			var node = doc.DocumentElement.SelectNodes("//temperature[@type='apparent']/value")?[0].InnerText;
			temperature = node;
		}
		catch (Exception ex) 
		{
			Console.WriteLine(ex.Message);
		}
		return temperature;
	
	}
}