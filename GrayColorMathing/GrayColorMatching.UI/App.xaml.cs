using System.Windows;
using Castle.Windsor;
using GrayColorMatching.UI.Infrastructure;
using GrayColorMatching.UI.ViewModels;

namespace GrayColorMatching.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = new WindsorContainer();
            container.Install(new DependencyInstaller());

            var viewModel = container.Resolve<ColorMatchingViewModel>();
            var mainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            viewModel.HighlightChanged += mainWindow.OnHighlightChanged;

            mainWindow.Show();
        }
    }
}
