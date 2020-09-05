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

                    var (stat, error) = FTAnlzer.GetStatistics(this.FilePath, this.SelectedProgram);

                    if (string.IsNullOrEmpty(error))
                    {
                        this.Statistics = stat;
                        this.IsSelectedProgramCorrect = true;
                    }
                    else
                    {
                        this.IsSelectedProgramCorrect = false;
                    }
                }
            }
        }
        public bool IsFileSelected => string.IsNullOrEmpty(FilePath) is false;

        private string _frametimingGraphFilePath = "";
        public string FrametimingGraphFilePath
        {
            get => _frametimingGraphFilePath;
            set
            {
                if (_frametimingGraphFilePath != value)
                {
                    _frametimingGraphFilePath = value;
                    this.RaisePropertyChanged(nameof(FrametimingGraphFilePath));
                    this.RaisePropertyChanged(nameof(IsFrametimingGraphFileSelected));
                }
            }
        }
        public bool IsFrametimingGraphFileSelected => string.IsNullOrEmpty(FrametimingGraphFilePath) is false;

        private string _probabilityDensityGraphFilePath = "";
        public string ProbabilityDensityGraphFilePath
        {
            get => _probabilityDensityGraphFilePath;
            set
            {
                if (_probabilityDensityGraphFilePath != value)
                {
                    _probabilityDensityGraphFilePath = value;
                    this.RaisePropertyChanged(nameof(ProbabilityDensityGraphFilePath));
                    this.RaisePropertyChanged(nameof(IsProbabilityDensityGraphFileSelected));
                }
            }
        }
        public bool IsProbabilityDensityGraphFileSelected => string.IsNullOrEmpty(ProbabilityDensityGraphFilePath) is false;

        private string _probabilityDistributionGraphFilePath = "";
        public string ProbabilityDistributionGraphFilePath
        {
            get => _probabilityDistributionGraphFilePath;
            set
            {
                if (_probabilityDistributionGraphFilePath != value)
                {
                    _probabilityDistributionGraphFilePath = value;
                    this.RaisePropertyChanged(nameof(ProbabilityDistributionGraphFilePath));
                    this.RaisePropertyChanged(nameof(IsProbabilityDistributionGraphFileSelected));
                }
            }
        }
        public bool IsProbabilityDistributionGraphFileSelected => string.IsNullOrEmpty(ProbabilityDistributionGraphFilePath) is false;

        public ReactiveCommand<Unit, Unit> SelectFileCommand { get; }
        public ReactiveCommand<Unit, Unit> UnselectFileCommand { get; }

        public ReactiveCommand<Unit, Unit> SelectFrametimingGraphFileCommand { get; }
        public ReactiveCommand<Unit, Unit> UnselectFrametimingGraphFileCommand { get; }

        public ReactiveCommand<Unit, Unit> SelectProbabilityDensityGraphFileCommand { get; }
        public ReactiveCommand<Unit, Unit> UnselectProbabilityDensityGraphFileCommand { get; }

        public ReactiveCommand<Unit, Unit> SelectProbabilityDistributionGraphFileCommand { get; }
        public ReactiveCommand<Unit, Unit> UnselectProbabilityDistributionGraphFileCommand { get; }

        public ReactiveCommand<Unit, Unit> SaveGraphsCommand { get; }

        public ReactiveCommand<Unit, Unit> ChangeFrameTimingGraphColorCommand { get; }
        public ReactiveCommand<Unit, Unit> ChangeProbabilityDensityGraphColorCommand { get; }
        public ReactiveCommand<Unit, Unit> ChangeProbabilityDistributionGraphColorCommand { get; }

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

        public ReactiveCommand<Unit, Unit> CopyStatToClipboardCommand { get; }

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

                    if(!this.IsFileSelected)
                        return;

                    var (stat, error) = FTAnlzer.GetStatistics(this.FilePath, value);

                    if (string.IsNullOrEmpty(error))
                    {
                        this.Statistics = stat;
                        this.IsSelectedProgramCorrect = true;
                    }
                    else
                    {
                        this.IsSelectedProgramCorrect = false;
                    }
                }
            }
        }

        private bool _isSelectedProgramCorrect;
        public bool IsSelectedProgramCorrect
        {
            get => _isSelectedProgramCorrect;
            set
            {
                if (_isSelectedProgramCorrect != value)
                {
                    _isSelectedProgramCorrect = value;
                    this.RaisePropertyChanged(nameof(IsSelectedProgramCorrect));
                }
            }
        }

        private FTStat _statistics = new FTStat();
        public FTStat Statistics
        {
            get => _statistics;
            set
            {
                if (_statistics != value)
                {
                    _statistics = value;
                    this.RaisePropertyChanged(nameof(Statistics));
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

        private bool _isAppend = true;

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
                    if (IsFrametimingGraphFileSelected)
                        yield return FrameTimingGraphColor;

                    if (IsProbabilityDensityGraphFileSelected)
                        yield return ProbabilityDensityGraphColor;

                    if (IsProbabilityDistributionGraphFileSelected)
                        yield return ProbabilityDistributionGraphColor;
                }

                return !NeededColors().GroupBy(color => color).Any(g => g.Count() > 1);
            }
        }

        public MainWindowViewModel()
        {
            CopyStatToClipboardCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (this.IsSelectedProgramCorrect)
                {
                    string statView = $"0.1%: {this.Statistics.Occasion01}" + Environment.NewLine +
                                      $"1%: {this.Statistics.Occasion1}" + Environment.NewLine +
                                      $"5%: {this.Statistics.Occasion5}" + Environment.NewLine +
                                      $"50%: {this.Statistics.Occasion50}" + Environment.NewLine +
                                      $"Avg: {this.Statistics.Avg}" + Environment.NewLine;

                    await TextCopy.ClipboardService.SetTextAsync(statView);
                }
            });

            SelectFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fileDialog = new OpenFileDialog()
                {
                    AllowMultiple = false,
                };

                string[] results = await fileDialog.ShowAsync((Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime)?.MainWindow);

                if (results is null || results.Length == 0)
                {
                    return;
                }

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

            SelectFrametimingGraphFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fileDialog = new OpenFileDialog()
                {
                    AllowMultiple = false,
                };

                string[] results = await fileDialog.ShowAsync((Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime)?.MainWindow);

                if (results is null || results.Length == 0)
                {
                    return;
                }

                string selectedFile = results.FirstOrDefault(r => string.IsNullOrEmpty(r) is false);

                if (!string.IsNullOrEmpty(selectedFile))
                {
                    this.FrametimingGraphFilePath = selectedFile;
                }
            });
            UnselectFrametimingGraphFileCommand = ReactiveCommand.Create(() =>
            {
                this.FrametimingGraphFilePath = "";
            });

            SelectProbabilityDensityGraphFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fileDialog = new OpenFileDialog()
                {
                    AllowMultiple = false,
                };

                string[] results = await fileDialog.ShowAsync((Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime)?.MainWindow);

                if (results is null || results.Length == 0)
                {
                    return;
                }

                string selectedFile = results.FirstOrDefault(r => string.IsNullOrEmpty(r) is false);

                if (!string.IsNullOrEmpty(selectedFile))
                {
                    this.ProbabilityDensityGraphFilePath = selectedFile;
                }
            });
            UnselectProbabilityDensityGraphFileCommand = ReactiveCommand.Create(() =>
            {
                this.ProbabilityDensityGraphFilePath = "";
            });

            SelectProbabilityDistributionGraphFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fileDialog = new OpenFileDialog()
                {
                    AllowMultiple = false,
                };

                string[] results = await fileDialog.ShowAsync((Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime)?.MainWindow);

                if (results is null || results.Length == 0)
                {
                    return;
                }

                string selectedFile = results.FirstOrDefault(r => string.IsNullOrEmpty(r) is false);

                if (!string.IsNullOrEmpty(selectedFile))
                {
                    this.ProbabilityDistributionGraphFilePath = selectedFile;
                }
            });
            UnselectProbabilityDistributionGraphFileCommand = ReactiveCommand.Create(() =>
            {
                this.ProbabilityDistributionGraphFilePath = "";
            });

            SaveGraphsCommand = ReactiveCommand.Create(() =>
            {
                IEnumerable<(GraphTypes graphType, Color color, string path)> NeededGraphs()
                {
                    if (IsFrametimingGraphFileSelected)
                        yield return (GraphTypes.FrameTiming, FrameTimingGraphColor, this.FrametimingGraphFilePath);

                    if (IsProbabilityDensityGraphFileSelected)
                        yield return (GraphTypes.ProbabilityDensity, ProbabilityDensityGraphColor, this.ProbabilityDensityGraphFilePath);

                    if (IsProbabilityDistributionGraphFileSelected)
                        yield return (GraphTypes.ProbabilityDistribution, ProbabilityDistributionGraphColor, this.ProbabilityDistributionGraphFilePath);
                }

                var (graphData, error) = FTAnlzer.GetGraphsData(FilePath, SelectedProgram, NeededGraphs().Select(i => (i.graphType, i.color)));

                if (!string.IsNullOrEmpty(error))
                {
                    var errorMessage = MessageBoxManager.GetMessageBoxStandardWindow("Error", $"Something went wrong. Check if arguments are valid. Error: {Environment.NewLine}{Environment.NewLine}{error}");
                    
                    errorMessage.ShowDialog(
                        (Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime)
                        ?.MainWindow);

                    return;
                }

                var graphDataAsArray = graphData.ToArray();
                int i = 0;

                foreach (var (_, _, path) in NeededGraphs())
                {
                    GraphExporter.Export(path, TestName, ProgramThatReadOutput, IsAppend, graphDataAsArray[i++]);
                }
            }, this.WhenAnyValue(
                vm => vm.IsFileSelected, 
                vm => vm.SelectedProgram, 
                vm => vm.IsFrametimingGraphFileSelected, 
                vm => vm.IsProbabilityDensityGraphFileSelected, 
                vm => vm.IsProbabilityDistributionGraphFileSelected, 
                vm => vm.ProgramThatReadOutput,
                vm => vm.TestName,
                vm => vm.IsNeededGraphsColorsUnique,
                (isFileSelected, selectedProgram, isFrametimingGraphFileSelected, isProbabilityDensityGraphFileSelected, isProbabilityDistributionGraphFileSelected, programThatReadOutput, testName, isNeededGraphsColorsUnique) 
                    => isFileSelected
                       && !string.IsNullOrEmpty(selectedProgram) 
                       && programThatReadOutput != ProgramsThatReadOutput.None 
                       && (isFrametimingGraphFileSelected || isProbabilityDensityGraphFileSelected || isProbabilityDistributionGraphFileSelected)
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
