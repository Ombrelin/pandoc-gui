using System;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using AvaloniaProgressRing;
using PandocGui.CliWrapper.Command;
using PandocGui.ViewModels;
using ReactiveUI;

namespace PandocGui.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private TextBox SearchSourceFileInput => this.FindControl<TextBox>("searchSourceFileInput");
        private Button SearchSourceFileButton => this.FindControl<Button>("searchSourceFileButton");

        private TextBox SearchTargetFileInput => this.FindControl<TextBox>("searchTargetFileInput");
        private Button SearchTargetFileButton => this.FindControl<Button>("searchTargetFileButton");
        private Button ExportButton => this.FindControl<Button>("exportButton");
        private ProgressRing Loader => this.FindControl<ProgressRing>("loader");

        private ToggleSwitch HighlightEnabledToggle => this.FindControl<ToggleSwitch>("highlightToggle");
        private ToggleSwitch HeaderNumberingEnabledToggle => this.FindControl<ToggleSwitch>("headersNumberingToggle");
        private TextBox HighlightFileInput => this.FindControl<TextBox>("highlightFileInput");
        private Button HighlightFileButton => this.FindControl<Button>("highlightFileButton");

        private ToggleSwitch CustomFontEnabledToggle => this.FindControl<ToggleSwitch>("customFontToggle");

        private TextBox CustomFontNameInput => this.FindControl<TextBox>("fontNameInput");
        private ToggleSwitch CustomMarginEnabledToggle => this.FindControl<ToggleSwitch>("customMarginToggle");
        private NumericUpDown CustomMarginValueInput => this.FindControl<NumericUpDown>("customMarginInput");
        private ComboBox PdfEngineCombobox => this.FindControl<ComboBox>("pdfEngineCombobox");
        private ToggleSwitch PdfEngineToggle => this.FindControl<ToggleSwitch>("pdfEngineToggle");
        private ToggleSwitch ContentTableToggle => this.FindControl<ToggleSwitch>("contentTableToggle");
        private TextBlock ResultText => this.FindControl<TextBlock>("resultText");
        
        
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

                this.Bind(ViewModel,
                    vm => vm.NumberedHeadersEnabled,
                    v => v.HeaderNumberingEnabledToggle.IsChecked
                ).DisposeWith(disposable);
                this.Bind(ViewModel,
                    vm => vm.CustomFontEnabled,
                    v => v.CustomFontEnabledToggle.IsChecked
                ).DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                    vm => vm.CustomFontEnabled,
                    v => v.CustomFontNameInput.IsEnabled
                ).DisposeWith(disposable);
                this.Bind(ViewModel,
                    vm => vm.CustomFontName,
                    v => v.CustomFontNameInput.Text
                ).DisposeWith(disposable);
                this.Bind(ViewModel,
                    vm => vm.CustomMarginEnabled,
                    v => v.CustomMarginEnabledToggle.IsChecked
                ).DisposeWith(disposable);
                this.Bind(ViewModel,
                    vm => vm.CustomMarginValue,
                    v => v.CustomMarginValueInput.Value,
                    DoubleToDecimalConverter, DecimalToDoubleConverter
                ).DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                    vm => vm.CustomMarginEnabled,
                    v => v.CustomMarginValueInput.IsEnabled
                ).DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                    vm => vm.SupportedEngine,
                    v => v.PdfEngineCombobox.Items
                ).DisposeWith(disposable);
                this.Bind(ViewModel,
                    vm => vm.CustomPdfEngineEnabled,
                    v => v.PdfEngineToggle.IsChecked
                ).DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                    vm => vm.CustomPdfEngineEnabled,
                    v => v.PdfEngineCombobox.IsEnabled
                ).DisposeWith(disposable);
                this.Bind(ViewModel,
                    vm => vm.CustomPdfEngineValue,
                    v => v.PdfEngineCombobox.SelectedItem
                ).DisposeWith(disposable);
                this.Bind(ViewModel,
                    vm => vm.TableOfContentEnabled,
                    v => v.ContentTableToggle.IsChecked
                ).DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                    vm => vm.Result,
                    v => v.ResultText.Text
                ).DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                    vm => vm.IsError,
                    v => v.ResultText.Foreground,
                    ErrorStatusToColor
                ).DisposeWith(disposable);
            });
        }

        private IBrush ErrorStatusToColor(bool isError)
        {
            return isError ? Brushes.Red : Brushes.Green;
        }

        private bool NotConverter(bool val) => !val;
        private double DoubleToDecimalConverter(decimal dec) => (double) dec;
        private decimal DecimalToDoubleConverter(double dec) => (decimal) dec;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}