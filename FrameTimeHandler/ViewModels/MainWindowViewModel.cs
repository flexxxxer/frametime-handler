using System;
using System.Collections.Generic;
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
using MessageBox.Avalonia;
using Color = Avalonia.Media.Color;

namespace FrameTimeHandler.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _filePath = "";
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
        public bool IsFileSelected => string.IsNullOrEmpty(FilePath) is false;

        private string _graphsFilePath = "";
        public string GraphsFilePath
        {
            get => _graphsFilePath;
            set
            {
                if (_graphsFilePath != value)
                {
                    _graphsFilePath = value;
                    this.RaisePropertyChanged(nameof(GraphsFilePath));
                    this.RaisePropertyChanged(nameof(IsGraphsFileSelected));
                }
            }
        }
        public bool IsGraphsFileSelected => string.IsNullOrEmpty(GraphsFilePath) is false;

        public ReactiveCommand<Unit, Unit> SelectFileCommand { get; }
        public ReactiveCommand<Unit, Unit> UnselectFileCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectGraphsFileCommand { get; }
        public ReactiveCommand<Unit, Unit> UnselectGraphsFileCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveGraphsCommand { get; }

        public ReactiveCommand<Unit, Unit> ChangeFrameTimingGraphColorCommand { get; }
        public ReactiveCommand<Unit, Unit> ChangeProbabilityDensityGraphColorCommand { get; }
        public ReactiveCommand<Unit, Unit> ChangeProbabilityDistributionGraphColorCommand { get; }

        public ObservableCollection<string> LogCreationPrograms { get; set; } = new ObservableCollection<string>();

        private string _selectedProgram = "";
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
                    this.RaisePropertyChanged(nameof(IsNeededGraphsColorsUnique));
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
                    this.RaisePropertyChanged(nameof(IsNeededGraphsColorsUnique));
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
                    this.RaisePropertyChanged(nameof(IsNeededGraphsColorsUnique));
                }
            }
        }

        private bool _isFrameTimingGraphChangingColor;
        public bool IsFrameTimingGraphChangingColor
        {
            get => _isFrameTimingGraphChangingColor;
            set
            {
                if (_isFrameTimingGraphChangingColor != value)
                {
                    _isFrameTimingGraphChangingColor = value;
                    this.RaisePropertyChanged(nameof(IsFrameTimingGraphChangingColor));
                }
            }
        }

        private bool _isProbabilityDensityChangingColor;
        public bool IsProbabilityDensityChangingColor
        {
            get => _isProbabilityDensityChangingColor;
            set
            {
                if (_isProbabilityDensityChangingColor != value)
                {
                    _isProbabilityDensityChangingColor = value;
                    this.RaisePropertyChanged(nameof(IsProbabilityDensityChangingColor));
                }
            }
        }

        private bool _isProbabilityDistributionGraphChangingColor;
        public bool IsProbabilityDistributionGraphChangingColor
        {
            get => _isProbabilityDistributionGraphChangingColor;
            set
            {
                if (_isProbabilityDistributionGraphChangingColor != value)
                {
                    _isProbabilityDistributionGraphChangingColor = value;
                    this.RaisePropertyChanged(nameof(IsProbabilityDistributionGraphChangingColor));
                }
            }
        }

        private Color _frameTimingGraphColor;
        public Color FrameTimingGraphColor
        {
            get => _frameTimingGraphColor;
            set
            {
                if (!_frameTimingGraphColor.Equals(value))
                {
                    _frameTimingGraphColor = value;
                    this.RaisePropertyChanged(nameof(FrameTimingGraphColor));
                    this.RaisePropertyChanged(nameof(IsNeededGraphsColorsUnique));
                }
            }
        }

        private Color _probabilityDensityGraphColor;
        public Color ProbabilityDensityGraphColor
        {
            get => _probabilityDensityGraphColor;
            set
            {
                if (!_probabilityDensityGraphColor.Equals(value))
                {
                    _probabilityDensityGraphColor = value;
                    this.RaisePropertyChanged(nameof(ProbabilityDensityGraphColor));
                    this.RaisePropertyChanged(nameof(IsNeededGraphsColorsUnique));
                }
            }
        }

        private Color _probabilityDistributionGraphColor;
        public Color ProbabilityDistributionGraphColor
        {
            get => _probabilityDistributionGraphColor;
            set
            {
                if (!_probabilityDistributionGraphColor.Equals(value))
                {
                    _probabilityDistributionGraphColor = value;
                    this.RaisePropertyChanged(nameof(ProbabilityDistributionGraphColor));
                    this.RaisePropertyChanged(nameof(IsNeededGraphsColorsUnique));
                }
            }
        }

        private string _testName = "";
        public string TestName
        {
            get => _testName;
            set
            {
                if (_testName != value)
                {
                    _testName = value;
                    this.RaisePropertyChanged(nameof(TestName));
                }
            }
        }

        private bool _isAppend = false;

        public bool IsAppend
        {
            get => _isAppend;
            set
            {
                if (_isAppend != value)
                {
                    _isAppend = value;
                    this.RaisePropertyChanged(nameof(IsAppend));
                }
            }
        }

        public bool IsNeededGraphsColorsUnique
        {
            get
            {
                IEnumerable<Color> NeededColors()
                {
                    if (IsFrametimingGraphNeeded)
                        yield return FrameTimingGraphColor;

                    if (IsProbabilityDensityGraphNeeded)
                        yield return ProbabilityDensityGraphColor;

                    if (IsProbabilityDistributionGraphNeeded)
                        yield return ProbabilityDistributionGraphColor;
                }

                return !NeededColors().GroupBy(color => color).Any(g => g.Count() > 1);
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

            SelectGraphsFileCommand = ReactiveCommand.CreateFromTask(async () =>
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
                    this.GraphsFilePath = selectedFile;
                }
            });
            UnselectGraphsFileCommand = ReactiveCommand.Create(() =>
            {
                this.GraphsFilePath = "";
            });

            SaveGraphsCommand = ReactiveCommand.Create(() =>
            {
                IEnumerable<(GraphTypes graphType, Color color)> NeededGraphs()
                {
                    if (IsFrametimingGraphNeeded)
                        yield return (GraphTypes.FrameTiming, FrameTimingGraphColor);

                    if (IsProbabilityDensityGraphNeeded)
                        yield return (GraphTypes.ProbabilityDensity, ProbabilityDensityGraphColor);

                    if (IsProbabilityDistributionGraphNeeded)
                        yield return (GraphTypes.ProbabilityDistribution, ProbabilityDistributionGraphColor);
                }

                var (graphData, error) = FTAnlzer.GetGraphsData(FilePath, SelectedProgram, NeededGraphs());

                if (!string.IsNullOrEmpty(error))
                {
                    var errorMessage = MessageBoxManager.GetMessageBoxStandardWindow("Error", $"Something went wrong. Check if arguments are valid. Error: {Environment.NewLine}{Environment.NewLine}{error}");
                    
                    errorMessage.ShowDialog(
                        (Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime)
                        ?.MainWindow);

                    return;
                }

                GraphExporter.Export(GraphsFilePath, TestName, ProgramThatReadOutput, IsAppend, graphData);

            }, this.WhenAnyValue(
                vm => vm.IsFileSelected, 
                vm => vm.IsGraphsFileSelected, 
                vm => vm.SelectedProgram, 
                vm => vm.ProgramThatReadOutput,
                vm => vm.IsFrametimingGraphNeeded,
                vm => vm.IsProbabilityDensityGraphNeeded, 
                vm => vm.IsProbabilityDistributionGraphNeeded,
                vm => vm.TestName,
                vm => vm.IsNeededGraphsColorsUnique,
                (isFileSelected, isGraphsFileSelected, selectedProgram, programThatReadOutput, isFrametimingGraphNeeded, isProbabilityDensityGraphNeeded, isProbabilityDistributionGraphNeeded, testName, isNeededGraphsColorsUnique) 
                    => isFileSelected && isGraphsFileSelected
                       && !string.IsNullOrEmpty(selectedProgram) 
                       && programThatReadOutput != ProgramsThatReadOutput.None 
                       && (isFrametimingGraphNeeded || isProbabilityDensityGraphNeeded || isProbabilityDistributionGraphNeeded)
                       && isNeededGraphsColorsUnique
                       && !string.IsNullOrEmpty(testName)));

            ChangeFrameTimingGraphColorCommand = ReactiveCommand.Create(() =>
            {
                IsProbabilityDensityChangingColor = IsProbabilityDistributionGraphChangingColor = false;

                IsFrameTimingGraphChangingColor = !IsFrameTimingGraphChangingColor;
            });

            ChangeProbabilityDensityGraphColorCommand = ReactiveCommand.Create(() =>
            {
                IsFrameTimingGraphChangingColor = IsProbabilityDistributionGraphChangingColor = false;

                IsProbabilityDensityChangingColor = !IsProbabilityDensityChangingColor;
            });

            ChangeProbabilityDistributionGraphColorCommand = ReactiveCommand.Create(() =>
            {
                IsFrameTimingGraphChangingColor = IsProbabilityDensityChangingColor = false;

                IsProbabilityDistributionGraphChangingColor = !IsProbabilityDistributionGraphChangingColor;
            });

            LogCreationPrograms.AddRange(FTAnlzer.SupportedPrograms);

            FrameTimingGraphColor = Color.FromArgb(255, 0, 0, 0);
            ProbabilityDensityGraphColor = Color.FromArgb(255, 0, 0, 0);
            ProbabilityDistributionGraphColor = Color.FromArgb(255, 0, 0, 0);
        }
    }
}
