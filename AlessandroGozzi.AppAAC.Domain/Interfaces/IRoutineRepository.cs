using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Models;
using Activity = AlessandroGozzi.AppAAC.Domain.Models.Activity;

namespace AlessandroGozzi.AppAAC.Domain.Interfaces
{
    public interface IRoutineRepository
    {
        // --- QUERIES ---
        Task<List<Routine>> GetAllAvailableAsync(CancellationToken cancellationToken = default);
        Task<Routine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Activity>> GetActivitiesByRoutineIdAsync(Guid routineId, CancellationToken cancellationToken = default);

        // --- COMMANDS ---
        Task SaveRoutineAsync(Routine routine, CancellationToken cancellationToken = default);
        Task DeleteRoutineAsync(Guid id, CancellationToken cancellationToken = default);

        // --- ACTIVITIES ---
        Task SaveActivityAsync(Guid routineId, Activity activity, CancellationToken cancellationToken = default);
        Task DeleteActivityAsync(Guid activityId, CancellationToken cancellationToken = default);
        Task UpdateActivitiesOrderAsync(Guid routineId, List<Guid> orderedActivityIds, CancellationToken cancellationToken = default);

        // --- ACTIONS ---
        Task SaveActionAsync(Guid activityId, RoutineAction action);
        Task<List<RoutineAction>> GetActionsByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default);
    }
}
