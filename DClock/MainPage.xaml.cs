namespace DClock;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		ClockTick();
	}

	//private void OnCounterClicked(object sender, EventArgs e)
	//{
	//	count++;

	//	if (count == 1)
	//		CounterBtn.Text = $"Clicked {count} time";
	//	else
	//		CounterBtn.Text = $"Clicked {count} times";

	//	SemanticScreenReader.Announce(CounterBtn.Text);
	//}
	private async void ClockTick()
	{
		MainClock.TextColor = Color.FromRgba("220000");
		while (true)
		{
			await Task.Run(async () =>
			{
				Dispatcher.Dispatch(() => MainClock.Text = DateTime.Now.ToString("hh:mm"));
				Thread.Sleep(1000);
			});
		}
	}
}

