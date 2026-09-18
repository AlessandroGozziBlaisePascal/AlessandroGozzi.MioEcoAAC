using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.AppAAC.Application.Dtos
{
    public record RoutineActionDto(Guid Id, string Image, string Name, string SpeechText, bool IsFinished);
}
