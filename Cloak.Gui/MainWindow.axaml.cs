using System;
using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Reactive;

namespace Cloak.Gui;

public partial class MainWindow : Window
{
    private readonly Core.Cloak _cloak = new();
    
    public MainWindow()
    {
        InitializeComponent();
        Title = "Cloak Obfuscator";
        CanResize = false;
        Height = 400;
        Width = 350;
        Protections.ItemsSource = _cloak.Protections.ConvertAll(p => p.Name);
        Protections.GetObservable(ListBox.SelectedItemsProperty)
            .Subscribe(new AnonymousObserver<IList>(selected => 
                _cloak.Protections.ForEach(p => p.Enabled = selected.Contains(p.Name)))!);
    }

    // ReSharper disable once UnusedParameter.local
    private void ProtectButtonClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(InputBox.Text) || string.IsNullOrWhiteSpace(OutputBox.Text)) return;
        _cloak.Protect(InputBox.Text, OutputBox.Text);
    }
}