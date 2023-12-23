using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace Fov_Menu;

public partial class MainWindow
{
    private readonly Addresses? _addresses;
    
    public MainWindow()
    {
        InitializeComponent();
        _addresses = new Addresses(this);
        Task.Run(_addresses.OpenGameProcess);
    }

    private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }


    private void NumericUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        if (_addresses == null)
        {
            return;
        }
        
        var numericUpDown = (NumericUpDown)sender;
        
        if (numericUpDown.Value == null)
        {
            return;
        }
            
        var numericUpDownName = numericUpDown.Name;
        var value = Convert.ToSingle(numericUpDown.Value);
        _addresses.WriteValue(numericUpDownName, value);
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Environment.Exit(1);
    }
}