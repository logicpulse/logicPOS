using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace LogicPOS.App.Views;

/// <summary>
/// "Período" filter: editable start/end fields + dual-month calendar popup.
/// </summary>
public partial class PeriodRangeField : UserControl
{
    private static readonly CultureInfo Pt = CultureInfo.GetCultureInfo("pt-PT");
    private static readonly string[] Formats =
    [
        "dd/MM/yyyy HH:mm",
        "dd/MM/yyyy H:mm",
        "dd/MM/yyyy",
        "d/M/yyyy HH:mm",
        "d/M/yyyy H:mm",
        "d/M/yyyy"
    ];

    private DateTime _start;
    private DateTime _end;
    private DateTime _draftStart;
    private DateTime _draftEnd;
    private bool _pickingEnd;
    private bool _syncing;
    private bool _opened;

    public PeriodRangeField()
    {
        InitializeComponent();
        var today = DateTime.Today;
        _start = new DateTime(today.Year, 1, 1, 0, 0, 0);
        _end = today.AddDays(1).AddSeconds(-1);
        _draftStart = _start;
        _draftEnd = _end;
        RefreshSummary();
    }

    /// <summary>Raised after a committed range change (Aplicar or typed dates).</summary>
    public event EventHandler? PeriodChanged;

    /// <summary>When false, the built-in "Período" caption is hidden (host supplies its own label).</summary>
    public bool ShowCaption
    {
        get => Caption.IsVisible;
        set => Caption.IsVisible = value;
    }

    public DateTime Start
    {
        get => _start;
        set => SetRange(value, _end, raise: false);
    }

    public DateTime End
    {
        get => _end;
        set => SetRange(_start, value, raise: false);
    }

    /// <summary>Inclusive calendar day of the period start.</summary>
    public DateTime StartDate => _start.Date;

    /// <summary>Inclusive calendar day of the period end.</summary>
    public DateTime EndDate => _end.Date;

    public void SetRange(DateTime start, DateTime end, bool raise = false)
    {
        Normalize(ref start, ref end);
        if (_start == start && _end == end)
        {
            RefreshSummary();
            if (raise)
            {
                PeriodChanged?.Invoke(this, EventArgs.Empty);
            }

            return;
        }

        _start = start;
        _end = end;
        RefreshSummary();
        if (raise)
        {
            PeriodChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnSummaryClick(object? sender, RoutedEventArgs e)
    {
        if (PickerPopup.IsOpen)
        {
            PickerPopup.IsOpen = false;
            return;
        }

        OpenDraft();
        PickerPopup.PlacementTarget = CalendarButton;
        PickerPopup.IsOpen = true;
        _opened = true;
    }

    private void OnPopupClosed(object? sender, EventArgs e)
    {
        _opened = false;
        _pickingEnd = false;
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        PickerPopup.IsOpen = false;
    }

    private void OnApplyClick(object? sender, RoutedEventArgs e)
    {
        CommitDraftTimes();
        Normalize(ref _draftStart, ref _draftEnd);
        SetRange(_draftStart, _draftEnd, raise: true);
        PickerPopup.IsOpen = false;
    }

    private void OnStartLostFocus(object? sender, RoutedEventArgs e) => TryCommitTyped(StartBox, isStart: true, raise: true);

    private void OnEndLostFocus(object? sender, RoutedEventArgs e) => TryCommitTyped(EndBox, isStart: false, raise: true);

    private void OnStartKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            TryCommitTyped(StartBox, isStart: true, raise: true);
            e.Handled = true;
        }
    }

