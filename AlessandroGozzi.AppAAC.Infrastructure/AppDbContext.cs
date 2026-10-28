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
            var activityIdDenti = Guid.NewGuid();
            var activityIdDipingere = Guid.NewGuid();

            // 1. ROUTINE: Giornata normale
            var routine = new Routine
            {
                Id = routineId,
                Name = "Giornata normale",
                SpeechText = "Giornata normale",
                ImageUrl = "settimana.png",
                IsDeleted = false
            };

            var allActivities = new List<Activity>();
            var allActions = new List<RoutineAction>();

            // Helper per generare le azioni dei denti
            List<RoutineAction> CreateActionsDenti(Guid activityId) => new()
{
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = activityId, Order = 0, Name = "Prendere lo spazzolino", SpeechText = "Prendi lo spazzolino", ImageUrl = "spazzolino.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = activityId, Order = 1, Name = "Mettere il dentifricio", SpeechText = "Prendi il dentifricio e mettilo sullo spazzolino", ImageUrl = "dentifricio_e_spazzolino.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = activityId, Order = 2, Name = "Spazzolare i denti", SpeechText = "Spazzola bene i denti", ImageUrl = "spazzolare_i_denti.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = activityId, Order = 3, Name = "Sputare", SpeechText = "Sputa nel lavandino e sciacqua", ImageUrl = "sputare.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = activityId, Order = 4, Name = "Mettere a posto lo spazzolino", SpeechText = "Metti a posto lo spazzolino nel bicchiere", ImageUrl = "mettere_a_posto_lo_spazzolino.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = activityId, Order = 5, Name = "Lavarsi le mani", SpeechText = "Lavati bene le mani con il sapone", ImageUrl = "lavare_le_mani.png", IsFinished = false, IsDeleted = false }
            };

            // Helper per generare le azioni del mangiare
            List<RoutineAction> CreateActionsMangiare(Guid activityId, string speechPasto) => new()
            {
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = activityId, Order = 0, Name = "Prendere la forchetta", SpeechText = "Prendi la forchetta", ImageUrl = "impugnare_la_forchetta.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = activityId, Order = 1, Name = "Mangiare", SpeechText = speechPasto, ImageUrl = "mangiare.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = activityId, Order = 2, Name = "Non giocare con il cibo", SpeechText = "Non si gioca con il cibo", ImageUrl = "no_giocre_con_cibo.jpg", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = activityId, Order = 3, Name = "Finire il piatto", SpeechText = "Mangia tutto fino a finire il piatto", ImageUrl = "piatto.png", IsFinished = false, IsDeleted = false }
            };

            // --- ATTIVITÀ 0: Lavarsi i denti (Mattina) ---
            var actDentiMattinaId = Guid.NewGuid();
            allActivities.Add(new Activity
            {
                Id = actDentiMattinaId,
                RoutineId = routineId,
                Order = 0,
                Name = "Lavarsi i denti",
                SpeechText = "Ora ci laviamo i denti",
                ImageUrl = "lavare_i_denti.png",
                IsDeleted = false
            });
            allActions.AddRange(CreateActionsDenti(actDentiMattinaId));

            // --- ATTIVITÀ 1: Andare a scuola ---
            var actScuolaId = Guid.NewGuid();
            allActivities.Add(new Activity
            {
                Id = actScuolaId,
                RoutineId = routineId,
                Order = 1,
                Name = "Andare a scuola",
                SpeechText = "Ora andiamo a scuola",
                ImageUrl = "scuola.png",
                IsDeleted = false
            });
            allActions.AddRange(new[]
            {
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actScuolaId, Order = 0, Name = "Andare da mamma", SpeechText = "Vai dalla mamma", ImageUrl = "mamma.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actScuolaId, Order = 1, Name = "Andare in macchina", SpeechText = "Saliamo in macchina", ImageUrl = "macchina.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actScuolaId, Order = 2, Name = "Andare dalla maestra", SpeechText = "Entra e vai dalla maestra", ImageUrl = "maestra.png", IsFinished = false, IsDeleted = false }
            });

            // --- ATTIVITÀ 2: Mangiare (Pranzo) ---
            var actPranzoId = Guid.NewGuid();
            allActivities.Add(new Activity
            {
                Id = actPranzoId,
                RoutineId = routineId,
                Order = 2,
                Name = "Mangiare",
                SpeechText = "È ora di pranzo, andiamo a mangiare",
                ImageUrl = "pranzo.png",
                IsDeleted = false
            });
            allActions.AddRange(CreateActionsMangiare(actPranzoId, "Mangia il pranzo"));

            // --- ATTIVITÀ 3: Tablet ---
            var actTabletId = Guid.NewGuid();
            allActivities.Add(new Activity
            {
                Id = actTabletId,
                RoutineId = routineId,
                Order = 3,
                Name = "Tablet",
                SpeechText = "Ora puoi usare il tablet",
                ImageUrl = "tablet.png",
                IsDeleted = false
            });
            allActions.Add(new RoutineAction
            {
                Id = Guid.NewGuid(),
                ActivityId = actTabletId,
                Order = 0,
                Name = "Guardare video",
                SpeechText = "Guarda il tuo video",
                ImageUrl = "video_tablet.jpg",
                IsFinished = false,
                IsDeleted = false
            });

            // --- ATTIVITÀ 4: Dipingere ---
            var actDipingereId = Guid.NewGuid();
            allActivities.Add(new Activity
            {
                Id = actDipingereId,
                RoutineId = routineId,
                Order = 4,
                Name = "Dipingere",
                SpeechText = "Ora andiamo a dipingere",
                ImageUrl = "dipingi.jpg",
                IsDeleted = false
            });
            allActions.AddRange(new[]
            {
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actDipingereId, Order = 0, Name = "Prendi pennello e colori", SpeechText = "Prendi il pennello e i colori", ImageUrl = "colori_ad_olio.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actDipingereId, Order = 1, Name = "Colora il pennello", SpeechText = "Intingi il pennello nel colore", ImageUrl = "intingi_pennello.jpg", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actDipingereId, Order = 2, Name = "Disegna", SpeechText = "Ora disegna sulla tela", ImageUrl = "dipingi.jpg", IsFinished = false, IsDeleted = false }
            });

            // --- ATTIVITÀ 5: Uscire al parco ---
            var actParcoId = Guid.NewGuid();
            allActivities.Add(new Activity
            {
                Id = actParcoId,
                RoutineId = routineId,
                Order = 5,
                Name = "Uscire al parco",
                SpeechText = "Ora usciamo e andiamo al parco",
                ImageUrl = "parco.png",
                IsDeleted = false
            });
            allActions.AddRange(new[]
            {
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actParcoId, Order = 0, Name = "Chiedere alla mamma", SpeechText = "Chiedi alla mamma", ImageUrl = "mamma.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actParcoId, Order = 1, Name = "Correre al parco", SpeechText = "Corri nel parco", ImageUrl = "correre.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actParcoId, Order = 2, Name = "Dondolare sull'altalena", SpeechText = "Dondolati sull'altalena", ImageUrl = "spingerrsi_con_altalena.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actParcoId, Order = 3, Name = "Scivolare", SpeechText = "Scendi dallo scivolo", ImageUrl = "scivolo.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actParcoId, Order = 4, Name = "Chiedere alla mamma di tornare", SpeechText = "Chiedi alla mamma di tornare a casa", ImageUrl = "mamma.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actParcoId, Order = 5, Name = "Tornare a casa", SpeechText = "Torniamo a casa", ImageUrl = "arrivare_a_casa.png", IsFinished = false, IsDeleted = false }
            });

            // --- ATTIVITÀ 6: Mangiare (Cena) ---
            var actCenaId = Guid.NewGuid();
            allActivities.Add(new Activity
            {
                Id = actCenaId,
                RoutineId = routineId,
                Order = 6,
                Name = "Mangiare",
                SpeechText = "È ora di cena, andiamo a mangiare",
                ImageUrl = "pranzo.png",
                IsDeleted = false
            });
            allActions.AddRange(CreateActionsMangiare(actCenaId, "Mangia la cena"));

            // --- ATTIVITÀ 7: Lavarsi i denti (Sera) ---
            var actDentiSeraId = Guid.NewGuid();
            allActivities.Add(new Activity
            {
                Id = actDentiSeraId,
                RoutineId = routineId,
                Order = 7,
                Name = "Lavarsi i denti",
                SpeechText = "Ora ci laviamo di nuovo i denti",
                ImageUrl = "lavare_i_denti.png",
                IsDeleted = false
            });
            allActions.AddRange(CreateActionsDenti(actDentiSeraId));

            // --- ATTIVITÀ 8: Dormire ---
            var actDormireId = Guid.NewGuid();
            allActivities.Add(new Activity
            {
                Id = actDormireId,
                RoutineId = routineId,
                Order = 8,
                Name = "Dormire",
                SpeechText = "È ora di fare la nanna, andiamo a dormire",
                ImageUrl = "dormire.png",
                IsDeleted = false
            });
            allActions.AddRange(new[]
            {
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actDormireId, Order = 0, Name = "Mettersi a letto", SpeechText = "Mettiti sotto le coperte nel letto", ImageUrl = "mettersi_a_letto.png", IsFinished = false, IsDeleted = false },
                new RoutineAction { Id = Guid.NewGuid(), ActivityId = actDormireId, Order = 1, Name = "Chiudere gli occhi", SpeechText = "Chiudi gli occhi e fai la nanna", ImageUrl = "chiudere_gli_occhi.png", IsFinished = false, IsDeleted = false }
            });

            // Salvataggio nel Database SQLite
            await Database.InsertAsync(routine);
            await Database.InsertAllAsync(allActivities);
            await Database.InsertAllAsync(allActions);
        }
    }
}
