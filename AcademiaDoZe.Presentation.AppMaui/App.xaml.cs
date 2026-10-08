// Autor: Iago Barboza Ceccatto
namespace AcademiaDoZe.Presentation.AppMaui;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell())
        {
            Title = "Academia do Zé - Iago Barboza Ceccatto",
            Width = 1280,
            Height = 820,
            MinimumWidth = 720,
            MinimumHeight = 560,
            TitleBar = new TitleBar
            {
                Title = "Academia do Zé",
                Subtitle = "Iago Barboza Ceccatto",
                Icon = "emblema.png",
                HeightRequest = 44
            }
        };
    }
}
