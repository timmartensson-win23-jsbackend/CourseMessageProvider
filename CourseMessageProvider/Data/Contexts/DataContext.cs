using CourseMessageProvider.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseMessageProvider.Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<JoinCourseRequest> JoinCourseRequests { get; set; }
}
