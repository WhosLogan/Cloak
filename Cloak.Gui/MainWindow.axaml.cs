using System;
using System.Collections;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Reactive;
using Cloak.Core.Protections;

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
    }

    // ReSharper disable once UnusedParameter.local
    private void ProtectButtonClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(InputBox.Text) || string.IsNullOrWhiteSpace(OutputBox.Text)) return;
        if (Protections.SelectedItems is not null)
            _cloak.Protections.ForEach(p => p.Enabled = Protections.SelectedItems.Contains(p.Name));
        _cloak.Protect(InputBox.Text, OutputBox.Text);
    }
}