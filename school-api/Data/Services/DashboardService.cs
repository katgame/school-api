using school_api.Data;
using school_api.Data.Dto;
using school_api.Data.Models;
using school_api.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace school_api.Data.Services
{
    public class DashboardService : IDashboardService
    {
        private AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public bool CreateSchool(School schoolDetails)
        {
            schoolDetails.Id = Guid.NewGuid();
            schoolDetails.UpdatedDate = DateTime.Now;
            _context.School.Add(schoolDetails);
            _context.SaveChanges();
            return true;
        }

        public List<School> GetDashboard() => _context.School.Where(x => x.Active).OrderBy(x => x.Price).ToList();

        public School GetSchoolDetails(Guid schoolId) => _context.School.FirstOrDefault(x => x.Id == schoolId);

        public bool UpdateSchool(School request)
        {
            request.UpdatedDate  = DateTime.Now;
            _context.School.Update(request);
            _context.SaveChanges();
            return true;
        }
    }
}
