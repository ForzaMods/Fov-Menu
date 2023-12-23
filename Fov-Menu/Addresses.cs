using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Memory;

namespace Fov_Menu;

public class Addresses
{
    private readonly MainWindow _mainWindow;
    private readonly Mem _mem = new()
    {
        SigScanTasks = Environment.ProcessorCount * (Environment.ProcessorCount / 2)
    };

    private bool _attached;
    
    private UIntPtr _chaseMin;
    private UIntPtr _chaseMax;
    private UIntPtr _farChaseMin;
    private UIntPtr _farChaseMax;
    private UIntPtr _driverMin;
    private UIntPtr _driverMax;
    private UIntPtr _hoodMin;
    private UIntPtr _hoodMax;
    private UIntPtr _bumperMin;
    private UIntPtr _bumperMax;

    public Addresses(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }
    
    public void OpenGameProcess()
    {
        while (true)
        {
            Task.Delay(_attached ? 1000 : 500).Wait();
            var isGameProcessOpened = _mem.OpenProcess("ForzaHorizon5") || _mem.OpenProcess("ForzaHorizon4");

            switch (_attached)
            {
                case false when isGameProcessOpened:
                    AddressesScan();
                    _attached = true;
                    break;
                case true when !isGameProcessOpened:
                    _attached = false;
                    break;
            }
        }
    }

    private void AddressesScan()
    {
        var bases1 = _mem.ScanForSig("90 40 CD CC 8C 40 1F 85 2B 3F 00 00 00 40").ToList();
        var bases2 = _mem.ScanForSig("CD CC 4C 3E 00 50 43 47 00 00 34 42 00 00 20").ToList();
        var base3 = _mem.ScanForSig("CD ? 4C 3E ? ? ? 47 00 ? 34 ? 00 00 20 42 ? 00 A0").FirstOrDefault() - 0x20;
        _mem._memoryCache.Clear();
        
        _chaseMin = bases1.FirstOrDefault() - 10;
        _chaseMax = bases1.FirstOrDefault() - 10 + 4;
        _farChaseMin = bases1.LastOrDefault() - 10;
        _farChaseMax = bases1.LastOrDefault() - 10 + 4;
        _driverMin = base3 - 4;
        _driverMax = base3;
        _bumperMin = bases2.FirstOrDefault() - 0x20 - 4;
        _bumperMax = bases2.FirstOrDefault() - 0x20;
        _hoodMin = bases2.LastOrDefault() - 0x20 - 4;
        _hoodMax = bases2.LastOrDefault() - 0x20;

        
        ReadValues();
    }

    private void ReadValues()
    {
        _mainWindow.Dispatcher.BeginInvoke((Action)delegate
        {
            _mainWindow.ChaseMin.Value = Convert.ToDouble(_mem.ReadMemory<float>(_chaseMin));
            _mainWindow.ChaseMax.Value = Convert.ToDouble(_mem.ReadMemory<float>(_chaseMax));
            _mainWindow.FarChaseMin.Value = Convert.ToDouble(_mem.ReadMemory<float>(_farChaseMin));
            _mainWindow.FarChaseMax.Value = Convert.ToDouble(_mem.ReadMemory<float>(_farChaseMax));
            _mainWindow.DriverMin.Value = Convert.ToDouble(_mem.ReadMemory<float>(_driverMin));
            _mainWindow.DriverMax.Value = Convert.ToDouble(_mem.ReadMemory<float>(_driverMax));
            _mainWindow.HoodMin.Value = Convert.ToDouble(_mem.ReadMemory<float>(_hoodMin));
            _mainWindow.HoodMax.Value = Convert.ToDouble(_mem.ReadMemory<float>(_hoodMax));
            _mainWindow.BumperMin.Value = Convert.ToDouble(_mem.ReadMemory<float>(_bumperMin));
            _mainWindow.BumperMax.Value = Convert.ToDouble(_mem.ReadMemory<float>(_bumperMax));
        });
    }
        

    public void WriteValue(string buttonName, float value)
    {
        if (!_attached)
        {
            return;
        }

        var fields = typeof(Addresses).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var address =
            (from field in fields.Where(f => f.FieldType == typeof(UIntPtr))
                where field.Name.ToLower().Contains(buttonName.ToLower())
                select (UIntPtr)(field.GetValue(this) ?? UIntPtr.Zero)).FirstOrDefault();

        if (address == 0) return;
        _mem.WriteMemory(address, value);
    }
}