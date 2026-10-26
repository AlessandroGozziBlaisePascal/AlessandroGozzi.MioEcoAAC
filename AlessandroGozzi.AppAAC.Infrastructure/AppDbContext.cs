using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Models;
using SQLite;

namespace AlessandroGozzi.AppAAC.Infrastructure
{
    public class AppDbContext
    {
        public SQLiteAsyncConnection Database { get; }

        public AppDbContext(string dbPath)
        {
            // Inizializza con i flag di creazione e lettura/scrittura corretti
            Database = new SQLiteAsyncConnection(dbPath, DatabaseConstants.Flags);
        }

        public async Task InitializeAsync()
        {
            // 1. Creazione tabelle se non presenti
            await Database.CreateTableAsync<Routine>();
            await Database.CreateTableAsync<Activity>();
            await Database.CreateTableAsync<RoutineAction>();

            // 2. Seeding iniziale solo se il DB è vuoto
            var routineCount = await Database.Table<Routine>().CountAsync();
            if (routineCount == 0)
            {
                await SeedInitialDataAsync();
            }
        }

        private async Task SeedInitialDataAsync()
        {
            var routineId = Guid.NewGuid();
            var activityId = Guid.NewGuid();

            // 1. ROUTINE: Normal Day
            var routine = new Routine
            {
                Id = routineId,
                Name = "Giornata normale",
                SpeechText = "Giornata normale",
                IsDeleted = false
            };

            // 2. ATTIVITÀ: Lavarsi i denti
            var activity = new Activity
            {
                Id = activityId,
                RoutineId = routineId,
                Order = 0,
                Name = "Lavarsi i denti",
                SpeechText = "Ora ci laviamo i denti",
                ImageUrl = "lavare_i_denti.png",
                IsDeleted = false
            };

            // 3. AZIONI
            var actions = new List<RoutineAction>
        {
            new RoutineAction
            {
                Id = Guid.NewGuid(),
                ActivityId = activityId,
                Name = "Prendere lo spazzolino",
                SpeechText = "Prendi lo spazzolino",
                ImageUrl = "spazzolino.png",
                IsFinished = false,
                IsDeleted = false
            },
            new RoutineAction
            {
                Id = Guid.NewGuid(),
                ActivityId = activityId,
                Name = "Mettere il dentifricio",
                SpeechText = "Prendi il dentifricio e mettilo sullo spazzolino",
                ImageUrl = "dentifricio_e_spazzolino.png",
                IsFinished = false,
                IsDeleted = false
            },
            new RoutineAction
            {
                Id = Guid.NewGuid(),
                ActivityId = activityId,
                Name = "Spazzolare i denti",
                SpeechText = "Spazzola bene i denti",
                ImageUrl = "spazzolare_i_denti.png",
                IsFinished = false,
                IsDeleted = false
            },
            new RoutineAction
            {
                Id = Guid.NewGuid(),
                ActivityId = activityId,
                Name = "Sputare",
                SpeechText = "Sputa nel lavandino e sciacqua",
                ImageUrl = "sputare.png",
                IsFinished = false,
                IsDeleted = false
            },
            new RoutineAction
            {
                Id = Guid.NewGuid(),
                ActivityId = activityId,
                Name = "Mettere a posto lo spazzolino",
                SpeechText = "Metti a posto lo spazzolino nel bicchiere",
                ImageUrl = "mettere_a_posto_lo_spazzolino.png",
                IsFinished = false,
                IsDeleted = false
            },
            new RoutineAction
            {
                Id = Guid.NewGuid(),
                ActivityId = activityId,
                Name = "Lavarsi le mani",
                SpeechText = "Lavati bene le mani con il sapone",
                ImageUrl = "lavare_le_mani.png",
                IsFinished = false,
                IsDeleted = false
            }
        };

            await Database.InsertAsync(routine);
            await Database.InsertAsync(activity);
            await Database.InsertAllAsync(actions);
        }
    }
}
