using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using GrayColorMatching.BL.Services;
using GrayColorMatching.UI.ViewModels;

namespace GrayColorMatching.UI.Infrastructure
{
    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IAppSettingsService>().ImplementedBy<AppSettingsService>());
            container.Register(Component.For<IColorMatchService>().ImplementedBy<ColorMatchService>());
            container.Register(Component.For<ColorMatchingViewModel>().ImplementedBy<ColorMatchingViewModel>());
        }
    }
}
