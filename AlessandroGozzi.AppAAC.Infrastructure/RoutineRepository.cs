using System.Collections.ObjectModel;
using AlessandroGozzi.AppAAC.Domain.Enums;
using AlessandroGozzi.AppAAC.Domain.Models;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.Infrastructure;
using SQLite;

public class RoutineRepository : IRoutineRepository
{
    private readonly string _dbPath;
    private SQLiteAsyncConnection? _database;

    public RoutineRepository(string dbPath)
    {
        _dbPath = dbPath;
    }

    private async Task InitAsync()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath, DatabaseConstants.Flags);

        await _database.CreateTableAsync<Routine>();
        await _database.CreateTableAsync<Activity>();
        await _database.CreateTableAsync<RoutineAction>();
    }

    // --- QUERIES ---

    public async Task<List<Routine>> GetAllAvailableAsync(CancellationToken cancellationToken = default)
    {
        await InitAsync();

        var routines = await _database!.Table<Routine>()
                                       .Where(r => !r.IsDeleted)
                                       .ToListAsync();

        foreach (var routine in routines)
        {
            var activities = await GetActivitiesByRoutineIdAsync(routine.Id, cancellationToken);
            routine.Activities = new ObservableCollection<Activity>(activities);
        }

        return routines;
    }

    public async Task<Routine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await InitAsync();

        var routine = await _database!.Table<Routine>()
                                      .Where(r => r.Id == id && !r.IsDeleted)
                                      .FirstOrDefaultAsync();

        if (routine != null)
        {
            var activities = await GetActivitiesByRoutineIdAsync(routine.Id, cancellationToken);
            routine.Activities = new ObservableCollection<Activity>(activities);
        }

        return routine;
    }

    public async Task<List<Activity>> GetActivitiesByRoutineIdAsync(Guid routineId, CancellationToken cancellationToken = default)
    {
        await InitAsync();

        var activities = await _database!.Table<Activity>()
                                         .Where(a => a.RoutineId == routineId && !a.IsDeleted)
                                         .OrderBy(a => a.Order)
                                         .ToListAsync();

        foreach (var activity in activities)
        {
            var actions = await _database!.Table<RoutineAction>()
                                          .Where(ra => ra.ActivityId == activity.Id && !ra.IsDeleted)
                                          .ToListAsync();

            activity.Actions = new ObservableCollection<RoutineAction>(actions);
        }

        return activities;
    }

    // --- COMMANDS ---

    public async Task SaveRoutineAsync(Routine routine, CancellationToken cancellationToken = default)
    {
        await InitAsync();

        await _database!.InsertOrReplaceAsync(routine);

        if (routine.Activities != null)
        {
            for (int i = 0; i < routine.Activities.Count; i++)
            {
                var activity = routine.Activities[i];
                activity.RoutineId = routine.Id;
                activity.Order = i;
                await SaveActivityAsync(routine.Id, activity, cancellationToken);
            }
        }
    }

    public async Task DeleteRoutineAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await InitAsync();

        var routine = await _database!.Table<Routine>()
                                      .Where(r => r.Id == id)
                                      .FirstOrDefaultAsync();

        if (routine != null)
        {
            routine.IsDeleted = true;
            await _database!.UpdateAsync(routine);
        }
    }

    // --- ACTIVITIES ---

    public async Task SaveActivityAsync(Guid routineId, Activity activity, CancellationToken cancellationToken = default)
    {
        await InitAsync();

        activity.RoutineId = routineId;
        await _database!.InsertOrReplaceAsync(activity);

        if (activity.Actions != null)
        {
            foreach (var action in activity.Actions)
            {
                action.ActivityId = activity.Id;
                await _database!.InsertOrReplaceAsync(action);
            }
        }
    }

    public async Task DeleteActivityAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        await InitAsync();

        var activity = await _database!.Table<Activity>()
                                       .Where(a => a.Id == activityId)
                                       .FirstOrDefaultAsync();

        if (activity != null)
        {
            activity.IsDeleted = true;
            await _database!.UpdateAsync(activity);
        }
    }

    public async Task UpdateActivitiesOrderAsync(Guid routineId, List<Guid> orderedActivityIds, CancellationToken cancellationToken = default)
    {
        await InitAsync();

        for (int i = 0; i < orderedActivityIds.Count; i++)
        {
            var id = orderedActivityIds[i];
            var activity = await _database!.Table<Activity>()
                                           .Where(a => a.Id == id && a.RoutineId == routineId)
                                           .FirstOrDefaultAsync();

            if (activity != null)
            {
                activity.Order = i;
                await _database!.UpdateAsync(activity);
            }
        }
    }
}