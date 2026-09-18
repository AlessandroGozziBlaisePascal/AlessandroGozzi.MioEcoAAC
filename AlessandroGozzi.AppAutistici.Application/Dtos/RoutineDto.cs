using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.AppAAC.Application.Dtos
{
    public record RoutineDto(Guid Id, string Image, string Name, List<ActivityDto> Actions, bool IsFinished, string SpeechText);
}