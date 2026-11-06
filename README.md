# 🌟 MioEco-Material - App di Comunicazione Aumentativa e Alternativa (CAA)

**MioEco-Material** è l'evoluzione con interfaccia basata sulle linee guida di **Material Design** dell'ecosistema **MioEco**, sviluppata su piattaforma **.NET MAUI** per supportare la comunicazione aumentativa e alternativa (CAA) per persone con bisogni comunicativi complessi o nello spettro autistico.

L'applicazione introduce un'esperienza visiva rinnovata con contrasti ottimizzati, tessere interattive ad alta leggibilità, risposte tattili e feedback sonori immediati pensati per favorire l'autonomia quotidiana.

---

## 🆔 IDENTITÀ DELL'APPLICAZIONE

MioEco-Material è configurata con un ID applicativo dedicato: può essere installata e usata in parallelo alla versione standard senza alcun conflitto di pacchetti o sovrascrittura dati:

- **Nome Visualizzato (Label)**: `MioEco-Material`
- **Application ID (Package Name)**: `com.alessandrogozzi.mioecomaterial`
- **Versione**: `1.0` (Build `1`)

---

## 🏛️ ARCHITETTURA DEL PROGETTO

L'architettura adotta i principi del **Domain-Driven Design (DDD)** integrati con il pattern **MVVM (Model-View-ViewModel)** attraverso *CommunityToolkit.Mvvm*, organizzata nella cartella `src/`:

- 📦 **`AlessandroGozzi.MioEco-Material.Domain`**: Entità di business, modelli di dominio, logica di validazione e contratti di interfaccia.
- 💾 **`AlessandroGozzi.MioEco-Material.Infrastructure`**: Persistenza dati locale tramite database SQLite (`AppDbContext`), gestione archiviazione locale e repository.
- 📱 **`AlessandroGozzi.MioEco-Material.MobileGUI`**: Interfaccia grafica Material (XAML/C#), stili tematici, converter, ViewModels e servizi specifici per la piattaforma Android.

---

## ⚙️ SERVIZI E DEPENDENCY INJECTION

La composizione dei servizi in `MauiProgram.cs` orchestra i componenti fondamentali:

- 📋 **`IRoutineRepository`**: Gestione del ciclo di vita CRUD di routine, attività e tessere operative con salvataggio asincrono su database locale.
- 🗣️ **`ITextToSpeechService`**: Motore nativo di sintesi vocale (TTS) per la lettura automatica di etichette, azioni e frasi descrittive.
- 🔊 **`IAudioManager`**: Coordinamento dei feedback uditivi (conferme di pressione, avvisi e suoni di traguardo completato) tramite *Plugin.Maui.Audio*.
- 🌐 **`IArasaacService`**: Client per l'integrazione e gestione delle risorse visive e dei pittogrammi CAA.

---

## ✨ FUNZIONALITÀ PRINCIPALI

- 🎨 **INTERFACCIA MATERIAL**: Schede, pulsanti e tessere disegnati secondo gli standard Material Design con feedback visivo immediato.
- 🗓️ **ROUTINE E ATTIVITÀ PASSO-PASSO**: Suddivisione chiara dei compiti giornalieri con monitoraggio visuale e indicatore di avanzamento.
- 🛡️ **ACCESSIBILITÀ SENSORIALE**: Layout bilanciato studiato per prevenire il sovraccarico cognitivo ed emotivo.
- 🔒 **PARENTAL GATE & IMPOSTAZIONI PROTETTE**: Sezione impostazioni riservata a genitori, terapisti ed educatori con meccanismo di sblocco a tutela da tocchi accidentali.
- ✉️ **SUPPORTO E SEGNALAZIONI DIRETTE**: Intent dedicato per comporre e inviare segnalazioni e richieste di assistenza via email.

---

## 📋 REQUISITI TECNICI

| Specifica | Dettaglio Versione Material |
| :--- | :--- |
| **Target Framework** | .NET 10.0 Android (`net10.0-android`) / .NET 9.0 Android |
| **Package ID** | `com.alessandrogozzi.mioecomaterial` |
| **Architetture Supportate** | `arm64-v8a` (64-bit), `armeabi-v7a` (32-bit fallback) |
| **Min SDK Android** | Android 7.0 (API Level 24) o superiore |

---

## 📲 INSTALLAZIONE DELL'APK

Il pacchetto `.apk` firmato è disponibile nella sezione [GitHub Releases](https://github.com/AlessandroGozziBlaisePascal/AlessandroGozzi.MioEcoAAC/releases) del branch `MioEco-Material`.

1. Scarica sul dispositivo il file `.apk` (es. `MioEcoMaterial-Signed.apk`).
2. Apri il file scaricato. Se compare un avviso di sicurezza Android, seleziona **Impostazioni** e abilita **Consenti l'installazione da questa sorgente**.
3. Premi **Installa** e avvia l'app al completamento.

> **Nota:** Essendo dotata di `ApplicationId` distinto, MioEco-Material non sovrascrive né cancella l'app MioEco standard eventualmente già presente sul dispositivo.

---

## 🚀 COMPILAZIONE DA TERMINALE (RELEASE)

Per compilare e generare autonomamente il pacchetto APK di rilascio da riga di comando:

```powershell
# 1. Posizionati nella directory del progetto GUI di Material
cd "src\AlessandroGozzi.MioEco-Material.MobileGUI"

# 2. Pulisci la cache per garantire una compilazione pulita
Remove-Item -Recurse -Force bin, obj -ErrorAction SilentlyContinue

# 3. Compila e genera l'APK in modalità Release
dotnet publish -f net10.0-android -c Release -p:AndroidPackageFormat=apk

# 4. Apri la cartella di output
explorer.exe "bin\Release\net10.0-android\publish"
```

*(Se il target framework nel file `.csproj` è impostato su .NET 9, sostituisci `net10.0-android` con `net9.0-android`).*

---

## 📄 Volantino Informativo e QR Code

Inquadra i codici QR dal volantino per scaricare l'app o salvalo per la stampa:

<img width="370" height="523" alt="Volantino A5 MioEcoFinalVersion" src="Volantino%20A5%20MioEcoFinalVersion.png" />
<br>

---

## 📬 CONTATTI E SUPPORTO

Per segnalazioni di bug, feedback o richieste di integrazione:

- 👤 **Sviluppatore**: Alessandro Gozzi
- 📧 **Email**: alessandrogozzi14@gmail.com
- 🐙 **Repository**: [AlessandroGozzi.MioEcoAAC](https://github.com/AlessandroGozziBlaisePascal/AlessandroGozzi.MioEcoAAC)
