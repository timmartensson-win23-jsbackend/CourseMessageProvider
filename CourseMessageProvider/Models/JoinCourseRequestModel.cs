using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseMessageProvider.Models;

public class JoinCourseRequestModel
{
    public string Email { get; set; } = null!;
    public string CourseName { get; set; } = null!;
}
