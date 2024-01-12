using System.Text;
using System.Xml;
using XmlDocument = System.Xml.XmlDocument;

namespace DClock;

public partial class MainPage : ContentPage
{
	Color textColor = Color.FromRgba("660011");
	string nwsUrl = "https://forecast.weather.gov/MapClick.php?lat=38.8756&lon=-77.4087&unit=0&lg=english&FcstType=dwml";

	public MainPage()
	{
		InitializeComponent();
		ClockTick();
	}

	private async void ClockTick()
	{
		MainClock.TextColor = textColor;
		Temperature.TextColor = textColor;
		CurrentDate.TextColor = textColor;
		while (true)
		{
			await Task.Run(async () =>
			{
				Dispatcher.Dispatch(() => CurrentDate.Text = DateTime.Now.ToString("ddd dd MMM"));
				Dispatcher.Dispatch(() => MainClock.Text = DateTime.Now.ToString("hh:mm"));
				if (DateTime.Now.ToString("mm").Substring(1).Equals("0")) 
				{
					//get temp every 10 minutes
					var t = await GetTemp();
					Dispatcher.Dispatch(() => Temperature.Text = $"{t}°");
				}
				Thread.Sleep(20000);
			});
		}
	}

	private async Task<string> GetTemp() 
	{
		string temperature = "--";
		try
		{
			HttpClient client = new HttpClient();
			//client.BaseAddress = new Uri(nwsUrl);
			client.DefaultRequestHeaders.Add("User-Agent", "LocalClockApp/v0.1 (http://perceptivepumpkin.com; derek@perceptivepumpkin.com)");
			var resp = await client.GetAsync(nwsUrl);
			//parse xml
			XmlDocument doc = new XmlDocument();
			Stream receiveStream = await resp.Content.ReadAsStreamAsync();
			StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
			var result = readStream.ReadToEnd();
			
			doc.LoadXml(result);
			var node = doc.DocumentElement.SelectSingleNode("//temperature/value")?.InnerText;
			temperature = node;
		}
		catch (Exception ex) 
		{
			Console.WriteLine(ex.Message);
		}
		return temperature;
	
	}
}