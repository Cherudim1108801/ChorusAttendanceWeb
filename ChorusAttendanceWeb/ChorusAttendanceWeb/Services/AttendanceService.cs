using ChorusAttendanceWeb.Data;
using ChorusAttendanceWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace ChorusAttendanceWeb.Services;

/// <summary>練習に対する出欠情報の取得・更新を担う。</summary>
public class AttendanceService(ApplicationDbContext db)
{
    /// <summary>指定練習に対する全メンバーの出欠を取得する。</summary>
    public async Task<List<Attendance>> GetForPracticeAsync(Guid practiceId, CancellationToken ct = default) =>
        await db.Attendances
            .Include(a => a.User)
            .Where(a => a.PracticeId == practiceId)
            .ToListAsync(ct);

    /// <summary>指定練習・指定メンバーの出欠状態を登録または更新する。</summary>
    public async Task SetStatusAsync(Guid practiceId, string userId, AttendanceStatus status, CancellationToken ct = default)
    {
        var attendance = await db.Attendances
            .FirstOrDefaultAsync(a => a.PracticeId == practiceId && a.UserId == userId, ct);

        if (attendance is null)
        {
            attendance = new Attendance
            {
                Id = Guid.NewGuid(),
                PracticeId = practiceId,
                UserId = userId
            };
            db.Attendances.Add(attendance);
        }

        attendance.Status = status;
        attendance.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }
}