    private void OnEndKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            TryCommitTyped(EndBox, isStart: false, raise: true);
            e.Handled = true;
        }
    }

    private void OnDraftStartLostFocus(object? sender, RoutedEventArgs e) => TryCommitDraftTyped(DraftStartBox, isStart: true);

    private void OnDraftEndLostFocus(object? sender, RoutedEventArgs e) => TryCommitDraftTyped(DraftEndBox, isStart: false);

    private void OnDraftStartKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            TryCommitDraftTyped(DraftStartBox, isStart: true);
            e.Handled = true;
        }
    }

    private void OnDraftEndKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            TryCommitDraftTyped(DraftEndBox, isStart: false);
            e.Handled = true;
        }
    }

    private void OnLeftCalendarChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_syncing || _opened == false)
        {
            return;
        }

        var day = LeftCalendar.SelectedDate;
        if (day is null)
        {
            return;
        }

        ApplyCalendarPick(day.Value.Date);
    }

    private void OnRightCalendarChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_syncing || _opened == false)
        {
            return;
        }

        var day = RightCalendar.SelectedDate;
        if (day is null)
        {
            return;
        }

        ApplyCalendarPick(day.Value.Date);
    }

    private void OnDraftTimeChanged(object? sender, TimePickerSelectedValueChangedEventArgs e)
    {
        if (_syncing || _opened == false)
        {
            return;
        }

        CommitDraftTimes();
        RefreshDraftBoxes();
        RefreshDaySpan();
    }

    private void OpenDraft()
    {
        _draftStart = _start;
        _draftEnd = _end;
        _pickingEnd = false;
        SyncDraftUi();
    }

    private void ApplyCalendarPick(DateTime day)
    {
        if (_pickingEnd == false)
        {
            _draftStart = Combine(day, _draftStart.TimeOfDay);
            _draftEnd = Combine(day, _draftEnd.TimeOfDay);
            _pickingEnd = true;
        }
        else
        {
            _draftEnd = Combine(day, _draftEnd.TimeOfDay);
            if (_draftEnd.Date < _draftStart.Date)
            {
                (_draftStart, _draftEnd) = (
                    Combine(_draftEnd.Date, _draftStart.TimeOfDay),
                    Combine(_draftStart.Date, _draftEnd.TimeOfDay));
            }

            _pickingEnd = false;
        }

        SyncDraftUi();
    }

    private void CommitDraftTimes()
    {
        var startTime = StartTimePicker.SelectedTime ?? TimeSpan.Zero;
        var endTime = EndTimePicker.SelectedTime ?? new TimeSpan(23, 59, 0);
        _draftStart = Combine(_draftStart.Date, startTime);
        _draftEnd = Combine(_draftEnd.Date, endTime);
        if (_draftEnd < _draftStart)
        {
            _draftEnd = Combine(_draftEnd.Date, startTime);
            if (_draftEnd < _draftStart)
            {
                _draftEnd = _draftStart;
            }
        }
    }

    private void SyncDraftUi()
    {
        _syncing = true;
        try
        {
            LeftCalendar.SelectedDate = _draftStart.Date;
            RightCalendar.SelectedDate = _draftEnd.Date;
            LeftCalendar.DisplayDate = new DateTime(_draftStart.Year, _draftStart.Month, 1);
            var rightMonth = new DateTime(_draftStart.Year, _draftStart.Month, 1).AddMonths(1);
            if (_draftEnd.Year > _draftStart.Year
                || (_draftEnd.Year == _draftStart.Year && _draftEnd.Month > _draftStart.Month))
            {
                rightMonth = new DateTime(_draftEnd.Year, _draftEnd.Month, 1);
            }

            RightCalendar.DisplayDate = rightMonth;
            StartTimePicker.SelectedTime = _draftStart.TimeOfDay;
            EndTimePicker.SelectedTime = _draftEnd.TimeOfDay;
            RefreshDraftBoxes();
            RefreshDaySpan();
        }
        finally
        {
            _syncing = false;
        }
    }

    private void RefreshSummary()
    {
        if (_syncing)
        {
            return;
        }

        _syncing = true;
        try
        {
            StartBox.Text = FormatDateTime(_start);
            EndBox.Text = FormatDateTime(_end);
            ToolTip.SetTip(StartBox, StartBox.Text);
            ToolTip.SetTip(EndBox, EndBox.Text);
        }
        finally
        {
            _syncing = false;
        }
    }

    private void RefreshDraftBoxes()
    {
        DraftStartBox.Text = FormatDateTime(_draftStart);
        DraftEndBox.Text = FormatDateTime(_draftEnd);
    }

    private void RefreshDaySpan()
    {
        var days = (_draftEnd.Date - _draftStart.Date).Days + 1;
        if (days < 1)
        {
            days = 1;
        }

        DaySpanLabel.Text = days == 1 ? "1 Dia" : $"{days} Dias";
    }

    private void TryCommitTyped(TextBox box, bool isStart, bool raise)
    {
        if (_syncing)
        {
            return;
        }

        if (TryParseDateTime(box.Text, out var value) == false)
        {
            RefreshSummary();
            return;
        }

        if (isStart == false && box.Text is { } text && LooksDateOnly(text))
        {
            value = value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        var start = isStart ? value : _start;
        var end = isStart ? _end : value;
        SetRange(start, end, raise);
    }

    private void TryCommitDraftTyped(TextBox box, bool isStart)
    {
        if (_syncing || _opened == false)
        {
            return;
        }

        if (TryParseDateTime(box.Text, out var value) == false)
        {
            RefreshDraftBoxes();
            return;
        }

        if (isStart == false && box.Text is { } text && LooksDateOnly(text))
        {
            value = value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        if (isStart)
        {
            _draftStart = value;
        }
        else
        {
            _draftEnd = value;
        }

        Normalize(ref _draftStart, ref _draftEnd);
        SyncDraftUi();
    }

    private static bool LooksDateOnly(string text)
    {
        var trimmed = text.Trim();
        return trimmed.Contains(':') == false && trimmed.Contains(' ') == false;
    }

    private static bool TryParseDateTime(string? text, out DateTime value)
    {
        value = default;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return DateTime.TryParseExact(
            text.Trim(),
            Formats,
            Pt,
            DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
            out value);
    }

    private static void Normalize(ref DateTime start, ref DateTime end)
    {
        if (end < start)
        {
            end = start;
        }
    }

    private static DateTime Combine(DateTime date, TimeSpan time)
        => date.Date.Add(time);

    private static string FormatDateTime(DateTime value)
        => value.ToString("dd/MM/yyyy HH:mm", Pt);
}
