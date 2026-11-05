using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LogicPOS.App.Views;

/// <summary>
/// Single "Período" filter control (qTrack-style): summary field + dual-month popup with Aplicar/Cancelar.
/// </summary>
public partial class PeriodRangeField : UserControl
{
    private static readonly CultureInfo Pt = CultureInfo.GetCultureInfo("pt-PT");

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

    /// <summary>Raised after Aplicar commits a new range (not on Cancelar / light-dismiss).</summary>
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

    /// <summary>Inclusive calendar day of the period start (matches previous CalendarDatePicker.SelectedDate?.Date).</summary>
    public DateTime StartDate => _start.Date;

    /// <summary>Inclusive calendar day of the period end.</summary>
    public DateTime EndDate => _end.Date;

    public void SetRange(DateTime start, DateTime end, bool raise = false)
    {
        Normalize(ref start, ref end);
        if (_start == start && _end == end)
        {
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
        PickerPopup.PlacementTarget = SummaryButton;
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
            RefreshDaySpan();
        }
        finally
        {
            _syncing = false;
        }
    }

    private void RefreshSummary()
    {
        SummaryText.Text = FormatRange(_start, _end);
        ToolTip.SetTip(SummaryButton, SummaryText.Text);
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

    private static void Normalize(ref DateTime start, ref DateTime end)
    {
        if (end < start)
        {
            end = start;
        }
    }

    private static DateTime Combine(DateTime date, TimeSpan time)
        => date.Date.Add(time);

    private static string FormatRange(DateTime start, DateTime end)
        => start.ToString("dd/MM/yyyy HH:mm", Pt) + " - " + end.ToString("dd/MM/yyyy HH:mm", Pt);
}
