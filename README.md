# 🧩 App AAC - Routine & Attività Visive 🎨✨

Benvenuto in **App AAC**, un'applicazione mobile moderna, accessibile e intuitiva sviluppata in **.NET MAUI** per supportare la **Comunicazione Aumentativa e Alternativa (CAA)** 💬📱. L'app è progettata specificamente per bambini e persone con bisogni comunicativi complessi o nello spettro autistico, guidandoli nell'apprendimento e nello svolgimento autonomo delle routine quotidiane attraverso supporti visivi chiari e sintesi vocale multimodale 🌟🚀.

---

## 🌟 Funzionalità Principali 💡
* 🗂️ **Gerarchia a 3 Livelli:** Organizzazione strutturata e ordinata: **Routine ➡️ Attività ➡️ Sequenza di Azioni**.
* 🖼️ **Supporto Visivo CAA:** Pittogrammi ad alto contrasto visivo e leggibilità immediata per ridurre il carico cognitivo.
* 🔓 **Avanzamento Sequenziale Guidato:** Sistema di blocco/sblocco progressivo delle tappe con indicatori di spunta visiva verde.
* 🔍 **Modalità Focus Dettaglio:** Visualizzazione a schermo intero con feedback modale per ogni singola azione.
* 🗣️ **Sintesi Vocale Italiana (TTS):** Istruzioni vocali chiare e incoraggiamenti pronunciati automaticamente in lingua italiana.
* 🏆 **Schermata Finale "Complimenti":** Rinforzo positivo con immagine celebrativa, speech automatico e uscita rapida con tocco ovunque sullo schermo.
* 💾 **100% Funzionante Offline:** Persistenza locale autonoma con database SQLite precaricato, senza bisogno di connessione internet.

---

## 🏗️ Architettura e Tecnologie ⚙️
* 💻 **Framework:** .NET 9.0 (.NET MAUI) per Android
* 🎯 **Design Pattern:** MVVM (Model-View-ViewModel) tramite **CommunityToolkit.Mvvm**
* 📦 **Persistenza Dati:** SQLite locale tramite **sqlite-net-pcl**
* 🧭 **Navigazione & DI:** Shell Navigation con Dependency Injection integrata
* 📁 **Suddivisione Modulare della Soluzione:**
  * 🏛️ `AlessandroGozzi.AppAAC.Domain`: Entità di dominio, modelli e logiche pure agnostiche dalla piattaforma.
  * 🗄️ `AlessandroGozzi.AppAAC.Infrastructure`: Context SQLite, seed automatico e repository dei dati.
  * 📱 `AlessandroGozzi.AppAAC.MobileGUI`: Viste XAML, ViewModels, handler TTS nativi e risorse grafiche.

---

## 📋 Routine & Attività Incluse nella Demo 🧪
La demo include un database locale pre-popolato pronto all'uso con la routine **"Giornata normale"**:
* 🪥 **Lavarsi i denti (6 step):** Prendere lo spazzolino ➡️ Mettere il dentifricio ➡️ Spazzolare i denti ➡️ Sputare e sciacquare ➡️ Mettere a posto lo spazzolino ➡️ Lavarsi le mani.
* 🎨 **Dipingere (3 step):** Prendere pennello e colori ➡️ Intingere il pennello ➡️ Disegnare sulla tela.

---

## 🚀 Guida Rapida e Installazione 📲

### 📥 Installazione Diretta (Utenti & Terapisti)
1. Scarica il file eseguibile **`com.companyname.alessandrogozzi.appaac.mobilegui-Signed.apk`** dalla sezione [Releases](../../releases) del repository 📦.
2. Trasferisci l'APK sul tuo smartphone o tablet Android 📲.
3. Consenti l'installazione da origini sconosciute nelle impostazioni di Android e avvia l'app 🎉.

### 💻 Compilazione da Codice Sorgente (Sviluppatori)
1. Clona il repository:
   ```bash
   git clone https://github.com/<tuo-utente>/<tuo-repo>.git
   cd <tuo-repo>
   ```
2. Ripristina i pacchetti NuGet:
   ```bash
   dotnet restore
   ```
3. Genera l'APK di Release pronto per la distribuzione:
   ```bash
   dotnet publish AlessandroGozzi.AppAAC.MobileGUI/AlessandroGozzi.AppAAC.MobileGUI.csproj -f net9.0-android -c Release -p:AndroidPackageFormat=apk
   ```

---

## 📜 Crediti & Licenza Simboli ARASAAC 🏷️
I simboli grafici utilizzati in questa applicazione provengono dal catalogo di **ARASAAC** (http://www.arasaac.org), creato da Sergio Palao per conto del Governo di Aragona, e sono distribuiti sotto licenza **Creative Commons BY-NC-SA** (Attribuzione - Non commerciale - Condividi allo stesso modo).
