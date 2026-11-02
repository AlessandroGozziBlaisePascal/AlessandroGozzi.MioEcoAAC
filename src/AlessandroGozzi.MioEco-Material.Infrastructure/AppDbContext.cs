using System;
using System.Collections.Generic;
using System.Text;
using AlessandroGozzi.MioEco_Material.Domain;
using SQLite;

namespace AlessandroGozzi.MioEco_Material.Infrastructure
{
    public class AppDbContext
    {
        private SQLiteAsyncConnection _connection;

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_connection is not null)
                return _connection;

            _connection = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags);

            await _connection.CreateTableAsync<FlashCard>();
            await SeedInitialDataAsync();

            return _connection;
        }

        private async Task SeedInitialDataAsync()
        {
            int count = await _connection.Table<FlashCard>().CountAsync();
            if (count > 0)
                return;

            var defaultCards = new List<FlashCard>
            {
                // Bevande
                new FlashCard("Acqua", "Voglio bere acqua", "acqua.png", Category.Bevande, "💧"),
                new FlashCard("Succo", "Voglio bere il succo", "succo.png", Category.Bevande, "🧃"),
                new FlashCard("Latte", "Voglio bere il latte", "latte.png", Category.Bevande, "🥛"),

                // Cibo
                new FlashCard("Biscotto", "Voglio un biscotto", "biscotti.png", Category.Cibo, "🍪"),
                new FlashCard("Mela", "Voglio mangiare la mela", "mela.png", Category.Cibo, "🍎"),
                new FlashCard("Pane", "Voglio un pezzo di pane", "pane.png", Category.Cibo, "🥖"),
                new FlashCard("Pasta", "Voglio la pasta", "pasta.png", Category.Cibo, "🍝"),

                // Bisogni
                new FlashCard("Bagno", "Devo andare in bagno", "bagno.png", Category.Bisogni, "🚾"),
                new FlashCard("Mani", "Voglio lavarmi le mani", "lavare_le_mani.png", Category.Bisogni, "🧼"),
                new FlashCard("Sonno", "Sono stanca, voglio dormire", "dormire.png", Category.Bisogni, "🛏️"),

                // Attività
                new FlashCard("Tablet", "Voglio giocare con il tablet", "tablet.png", Category.Attivita, "📱"),
                new FlashCard("Disegnare", "Voglio disegnare coi pennarelli", "disegnare.png", Category.Attivita, "🎨"),
                new FlashCard("Musica", "Voglio ascoltare musica", "musica.png", Category.Attivita, "🎵"),
                new FlashCard("Parco", "Voglio andare al parco giochi", "parco.png", Category.Attivita, "🛝"),

                // Persone
                new FlashCard("Mamma", "Voglio andare dalla mamma", "mamma.png", Category.Persone, "👩"),
                new FlashCard("Papà", "Voglio andare dal papà", "papa.png", Category.Persone, "👨"),

                // Emozioni
                new FlashCard("Felice", "Sono felice", "felice.png", Category.Emozioni, "😊"),
                new FlashCard("Arrabbiata", "Sono arrabbiata", "arrabbiata.png", Category.Emozioni, "😡"),
                new FlashCard("Triste", "Sono triste", "triste.png", Category.Emozioni, "😢")
            };

            await _connection.InsertAllAsync(defaultCards);
        }
    }
}
