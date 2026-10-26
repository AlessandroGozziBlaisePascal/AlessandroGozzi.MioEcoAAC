using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Dtos;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Queries.GetActionsById
{
    public record GetActionsByRoutineIdQuery(Guid RoutineId) : IRequest<List<ActivityDto>>;
}
