using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.AppAutistici.Application.Dtos
{
    public record ActivityDto(Guid Id, List<RoutineActionDto> Actions, string Name, bool IsFinished, string Text);
}
