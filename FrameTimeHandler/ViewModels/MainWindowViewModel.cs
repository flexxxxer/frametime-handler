using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Joins;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
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
        public ReactiveCommand<Unit, Unit> SaveGraphsCommand { get; }
        public ReactiveCommand<Unit, Unit> ChangeCustomColorCommand { get; }

        public ObservableCollection<string> LogCreationPrograms { get; set; } = new ObservableCollection<string>();

        private string _selectedProgram;
        public string SelectedProgram
        {
            get => _selectedProgram;
            set
            {
                if (_selectedProgram != value)
                {
                    _selectedProgram = value;
                    this.RaisePropertyChanged(nameof(SelectedProgram));
                }
            }
        }

        private bool _isFrametimingGraphNeeded;
        public bool IsFrametimingGraphNeeded
        {
            get => _isFrametimingGraphNeeded;
            set
            {
                if (_isFrametimingGraphNeeded != value)
                {
                    _isFrametimingGraphNeeded = value;
                    this.RaisePropertyChanged(nameof(IsFrametimingGraphNeeded));
                }
            }
        }

        private bool _isProbabilityDensityGraphNeeded;
        public bool IsProbabilityDensityGraphNeeded
        {
            get => _isProbabilityDensityGraphNeeded;
            set
            {
                if (_isProbabilityDensityGraphNeeded != value)
                {
                    _isProbabilityDensityGraphNeeded = value;
                    this.RaisePropertyChanged(nameof(IsProbabilityDensityGraphNeeded));
                }
            }
        }

        private bool _isProbabilityDistributionGraphNeeded;
        public bool IsProbabilityDistributionGraphNeeded
        {
            get => _isProbabilityDistributionGraphNeeded;
            set
            {
                if (_isProbabilityDistributionGraphNeeded != value)
                {
                    _isProbabilityDistributionGraphNeeded = value;
                    this.RaisePropertyChanged(nameof(IsProbabilityDistributionGraphNeeded));
                }
            }
        }

        private bool _isChangingColor;
        public bool IsChangingColor
        {
            get => _isChangingColor;
            set
            {
                if (_isChangingColor != value)
                {
                    _isChangingColor = value;
                    this.RaisePropertyChanged(nameof(IsChangingColor));
                }
            }
        }

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

        private Color _customColor;
        public Color CustomColor
        {
            get => _customColor;
            set
            {
                if (!_customColor.Equals(value))
                {
                    _customColor = value;
                    this.RaisePropertyChanged(nameof(CustomColor));
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
            SaveGraphsCommand = ReactiveCommand.Create(() =>
            {


            }, this.WhenAnyValue(
                vm => vm.IsFileSelected, 
                vm => vm.SelectedProgram, 
                vm => vm.ProgramThatReadOutput,
                vm => vm.IsFrametimingGraphNeeded,
                vm => vm.IsProbabilityDensityGraphNeeded, 
                vm => vm.IsProbabilityDistributionGraphNeeded,
                (isFileSelected, selectedProgram, programThatReadOutput, isFrametimingGraphNeeded, isProbabilityDensityGraphNeeded, isProbabilityDistributionGraphNeeded) 
                    => isFileSelected 
                       && !string.IsNullOrEmpty(selectedProgram) 
                       && programThatReadOutput != ProgramsThatReadOutput.None 
                       && (isFrametimingGraphNeeded || isProbabilityDensityGraphNeeded ||
                           isProbabilityDistributionGraphNeeded)));

            ChangeCustomColorCommand = ReactiveCommand.Create(() =>
            {
                IsChangingColor = !IsChangingColor;
            });

            LogCreationPrograms.AddRange(FTAnlzer.SupportedPrograms);
            CustomColor = Color.FromArgb(255, 0, 0, 0);
        }
    }
}
