using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.AppAAC.Application.Dtos
{
    public record RoutineSummaryDto(
    Guid Id,
    string Name,
    string Text,
    string Type
);
}
