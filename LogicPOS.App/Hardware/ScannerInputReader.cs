using Avalonia.Input;
using Avalonia.Threading;
using Microsoft.Extensions.Configuration;

namespace LogicPOS.App.Hardware;

public enum ScannerDevice
{
    None,
    BarCodeReader,
    CardReader
}

public sealed class ScannerCapturedEventArgs : EventArgs
{
    public required string Code { get; init; }

    public required ScannerDevice Device { get; init; }
}

/// <summary>
/// Keyboard-wedge barcode/card reader capture (GTK InputReader equivalent).
/// Buffers rapid keystrokes and classifies by configured lengths after a short idle,
/// or immediately when Enter is received (common HID scanner suffix).
/// </summary>
public sealed class ScannerInputReader
{
    private readonly Dictionary<int, ScannerDevice> _sizes;
    private readonly DispatcherTimer _timer;
    private string _buffer = string.Empty;
    private bool _running;

    public ScannerInputReader(int timerMs, IEnumerable<int> barcodeSizes, IEnumerable<int> cardSizes)
    {
        _sizes = new Dictionary<int, ScannerDevice>();
        foreach (var size in barcodeSizes.Where(item => item > 0).Distinct())
        {
            _sizes[size] = ScannerDevice.BarCodeReader;
        }

        foreach (var size in cardSizes.Where(item => item > 0).Distinct())
        {
            if (_sizes.ContainsKey(size) == false)
            {
                _sizes[size] = ScannerDevice.CardReader;
            }
        }

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(Math.Clamp(timerMs, 40, 1000))
        };
        _timer.Tick += (_, _) => Flush(fromEnter: false);
    }

    public event EventHandler<ScannerCapturedEventArgs>? Captured;

    public static ScannerInputReader FromConfiguration(IConfiguration? configuration)
    {
        var section = configuration?.GetSection("LogicPOS:Scanner");
        var timerMs = section?.GetValue("TimerMs", 200) ?? 200;
        var barcode = ParseSizes(section?["BarcodeSizes"] ?? "8,9,12,13,14,18");
        var card = ParseSizes(section?["CardSizes"] ?? "7,10");
        return new ScannerInputReader(timerMs, barcode, card);
    }

    public bool TryHandleKey(Key key, KeyModifiers modifiers, out bool handled)
    {
        handled = false;
        if ((modifiers & ~(KeyModifiers.Shift)) != KeyModifiers.None)
        {
            return false;
        }

        if (key is Key.Enter or Key.Return)
        {
            if (_buffer.Length == 0)
            {
                return false;
            }

            Flush(fromEnter: true);
            handled = true;
            return true;
        }

        if (TryMapChar(key, out var character) == false)
        {
            return false;
        }

        if (_running == false)
        {
            _running = true;
            _timer.Start();
        }
        else
        {
            _timer.Stop();
            _timer.Start();
        }

        _buffer += character;
        handled = true;
        return true;
    }

    public void Reset()
    {
        _timer.Stop();
        _running = false;
        _buffer = string.Empty;
    }

    private void Flush(bool fromEnter)
    {
        _timer.Stop();
        _running = false;

        var text = _buffer.Trim();
        _buffer = string.Empty;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var device = ScannerDevice.None;
        if (_sizes.TryGetValue(text.Length, out var matched))
        {
            device = matched;
        }
        else if (fromEnter && text.Length >= 4)
        {
            device = ScannerDevice.BarCodeReader;
        }
        else if (_sizes.Count == 0 && text.Length >= 4)
        {
            device = ScannerDevice.BarCodeReader;
        }

        if (device == ScannerDevice.None)
        {
            return;
        }

        Captured?.Invoke(this, new ScannerCapturedEventArgs
        {
            Code = text,
            Device = device
        });
    }

    private static IEnumerable<int> ParseSizes(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(item => int.TryParse(item, out var size) ? size : 0)
            .Where(size => size > 0);
    }

    private static bool TryMapChar(Key key, out char character)
    {
        character = '\0';
        if (key is >= Key.D0 and <= Key.D9)
        {
            character = (char)('0' + (key - Key.D0));
            return true;
        }

        if (key is >= Key.NumPad0 and <= Key.NumPad9)
        {
            character = (char)('0' + (key - Key.NumPad0));
            return true;
        }

        if (key is >= Key.A and <= Key.Z)
        {
            character = (char)('A' + (key - Key.A));
            return true;
        }

        character = key switch
        {
            Key.OemMinus or Key.Subtract => '-',
            Key.OemPlus => '+',
            Key.OemPeriod or Key.Decimal => '.',
            Key.OemComma => ',',
            Key.OemQuestion => '/',
            Key.OemPipe => '|',
            Key.Space => ' ',
            _ => '\0'
        };
        return character != '\0';
    }
}
