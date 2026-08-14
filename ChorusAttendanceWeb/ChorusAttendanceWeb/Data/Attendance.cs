using ChorusAttendanceWeb.Models;

namespace ChorusAttendanceWeb.Data;

/// <summary>ある練習に対する、あるメンバー1人分の出欠。</summary>
public class Attendance
{
    public Guid Id { get; set; }

    public Guid PracticeId { get; set; }
    public Practice? Practice { get; set; }

    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public AttendanceStatus Status { get; set; }

    public DateTime UpdatedAt { get; set; }
}
