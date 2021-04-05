using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using AvaloniaProgressRing;
using PandocGui.ViewModels;
using ReactiveUI;

namespace PandocGui.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public TextBox SearchSourceFileInput => this.FindControl<TextBox>("searchSourceFileInput");
        public Button SearchSourceFileButton => this.FindControl<Button>("searchSourceFileButton");
        
        public TextBox SearchTargetFileInput => this.FindControl<TextBox>("searchTargetFileInput");
        public Button SearchTargetFileButton => this.FindControl<Button>("searchTargetFileButton");
        public Button ExportButton => this.FindControl<Button>("exportButton");
        public ProgressRing Loader => this.FindControl<ProgressRing>("loader");

        public ToggleSwitch HighlightEnabledToggle => this.FindControl<ToggleSwitch>("highlightToggle");
        public ToggleSwitch HeaderNumberingEnabledToggle => this.FindControl<ToggleSwitch>("headersNumberingToggle");
        public TextBox HighlightFileInput => this.FindControl<TextBox>("highlightFileInput");
        public Button HighlightFileButton => this.FindControl<Button>("highlightFileButton");

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel,
                    vm => vm.SourcePath,
                    v => v.SearchSourceFileInput.Text
                ).DisposeWith(disposable);
                
                this.BindCommand(ViewModel,
                    vm => vm.SearchSourceFileCommand,
                    v => v.SearchSourceFileButton
                ).DisposeWith(disposable);
                
                this.Bind(ViewModel,
                    vm => vm.TargetPath,
                    v => v.SearchTargetFileInput.Text
                ).DisposeWith(disposable);
                
                this.BindCommand(ViewModel,
                    vm => vm.SearchTargetFileCommand,
                    v => v.SearchTargetFileButton
                ).DisposeWith(disposable);
                
                this.BindCommand(ViewModel,
                    vm => vm.ExportCommand,
                    v => v.ExportButton
                ).DisposeWith(disposable);
                
                this.Bind(ViewModel,
                    vm => vm.Loading,
                    v => v.Loader.IsVisible
                ).DisposeWith(disposable);
                
                this.OneWayBind(ViewModel,
                    vm => vm.Loading,
                    v => v.ExportButton.IsVisible,
                    NotConverter
                ).DisposeWith(disposable);
                
                this.Bind(ViewModel,
                    vm => vm.CustomHighlightThemeEnabled,
                    v => v.HighlightEnabledToggle.IsChecked
                ).DisposeWith(disposable);
                
                this.BindCommand(ViewModel,
                    vm => vm.SearchHighlightThemeSourceCommand,
                    v => v.HighlightFileButton
                ).DisposeWith(disposable);
                
                this.Bind(ViewModel,
                    vm => vm.CustomHighlightThemeSource,
                    v => v.HighlightFileInput.Text
                ).DisposeWith(disposable);
                
                this.OneWayBind(ViewModel,
                    vm => vm.CustomHighlightThemeEnabled,
                    v => v.HighlightFileInput.IsEnabled
                ).DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                    vm => vm.CustomHighlightThemeEnabled,
                    v => v.HighlightFileButton.IsEnabled
                ).DisposeWith(disposable);
            });
            
            
        }

        private bool NotConverter(bool val) => !val;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}