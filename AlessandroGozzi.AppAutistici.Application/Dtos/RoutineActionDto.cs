using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.AppAutistici.Application.Dtos
{
    public record RoutineActionDto(Guid Id, string Image, string Description, string Audio, bool IsFinished, Guid ActivityId);
}
