using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.AppAutistici.Application.Dtos
{
    public record RoutineDto(Guid Id, string Name, List<RoutineActionDto> Actions, bool IsFinished, string Text);
}