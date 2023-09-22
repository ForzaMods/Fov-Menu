using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IniParser;
using IniParser.Model;
using Memory;
using Microsoft.Win32;

namespace Fov_Menu;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        NumericBoxes(false);
        Task.Run(ForzaAttach);

        if (!Directory.Exists(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Forza Mods Fov Menu"))
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                      @"\Forza Mods Fov Menu");
    }

    #region Disable/enable numeric boxes

    private void NumericBoxes(bool enable = true)
    {
        Dispatcher.BeginInvoke((Action)delegate
        {
            ChaseMinBox.IsEnabled = enable;
            ChaseMaxBox.IsEnabled = enable;
            FarChaseMinBox.IsEnabled = enable;
            FarChaseMaxBox.IsEnabled = enable;
            DriverAndDashMinBox.IsEnabled = enable;
            DriverAndDashMaxBox.IsEnabled = enable;
            HoodMinBox.IsEnabled = enable;
            HoodMaxBox.IsEnabled = enable;
            BumperMinBox.IsEnabled = enable;
            BumperMaxBox.IsEnabled = enable;
            SaveLoadBox.IsEnabled = enable;
        });

        if (enable)
            Dispatcher.BeginInvoke((Action)delegate
            {
                ChaseMinBox.Value = m.ReadFloat(ChaseMin);
                ChaseMaxBox.Value = m.ReadFloat(ChaseMax);
                FarChaseMinBox.Value = m.ReadFloat(FarChaseMin);
                FarChaseMaxBox.Value = m.ReadFloat(FarChaseMax);
                DriverAndDashMinBox.Value = m.ReadFloat(DriverAndDashboardMin);
                DriverAndDashMaxBox.Value = m.ReadFloat(DriverAndDashboardMax);
                HoodMinBox.Value = m.ReadFloat(HoodMin);
                HoodMaxBox.Value = m.ReadFloat(HoodMax);
                BumperMinBox.Value = m.ReadFloat(BumperMin);
                BumperMaxBox.Value = m.ReadFloat(BumperMax);
            });
    }

    #endregion

    #region Saving/Loading

    private void SaveLoadBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        #region Vars

        var comboBox = (ComboBox)sender;
        var selectedItem = (ComboBoxItem)comboBox.SelectedItem;
        var selectedOption = selectedItem.Content.ToString();
        var openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Forza Mods Fov Menu";
        openFileDialog.Filter = "INI Files (*.ini)|*.ini";
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.InitialDirectory =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Forza Mods Fov Menu";
        saveFileDialog.Filter = "INI Files (*.ini)|*.ini";
        var SettingsParser = new FileIniDataParser();

        #endregion

        switch (selectedOption)
        {
            #region Saving

            case "Save":
                if (saveFileDialog.ShowDialog() == true)
                {
                    var SelectedFilePath = saveFileDialog.FileName;
                    var Settings = new IniData();

                    Settings["Settings"]["Chase Min"] = ChaseMinBox.Value?.ToString();
                    Settings["Settings"]["Chase Max"] = ChaseMaxBox.Value?.ToString();
                    Settings["Settings"]["Far Chase Min"] = FarChaseMinBox.Value?.ToString();
                    Settings["Settings"]["Far Chase Max"] = FarChaseMaxBox.Value?.ToString();
                    Settings["Settings"]["Driver And Dashboard Min"] = DriverAndDashMinBox.Value?.ToString();
                    Settings["Settings"]["Driver And Dashboard Max"] = DriverAndDashMaxBox.Value?.ToString();
                    Settings["Settings"]["Hood Min"] = HoodMinBox.Value?.ToString();
                    Settings["Settings"]["Hood Max"] = HoodMaxBox.Value?.ToString();
                    Settings["Settings"]["Bumper Min"] = BumperMinBox.Value?.ToString();
                    Settings["Settings"]["Bumper Max"] = BumperMaxBox.Value?.ToString();

                    SettingsParser.WriteFile(SelectedFilePath, Settings);
                }

                break;

            #endregion

            #region Loading

            case "Load":
                if (openFileDialog.ShowDialog() == true)
                {
                    var SelectedFilePath = openFileDialog.FileName;
                    var Settings = SettingsParser.ReadFile(SelectedFilePath);

                    double chaseMinValue,
                        chaseMaxValue,
                        farChaseMinValue,
                        farChaseMaxValue,
                        driverAndDashMinValue,
                        driverAndDashMaxValue,
                        hoodMinValue,
                        hoodMaxValue,
                        bumperMinValue,
                        bumperMaxValue;

                    if (double.TryParse(Settings["Settings"]["Chase Min"], out chaseMinValue))
                        ChaseMinBox.Value = chaseMinValue;
                    if (double.TryParse(Settings["Settings"]["Chase Max"], out chaseMaxValue))
                        ChaseMaxBox.Value = chaseMaxValue;
                    if (double.TryParse(Settings["Settings"]["Far Chase Min"], out farChaseMinValue))
                        FarChaseMinBox.Value = farChaseMinValue;
                    if (double.TryParse(Settings["Settings"]["Far Chase Max"], out farChaseMaxValue))
                        FarChaseMaxBox.Value = farChaseMaxValue;
                    if (double.TryParse(Settings["Settings"]["Driver And Dashboard Min"], out driverAndDashMinValue))
                        DriverAndDashMinBox.Value = driverAndDashMinValue;
                    if (double.TryParse(Settings["Settings"]["Driver And Dashboard Max"], out driverAndDashMaxValue))
                        DriverAndDashMaxBox.Value = driverAndDashMaxValue;
                    if (double.TryParse(Settings["Settings"]["Hood Min"], out hoodMinValue))
                        HoodMinBox.Value = hoodMinValue;
                    if (double.TryParse(Settings["Settings"]["Hood Max"], out hoodMaxValue))
                        HoodMaxBox.Value = hoodMaxValue;
                    if (double.TryParse(Settings["Settings"]["Bumper Min"], out bumperMinValue))
                        BumperMinBox.Value = bumperMinValue;
                    if (double.TryParse(Settings["Settings"]["Bumper Max"], out bumperMaxValue))
                        BumperMaxBox.Value = bumperMaxValue;
                }

                break;

            #endregion

            #region Default

            case "Default":
                ChaseMinBox.Value = 43.5;
                ChaseMaxBox.Value = 65;
                FarChaseMinBox.Value = 43.5;
                FarChaseMaxBox.Value = 65;
                DriverAndDashMinBox.Value = 40;
                DriverAndDashMaxBox.Value = 55;
                HoodMinBox.Value = 40;
                HoodMaxBox.Value = 70;
                BumperMinBox.Value = 40;
                BumperMaxBox.Value = 70;
                break;

            #endregion
        }
    }

    #endregion

    #region Vars

    public static Mem m = new();
    public static long Base;
    public static string ChaseMin;
    public static string ChaseMax;
    public static string FarChaseMin;
    public static string FarChaseMax;
    public static string DriverAndDashboardMin;
    public static string DriverAndDashboardMax;
    public static string HoodMin;
    public static string HoodMax;
    public static string BumperMin;
    public static string BumperMax;

    #endregion

    #region Attaching/Scanning

    private void ForzaAttach()
    {
        var Attached = false;

        while (true)
        {
            Thread.Sleep(1000);
            if (m.OpenProcess("ForzaHorizon5"))
            {
                if (Attached)
                    continue;
                Task.Run(() => Scanning(5));

                Attached = true;
            }
            else if (m.OpenProcess("ForzaHorizon4"))
            {
                if (Attached)
                    continue;
                Task.Run(() => Scanning(4));

                Attached = true;
            }
            else
            {
                if (!Attached)
                    continue;

                NumericBoxes(false);
                Attached = false;
            }
        }
    }

    private async void Scanning(int ver)
    {
        var TargetProcess = Process.GetProcessesByName("ForzaHorizon" + ver)[0];
        long ScanStart = TargetProcess.MainModule!.BaseAddress;
        long ScanEnd = TargetProcess.MainModule!.BaseAddress + TargetProcess.MainModule.ModuleMemorySize;

        switch (ver)
        {
            case 5:
            {
                List<long> Bases1;
                List<long> Bases2;
                long Base1 = 0;
                long Base2 = 0;
                long Base3 = 0;
                long Base4 = 0;
                long Base5 = 0;

                Bases1 = (await m.AoBScan(ScanStart, ScanEnd,
                        "00 00 ? ? 00 00 ? 42 00 00 90 40 CD CC 8C 40 1F 85 2B 3F 00 00 00 40", true, true, false)).ToList();

                Base1 = Bases1.FirstOrDefault();
                Base2 = Bases1.LastOrDefault();

                var scanStartNew = Base1 - 0x3000;
                var scanEndNew = Base1 + 0x3000;

                Base3 = (await m.AoBScan(scanStartNew, scanEndNew, "00 00 5C 42 00 ? 80 3F", true, true, false)).FirstOrDefault();

                Bases2 = (await m.AoBScan(scanStartNew, scanEndNew, "00 00 8C 42 ? 99", true, true, false)).ToList();

                Base4 = Bases2.FirstOrDefault();
                Base5 = Bases2.LastOrDefault();

                ChaseMin = Base1.ToString("X");
                ChaseMax = (Base1 + 0x4).ToString("X");
                FarChaseMin = Base2.ToString("X");
                FarChaseMax = (Base2 + 0x4).ToString("X");
                DriverAndDashboardMin = (Base3 - 0x4).ToString("X");
                DriverAndDashboardMax = Base3.ToString("X");
                BumperMin = (Base4 - 0x4).ToString("X");
                BumperMax = Base4.ToString("X");
                HoodMin = (Base5 - 0x4).ToString("X");
                HoodMax = Base5.ToString("X");
                break;
            }
            case 4:
            {
                Base = 0;

                while (Base == 0)
                    Base = (await m.AoBScan(ScanStart, ScanEnd,
                            "00 00 ? ? 00 00 ? 42 00 00 90 40 CD CC 8C 40 1F 85 2B 3F 00 00 00 40", true, true, false))
                        .FirstOrDefault();

                ChaseMin = Base.ToString("X");
                ChaseMax = (Base + 0x4).ToString("X");
                FarChaseMin = (Base + 0x50).ToString("X");
                FarChaseMax = (Base + 0x54).ToString("X");
                DriverAndDashboardMin = (Base - 0x84).ToString("X");
                DriverAndDashboardMax = (Base - 0x80).ToString("X");
                HoodMin = (Base - 0xE4).ToString("X");
                HoodMax = (Base - 0xE0).ToString("X");
                BumperMin = (Base - 0x144).ToString("X");
                BumperMax = (Base - 0x140).ToString("X");
                break;
            }
        }

        NumericBoxes();
    }

    #endregion

    #region Interaction

    private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void ExitButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Environment.Exit(1);
    }

    private void GithubButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Process.Start("explorer.exe", "https://github.com/ForzaMods/Forza-Mods-Fov-Menu");
    }

    private void DiscordButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Process.Start("explorer.exe", "https://discord.gg/2szBrzRTH9");
    }

    private void TopMost_Toggled(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow).Topmost = TopMost.IsOn;
    }

    #endregion

    #region Memory writing

    private void ChaseMinBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        try
        {
            m.WriteMemory(ChaseMin, "float", ChaseMinBox.Value.ToString());
        }
        catch
        {
            MessageBox.Show("Failed to write memory");
        }
    }

    private void ChaseMaxBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        try
        {
            m.WriteMemory(ChaseMax, "float", ChaseMaxBox.Value.ToString());
        }
        catch
        {
            MessageBox.Show("Failed to write memory");
        }
    }

    private void FarChaseMinBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        try
        {
            m.WriteMemory(FarChaseMin, "float", FarChaseMinBox.Value.ToString());
        }
        catch
        {
            MessageBox.Show("Failed to write memory");
        }
    }

    private void FarChaseMaxBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        try
        {
            m.WriteMemory(FarChaseMax, "float", FarChaseMaxBox.Value.ToString());
        }
        catch
        {
            MessageBox.Show("Failed to write memory");
        }
    }

    private void DriverAndDashMinBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        try
        {
            m.WriteMemory(DriverAndDashboardMin, "float", DriverAndDashMinBox.Value.ToString());
        }
        catch
        {
            MessageBox.Show("Failed to write memory");
        }
    }

    private void DriverAndDashMaxBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        try
        {
            m.WriteMemory(DriverAndDashboardMax, "float", DriverAndDashMaxBox.Value.ToString());
        }
        catch
        {
            MessageBox.Show("Failed to write memory");
        }
    }

    private void HoodMinBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        try
        {
            m.WriteMemory(HoodMin, "float", HoodMinBox.Value.ToString());
        }
        catch
        {
            MessageBox.Show("Failed to write memory");
        }
    }

    private void HoodMaxBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        try
        {
            m.WriteMemory(HoodMax, "float", HoodMaxBox.Value.ToString());
        }
        catch
        {
            MessageBox.Show("Failed to write memory");
        }
    }

    private void BumperMinBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        try
        {
            m.WriteMemory(BumperMin, "float", BumperMinBox.Value.ToString());
        }
        catch
        {
            MessageBox.Show("Failed to write memory");
        }
    }

    private void BumperMaxBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        try
        {
            m.WriteMemory(BumperMax, "float", BumperMaxBox.Value.ToString());
        }
        catch
        {
            MessageBox.Show("Failed to write memory");
        }
    }

    #endregion
}