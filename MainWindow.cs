namespace WkcCommunicator;

/// <summary>
/// Window used by the app.
/// On Android the window draws edge to edge, so the status bar keeps whatever color the Android
/// theme resolves unless it is set explicitly; it is tinted with the app's primary color (see
/// MainWindow.Android.cs). The same color is re-applied from MauiProgram after the activity
/// resumes, so showing a popup or a dialog cannot knock the status bar off its color.
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow(Page page) : base(page)
	{
	}

	protected override void OnCreated()
	{
		base.OnCreated();
		ApplyStatusBarColor();
	}
}
