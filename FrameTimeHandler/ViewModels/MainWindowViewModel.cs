using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

using ReactiveUI;
using DynamicData;

using FrameTimeHandler.FTAnlzerInterop;
using FrameTimeHandler.GraphsExport;

namespace FrameTimeHandler.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _filePath = "";

        public bool IsFileSelected => string.IsNullOrEmpty(_filePath) is false;

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    this.RaisePropertyChanged(nameof(FilePath));
                    this.RaisePropertyChanged(nameof(IsFileSelected));
                }
            }
        }

        public ReactiveCommand<Unit, Unit> SelectFileCommand { get; }
        public ReactiveCommand<Unit, Unit> UnselectFileCommand { get; }

        public ObservableCollection<string> LogCreationPrograms { get; set; } = new ObservableCollection<string>() { "none" };
        public string SelectedProgram { get; set; }

        public bool IsFrametimingGraphNeeded { get; set; } = false;
        public bool IsProbabilityDensityGraphNeeded { get; set; } = false;
        public bool IsProbabilityDistributionGraphNeeded { get; set; } = false;


        private ProgramsThatReadOutput _programThatReadOutput;
        public ProgramsThatReadOutput ProgramThatReadOutput
        {
            get => _programThatReadOutput;
            set
            {
                if (_programThatReadOutput != value)
                {
                    _programThatReadOutput = value;
                    this.RaisePropertyChanged(nameof(ProgramThatReadOutput));
                }
            }
        }

        public MainWindowViewModel()
        {
            SelectFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fileDialog = new OpenFileDialog()
                {
                    AllowMultiple = false,
                    Directory = Directory.GetCurrentDirectory()
                };

                string[] results = await fileDialog.ShowAsync((Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime)?.MainWindow);

                string selectedFile = results.FirstOrDefault(r => string.IsNullOrEmpty(r) is false);
                
                if (!string.IsNullOrEmpty(selectedFile))
                {
                    this.FilePath = selectedFile;
                }
            });
            UnselectFileCommand = ReactiveCommand.Create(() =>
            {
                this.FilePath = "";
            });

            LogCreationPrograms.AddRange(FTAnlzer.SupportedPrograms);
        }
    }
}
