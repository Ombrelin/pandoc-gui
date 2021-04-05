using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
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
                    v => v.SearchTargetFileButton
                ).DisposeWith(disposable);
            });
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}