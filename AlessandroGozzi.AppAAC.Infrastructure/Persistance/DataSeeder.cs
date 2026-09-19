using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.AggregateRoots;
using AlessandroGozzi.AppAAC.Domain.Entities;
using AlessandroGozzi.AppAAC.Domain.Enums;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace AlessandroGozzi.AppAAC.Infrastructure.Persistance
{
    internal static class DataSeeder
    {
        public static async Task SeedAsync(DbContext context)
        {

            if (await context.Set<Routine>().AnyAsync())
            {
                return;
            }

            static SpeechText Text(string val) => SpeechText.Create(val).Value;
            static ImageUrl Img(string val) => ImageUrl.Create(val).Value;

            static List<RoutineAction> SingleAction(string title, string img) => new()
        {
            RoutineAction.Create(Img(img), Text(title), title).Value
        };

            var dentiActions = new List<RoutineAction>
        {
            RoutineAction.Create(
                Img("spazzolino.png"),
                Text("Prendere lo spazzolino"),
                "Prendere lo spazzolino").Value,

            RoutineAction.Create(
                Img("dentifricio_e_spazzolino.png"),
                Text("Mettere il dentifricio sullo spazzolino"),
                "Mettere il dentifricio").Value,

            RoutineAction.Create(
                Img("spazzolare_i_denti.png"),
                Text("Spazzolare bene tutti i denti"),
                "Spazzolare i denti").Value
        };

            var activities = new List<Activity>
        {
            Activity.Create("Svegliarsi", Text("Svegliarsi la mattina"), Img("svegliarsi.png"), SingleAction("Svegliarsi", "svegliarsi.png")).Value,
            Activity.Create("Colazione", Text("Fare una buona colazione"), Img("far_colazione.png"), SingleAction("Colazione", "far_colazione.png")).Value,
            Activity.Create("Lavarsi i denti", Text("Lavarsi bene i denti"), Img("lavarsi_i_denti.png"), dentiActions).Value,
            Activity.Create("Vestirsi", Text("Vestirsi per la giornata"), Img("vestirsi.png"), SingleAction("Vestirsi", "vestirsi.png")).Value,
            Activity.Create("Pranzo", Text("Mangiare a pranzo"), Img("pranzo.png"), SingleAction("Pranzo", "pranzo.png")).Value,
            Activity.Create("Gioco", Text("Tempo di giocare"), Img("gioco.png"), SingleAction("Gioco", "gioco.png")).Value,
            Activity.Create("Cena", Text("Mangiare a cena"), Img("cena.png"), SingleAction("Cena", "cena.png")).Value,
            Activity.Create("Dormire", Text("Andare a dormire nel letto"), Img("dormire.png"), SingleAction("Dormire", "dormire.png")).Value
        };

            var routineResult = Routine.Create("Giorno Normale", Text("Routine giornaliera completa"), RoutineType.NormalDay);
            var routine = routineResult.Value;

            foreach (var activity in activities)
            {
                routine.AddActivity(activity);
            }

            await context.Set<Routine>().AddAsync(routine);

            await context.SaveChangesAsync();
        }
    }
}
