namespace Experion.CabO.Services.DTOs
{
    public class TTSProjectsByUserId
    {
        public int projectMemberId { get; set; }
        public int projectId { get; set; }
        public int userId { get; set; }
        public int roleId { get; set; }
        public bool isActive { get; set; }
        public TTSProject project { get; set; }
        public TTSRole role { get; set; }
        public TTSUser user { get; set; }
    }
}
