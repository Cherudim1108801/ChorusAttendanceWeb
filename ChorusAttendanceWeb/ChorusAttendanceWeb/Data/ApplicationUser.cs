using ChorusAttendanceWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace ChorusAttendanceWeb.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        /// <summary>表示名。オンボーディング完了までは null。</summary>
        public string? DisplayName { get; set; }

        /// <summary>所属パート。オンボーディング完了までは null。</summary>
        public PartType? Part { get; set; }

        /// <summary>プロフィール最終更新日時（UTC）。</summary>
        public DateTime? ProfileUpdatedAt { get; set; }

        /// <summary>名前・パートの登録が完了しているかどうか。</summary>
        public bool IsProfileComplete => !string.IsNullOrWhiteSpace(DisplayName) && Part is not null;
    }

}
