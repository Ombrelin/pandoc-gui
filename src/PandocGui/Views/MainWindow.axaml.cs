using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using AvaloniaProgressRing;
using FluentAvalonia.UI.Controls;
using PandocGui.CliWrapper;
using PandocGui.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace PandocGui.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private Border EmptyStatePanel => this.FindControl<Border>("emptyStatePanel")!;
    private DockPanel LoadedStatePanel => this.FindControl<DockPanel>("loadedStatePanel")!;
    private Button BrowseSourceEmptyButton => this.FindControl<Button>("browseSourceEmptyButton")!;

    private TextBox SearchSourceFileInput => this.FindControl<TextBox>("searchSourceFileInput")!;
    private ComboBox InputFormatComboBox => this.FindControl<ComboBox>("inputFormatCombo")!;
    private Button SearchSourceFileButton => this.FindControl<Button>("searchSourceFileButton")!;

    private TextBox SearchTargetFileInput => this.FindControl<TextBox>("searchTargetFileInput")!;
    private ComboBox OutputFormatComboBox => this.FindControl<ComboBox>("outputFormatCombo")!;
    private Button SearchTargetFileButton => this.FindControl<Button>("searchTargetFileButton")!;

    private Button ExportButton => this.FindControl<Button>("exportButton")!;
    private ProgressRing Loader => this.FindControl<ProgressRing>("loader")!;
    private Button ClearButton => this.FindControl<Button>("clearButton")!;
    private Button CopyButton => this.FindControl<Button>("copyCommandButton")!;
    private ToggleSwitch OpenOnCompletionToggle => this.FindControl<ToggleSwitch>("openOnCompletionToggle")!;
    private FAInfoBar StatusBar => this.FindControl<FAInfoBar>("statusBar")!;

    private ToggleSwitch HighlightEnabledToggle => this.FindControl<ToggleSwitch>("highlightToggle")!;
    private TextBox HighlightFileInput => this.FindControl<TextBox>("highlightFileInput")!;
    private Button HighlightFileButton => this.FindControl<Button>("highlightFileButton")!;
    private ToggleSwitch HeaderNumberingEnabledToggle => this.FindControl<ToggleSwitch>("headersNumberingToggle")!;
    private ToggleSwitch ContentTableToggle => this.FindControl<ToggleSwitch>("contentTableToggle")!;
    private ToggleSwitch CustomFontEnabledToggle => this.FindControl<ToggleSwitch>("customFontToggle")!;
    private ComboBox CustomFontNameComboBox => this.FindControl<ComboBox>("fontNameCombobox")!;
    private ToggleSwitch CustomMarginEnabledToggle => this.FindControl<ToggleSwitch>("customMarginToggle")!;
    private NumericUpDown CustomMarginValueInput => this.FindControl<NumericUpDown>("customMarginInput")!;
    private ToggleSwitch PdfEngineToggle => this.FindControl<ToggleSwitch>("pdfEngineToggle")!;
    private ComboBox PdfEngineCombobox => this.FindControl<ComboBox>("pdfEngineCombobox")!;
    private MenuItem OpenLogFolderMenu => this.FindControl<MenuItem>("openLogFolderMenu")!;

    public MainWindow()
    {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);

        this.WhenActivated((CompositeDisposable disposable) =>
        {
            this.OneWayBind(ViewModel, vm => vm.HasInput, v => v.LoadedStatePanel.IsVisible)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.HasInput, v => v.EmptyStatePanel.IsVisible, hasInput => !hasInput)
                .DisposeWith(disposable);

            this.BindCommand(ViewModel, vm => vm.SearchSourceFileCommand, v => v.BrowseSourceEmptyButton)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.SourcePath, v => v.SearchSourceFileInput.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.SupportedInputFormats, v => v.InputFormatComboBox.ItemsSource)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.SelectedInputFormat, v => v.InputFormatComboBox.SelectedItem)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, vm => vm.SearchSourceFileCommand, v => v.SearchSourceFileButton)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.TargetPath, v => v.SearchTargetFileInput.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.SupportedOutputFormats, v => v.OutputFormatComboBox.ItemsSource)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.SelectedOutputFormat, v => v.OutputFormatComboBox.SelectedItem)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, vm => vm.SearchTargetFileCommand, v => v.SearchTargetFileButton)
                .DisposeWith(disposable);

            this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.IsExporting, v => v.Loader.IsVisible)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, vm => vm.ClearCommand, v => v.ClearButton)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, vm => vm.CopyCommand, v => v.CopyButton)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.OpenFileOnCompletion, v => v.OpenOnCompletionToggle.IsChecked)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.IsStatusVisible, v => v.StatusBar.IsOpen)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.Result, v => v.StatusBar.Message)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.IsError, v => v.StatusBar.Severity, ErrorToSeverity)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.CustomHighlightThemeEnabled, v => v.HighlightEnabledToggle.IsChecked)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.CustomHighlightThemeSource, v => v.HighlightFileInput.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.CustomHighlightThemeEnabled, v => v.HighlightFileInput.IsEnabled)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.CustomHighlightThemeEnabled, v => v.HighlightFileButton.IsEnabled)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, vm => vm.SearchHighlightThemeSourceCommand, v => v.HighlightFileButton)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.NumberedHeadersEnabled, v => v.HeaderNumberingEnabledToggle.IsChecked)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.TableOfContentEnabled, v => v.ContentTableToggle.IsChecked)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.CustomFontEnabled, v => v.CustomFontEnabledToggle.IsChecked)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.CustomFontEnabled, v => v.CustomFontNameComboBox.IsEnabled)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.InstalledFonts, v => v.CustomFontNameComboBox.ItemsSource)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.CustomFontName, v => v.CustomFontNameComboBox.SelectedItem)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.CustomMarginEnabled, v => v.CustomMarginEnabledToggle.IsChecked)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.CustomMarginValue, v => v.CustomMarginValueInput.Value,
                    MarginToValue, ValueToMargin)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.CustomMarginEnabled, v => v.CustomMarginValueInput.IsEnabled)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.CustomPdfEngineEnabled, v => v.PdfEngineToggle.IsChecked)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.SupportedEngine, v => v.PdfEngineCombobox.ItemsSource)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.CustomPdfEngineEnabled, v => v.PdfEngineCombobox.IsEnabled)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.CustomPdfEngineValue, v => v.PdfEngineCombobox.SelectedItem)
                .DisposeWith(disposable);

            this.BindCommand(ViewModel, vm => vm.OpenLogFolderCommand, v => v.OpenLogFolderMenu)
                .DisposeWith(disposable);
        });
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Contains(DataFormat.File) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        var files = e.DataTransfer.TryGetFiles();
        var path = files?.OfType<IStorageFile>().FirstOrDefault()?.TryGetLocalPath();

        if (!string.IsNullOrWhiteSpace(path) && PandocFormats.IsSupportedInput(path))
        {
            ViewModel.SourcePath = path;
        }
    }

    private FAInfoBarSeverity ErrorToSeverity(bool isError) =>
        isError ? FAInfoBarSeverity.Error : FAInfoBarSeverity.Success;

    private decimal? MarginToValue(decimal value) => value;
    private decimal ValueToMargin(decimal? value) => value ?? 1.3m;
}
