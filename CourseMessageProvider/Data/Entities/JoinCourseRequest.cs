using System.ComponentModel.DataAnnotations;

namespace CourseMessageProvider.Data.Entities;

public class JoinCourseRequest
{
    [Key]
    public string Email { get; set; } = null!;
    public string CourseName { get; set; } = null!;
}
