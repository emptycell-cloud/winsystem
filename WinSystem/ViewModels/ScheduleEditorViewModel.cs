using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>日程编辑表单：新增/编辑日程（sys_schedule）。</summary>
    public class ScheduleEditorViewModel : EditorFormViewModel
    {
        private bool _isNew;
        private string _title;
        private DateTime? _scheduleDate;
        private string _startTime;
        private string _endTime;
        private string _location;
        private string _description;
        private bool _isCompleted;

        public ScheduleEditorViewModel(SysSchedule? schedule)
        {
            _isNew = schedule == null;
            _title = schedule?.Title ?? "";
            _scheduleDate = schedule?.ScheduleDate ?? DateTime.Today;
            _startTime = schedule?.StartTime ?? "";
            _endTime = schedule?.EndTime ?? "";
            _location = schedule?.Location ?? "";
            _description = schedule?.Description ?? "";
            _isCompleted = schedule?.IsCompleted == 1;
            Title = _isNew ? "新增日程" : "编辑日程";
        }

        public static ScheduleEditorViewModel CreateNew() => new(null);

        public string ScheduleTitle { get => _title; set => SetProperty(ref _title, value); }
        public DateTime? ScheduleDate { get => _scheduleDate; set => SetProperty(ref _scheduleDate, value); }
        public string StartTime { get => _startTime; set => SetProperty(ref _startTime, value); }
        public string EndTime { get => _endTime; set => SetProperty(ref _endTime, value); }
        public string Location { get => _location; set => SetProperty(ref _location, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        public bool IsCompleted { get => _isCompleted; set => SetProperty(ref _isCompleted, value); }

        public bool IsNew => _isNew;

        public SysSchedule BuildEntity() => new()
        {
            Title = ScheduleTitle.Trim(),
            ScheduleDate = ScheduleDate ?? DateTime.Today,
            StartTime = string.IsNullOrWhiteSpace(StartTime) ? null : StartTime.Trim(),
            EndTime = string.IsNullOrWhiteSpace(EndTime) ? null : EndTime.Trim(),
            Location = Location.Trim(),
            Description = Description.Trim(),
            IsCompleted = IsCompleted ? 1 : 0,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public void ApplyTo(SysSchedule schedule)
        {
            schedule.Title = ScheduleTitle.Trim();
            schedule.ScheduleDate = ScheduleDate ?? DateTime.Today;
            schedule.StartTime = string.IsNullOrWhiteSpace(StartTime) ? null : StartTime.Trim();
            schedule.EndTime = string.IsNullOrWhiteSpace(EndTime) ? null : EndTime.Trim();
            schedule.Location = Location.Trim();
            schedule.Description = Description.Trim();
            schedule.IsCompleted = IsCompleted ? 1 : 0;
            schedule.UpdatedAt = DateTime.Now;
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(ScheduleTitle)) { Message = "请填写日程标题。"; return false; }
            if (ScheduleDate == null) { Message = "请选择日程日期。"; return false; }
            if (!string.IsNullOrWhiteSpace(StartTime) && !string.IsNullOrWhiteSpace(EndTime) &&
                TimeSpan.TryParse(StartTime, out var st) && TimeSpan.TryParse(EndTime, out var et) && et < st)
            {
                Message = "结束时间不能早于开始时间。";
                return false;
            }
            Message = "";
            return true;
        }
    }
}
