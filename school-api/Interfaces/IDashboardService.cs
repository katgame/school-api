using school_api.Data.Models;

namespace school_api.Interfaces
{
    public interface IDashboardService
    {
        List<School> GetDashboard();
        public School GetSchoolDetails(Guid schoolId);
        public bool CreateSchool(School schoolDetails);
        public bool UpdateSchool(School request);

    }
}
