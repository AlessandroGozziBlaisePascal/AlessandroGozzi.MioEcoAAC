# 🌟 MioEco - App di Comunicazione Aumentativa e Alternativa (CAA)

**MioEco** è un'applicazione mobile sviluppata con **.NET MAUI** ideata per supportare la comunicazione aumentativa e alternativa (CAA) per persone con bisogni comunicativi complessi o nello spettro autistico.

L'obiettivo principale è stimolare l'autonomia nella vita quotidiana attraverso routine visive chiare, sintesi vocale e feedback sonori immediati.

---

## 🏛️ ARCHITETTURA DEL PROGETTO

L'architettura segue i principi del **Domain-Driven Design (DDD)** combinati con il pattern **MVVM (Model-View-ViewModel)** mediante l'ausilio di *CommunityToolkit.Mvvm*:

- 📦 **`AlessandroGozzi.AppAAC.Domain`**: Entità di business, modelli di dominio e contratti di interfaccia.
- 💾 **`AlessandroGozzi.AppAAC.Infrastructure`**: Implementazione pratica della persistenza dati e gestione storage locale.
- 📱 **`AlessandroGozzi.AppAAC.MobileGUI`**: Interfaccia grafica (XAML/C#), ViewModels e configurazioni native per la piattaforma Android.

---

## ⚙️ SERVIZI E INTERFACCE CHIAVE

L'intera applicazione sfrutta il meccanismo nativo di **Dependency Injection** configurato in `MauiProgram.cs`:

- 📋 **`IRoutineRepository`**: Gestisce il ciclo di vita completo (creazione, lettura, aggiornamento e salvataggio) delle sequenze giornaliere e di ciascun singolo step operativo.
- 🗣️ **`ITextToSpeechService`**: Incapsula il motore di sintesi vocale (TTS) del dispositivo per leggere testi, etichette delle azioni e messaggi di incoraggiamento.
- 🔊 **`IAudioManager`**: Coordina gli effetti acustici ed i feedback sonori (suoni di avvio, conferme di selezione e traguardi completati).

---

## ✨ FUNZIONALITÀ PRINCIPALI

- 🗓️ **ROUTINE VISIVE PASSO-PASSO**: Sequenze chiare per scandire la giornata con monitoraggio visivo dello stato di completamento.
- 🛡️ **INTRO-PAGE ACCESSIBILE**: Schermata iniziale studiata per evitare il sovraccarico sensoriale, priva di animazioni distraenti e con audio bilanciato.
- 🔒 **PARENTAL GATE & IMPOSTAZIONI PROTETTE**: Area `SettingsPage` sicura per educatori e genitori, protetta contro tocchi e uscite accidentali.
- ✉️ **INVIA SEGNALAZIONE INTEGRATO**: Canale di supporto rapido che apre direttamente l'app **Gmail** con destinatario, oggetto e modello del messaggio precompilati.
- 🎨 **ICONA ADATTIVA PERSONALIZZATA**: Grafica coerente con sfondo bianco (`icon_app_logo.png`) studiata per adattarsi alle diverse maschere dei launcher Android.

---

## 📋 VARIANTI E REQUISITI TECNICI

MioEco è disponibile in due rami/versioni per garantire la massima copertura hardware:

| Specifica | Versione Standard (Main) | Versione Legacy / 32-bit (Branch dedicato) |
| :--- | :--- | :--- |
| **Target Framework** | .NET 9.0 (`net9.0-android`) | .NET 8.0 (`net8.0-android`) |
| **Architetture Supportate** | 64-bit (`arm64-v8a`, `x86_64`) | 32-bit (`armeabi-v7a`) & 64-bit fallback |
| **Versione Android Minima** | Android 7.0 (API 24+) | Android 5.0 (API 21+) |
| **Destinazione d'Uso** | Smartphone e tablet moderni | Tablet AAC dedicati, dispositivi datati o entry-level |

---

## 📲 GUIDA ALL'INSTALLAZIONE (APK)

Nelle [GitHub Releases](https://github.com/) del progetto sono disponibili due pacchetti `.apk` distinti. Scegli quello adatto al tuo dispositivo:

### 1. Dispositivi Recenti (Consigliato per la maggior parte degli utenti)
* **File da scaricare**: `com.alessandrogozzi.mioeco-Signed.apk` (o `MioEco_vX.X_64bit_Signed.apk`)
* **Compatibilità**: Tutti i dispositivi moderni con Android 7.0 o superiore a 64-bit.
* **Vantaggi**: Ottimizzazioni prestazionali del runtime .NET 9.

### 2. Dispositivi Datati o Tablet Dedicati (32-bit)
* **File da scaricare**: `MioEco_v1-0_32bit_Signed.apk`
* **Compatibilità**: Dispositivi con processori `armeabi-v7a` o versioni Android a partire dalla 5.0 (Lollipop).
* **Vantaggi**: Risolve errori del tipo *"Applicazione non compatibile"* su dispositivi che non supportano istruzioni a 64-bit.

#### Passaggi per l'installazione su Android:
1. Scarica l'APK scelto sul dispositivo.
2. Tocca il file scaricato. Se richiesto, attiva l'opzione **Consenti da questa fonte** / **Origini sconosciute**.
3. Seleziona **Installa** e attendi il messaggio di conferma.

---

## 🚀 COMPILAZIONE DA SORGENTI

### Versione Standard (.NET 9 - 64-bit)
Assicurati di essere sul branch principale (`main`):

```powershell
dotnet publish .\AlessandroGozzi.AppAAC.MobileGUI\AlessandroGozzi.AppAAC.MobileGUI.csproj -c Release -f net9.0-android -p:AndroidPackageFormat=apk
```
📂 Percorso output: `.\AlessandroGozzi.AppAAC.MobileGUI\bin\Release\net9.0-android\publish\`

### Versione Legacy (.NET 8 - 32-bit)
Esegui il checkout sul branch 32-bit (`git checkout <nome-branch-32bit>`):

```powershell
dotnet publish .\AlessandroGozzi.AppAAC.MobileGUI\AlessandroGozzi.AppAAC.MobileGUI.csproj -c Release -f net8.0-android -p:AndroidPackageFormat=apk -p:RuntimeIdentifier=android-arm
```
📂 Percorso output: `.\AlessandroGozzi.AppAAC.MobileGUI\bin\Release\net8.0-android\android-arm\publish\`

---

## 📬 CONTATTI E SUPPORTO

Per assistenza, dubbi, feedback o proposte di miglioramento:

- 👤 **Sviluppatore**: Alessandro Gozzi
- 📧 **Email**: alessandrogozzi14@gmail.com
