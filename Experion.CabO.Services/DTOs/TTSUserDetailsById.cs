using System.Collections.Generic;

namespace Experion.CabO.Services.DTOs
{
    public class TTSUserDetailsById
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public byte status { get; set; }
        public bool management { get; set; }
        public bool isActive { get; set; }
        public string sunOpr { get; set; }
        public string colorTmpl { get; set; }
        public string defaultCountry { get; set; }
        public string area { get; set; }
        public string datacashLogin { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string roleId { get; set; }
        public string mobileNo { get; set; }
        public string employeeId { get; set; }
        public string profileImage { get; set; }
        public string managerId { get; set; }
        public string fullName { get; set; }
        public string businessUnitId { get; set; }
        public bool excludeFromCosting { get; set; }
        public string joiningDate { get; set; }
        public string relievingDate { get; set; }
        public string jiraUserName { get; set; }
        public string designationId { get; set; }
        public bool shortLogImmune { get; set; }
        public TTSBusinessUnit businessUnit { get; set; }
        public IEnumerable<string> mtdashboard { get; set; }
        public IEnumerable<string> mtpermission { get; set; }
        public IEnumerable<string> mtuserAllocation { get; set; }
        public IEnumerable<string> mtuserSkill { get; set; }
        public IEnumerable<string> projectMembers { get; set; }
        public IEnumerable<string> timesheets { get; set; }
        public DesignationDto designation { get; set; }
    }
    public class TTSDetailById
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string employeeId { get; set; }
        public string mobileNo { get; set; }
        public DesignationDto designation { get; set; }
        public string managerId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }

    }
}
