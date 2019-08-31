﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.ExtendedToolkit;
using Avalonia.ExtendedToolkit.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.ExampleApp.Views
{
    public class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
