using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FrameTimeHandler.ViewModels;

namespace FrameTimeHandler.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.Deactivated += (sender, args) =>
            {
                var vm = this.DataContext as MainWindowViewModel;
                vm.IsFrameTimingGraphChangingColor = vm.IsProbabilityDensityChangingColor = vm.IsProbabilityDistributionGraphChangingColor = false;
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
