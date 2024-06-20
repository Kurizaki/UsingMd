# Projekt-Dokumentation UsingMd

Keanu Koelewijn

| Datum      | Version | Zusammenfassung                                             |
| ---------- | ------- | ----------------------------------------------------------- |
| 17.05.2024 | 0.0.1   | Initiales Setup des Projekts, Projektdokumentation erstellt |
| 24.05.2024 | 0.1.0   | Grundlegende Funktionalitäten implementiert                 |
| 07.06.2024 | 0.2.0   | Erweiterung der Benutzeroberfläche                          |
| 14.06.2024 | 0.3.0   | Implementierung des Markdown-Editors abgeschlossen          |
| 14.06.2024 | 1.0.0   | Projekt abgeschlossen und final getestet                    |

## 1 Informieren

### 1.1 Projektübersicht

Entwicklung einer WPF-Anwendung zur Bearbeitung und Vorschau von Markdown-Dateien mit einer integrierten WebView2-Komponente.

### 1.2 User Stories

| US-№ | Verbindlichkeit | Typ        | Beschreibung                                                                                                                           |
| ---- | --------------- | ---------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| 1    | Muss            | Funktional | Als Entwickler möchte ich eine Methode implementieren, um Markdown-Dateien zu öffnen, damit ich vorhandene Dateien bearbeiten kann.    |
| 2    | Muss            | Funktional | Als Entwickler möchte ich eine Methode implementieren, um Markdown-Dateien zu speichern, damit Änderungen gespeichert werden können.   |
| 3    | Muss            | Funktional | Als Entwickler möchte ich eine Live-Vorschau des Markdown-Textes in HTML anzeigen, damit Benutzer ihre Änderungen sofort sehen können. |
| 4    | Muss            | Funktional | Als Benutzer möchte ich Text im Markdown-Format eingeben und formatiert anzeigen lassen, um die Lesbarkeit zu erhöhen.                 |
| 5    | Muss            | Funktional | Als Benutzer möchte ich verschiedene Formatierungsoptionen wie Fett und Kursiv nutzen können, um den Text zu gestalten.                |
| 6    | Muss            | Funktional | Als Benutzer möchte ich zwischen verschiedenen Themen und Modi (Hell/Dunkel) wechseln können, um die Benutzeroberfläche anzupassen.    |
| 7    | Kann            | Funktional | Als Benutzer möchte ich Tabellen im Markdown-Format bearbeiten und anzeigen können, um strukturierte Daten zu verwalten.               |

### 1.3 Testfälle

| TC-№ | Ausgangslage                             | Eingabe                          | Erwartete Ausgabe                           |
| ---- | ---------------------------------------- | -------------------------------- | ------------------------------------------- |
| 1.1  | Anwendung ist gestartet                  | Öffnen einer Markdown-Datei      | Inhalt der Datei wird im Textfeld angezeigt |
| 2.1  | Markdown-Text ist eingegeben             | Speichern der Datei              | Datei wird erfolgreich gespeichert          |
| 3.1  | Markdown-Text ist eingegeben             | Aktualisieren der Vorschau       | WebViewEditor wird korrekt angezeigt        |
| 4.1  | Anwendung ist gestartet                  | Eingabe von Markdown-Text im raw | Text wird formatiert angezeigt              |
| 5.1  | Markdown-Text ist eingegeben             | Anwendung von Fettschrift        | Text wird fett dargestellt                  |
| 6.1  | Anwendung ist gestartet                  | Wechsel des Themas               | Benutzeroberfläche wechselt das Thema       |
| 7.1  | Markdown-Text mit Tabelle ist eingegeben | Tabelle wird bearbeitet          | Änderungen werden korrekt angezeigt         |

### 1.4 Diagramme

![ee7c484a-22e8-48c5-99fd-ef937e5a74b2](https://github.com/Kurizaki/UsingMd/assets/110892283/c7023f31-5526-412d-86dd-5a761599ee88)
![76f54aa1-dac1-43c4-83d7-bd923bc93f16](https://github.com/Kurizaki/UsingMd/assets/110892283/662b83cd-83c8-4578-9d19-7915e063d5b6)


Hier ist die überarbeitete Planung mit einem Gesamtzeitaufwand von 20 Stunden:

## 2 Planen

| AP-№ | Frist      | Zuständig       | Beschreibung                                         | geplante Zeit |
| ---- | ---------- | --------------- | ---------------------------------------------------- | ------------- |
| 1.A  | 24.05.2024 | Keanu Koelewijn | Projektsetup in Visual Studio                        | 1h            |
| 1.B  | 24.05.2024 | Keanu Koelewijn | GitHub-Repository erstellen                          | 15min         |
| 2.A  | 24.05.2024 | Keanu Koelewijn | Implementierung der Dateiöffnungs-Funktion           | 2h 30min      |
| 2.B  | 07.06.2024 | Keanu Koelewijn | Implementierung der Dateispeicher-Funktion           | 2h 30min      |
| 3.A  | 07.06.2024 | Keanu Koelewijn | Implementierung der Markdown-Vorschau                | 3h            |
| 4.A  | 07.06.2024 | Keanu Koelewijn | Integration von Formatierungsoptionen (Fett, Kursiv) | 2h 30min      |
| 5.A  | 14.06.2024 | Keanu Koelewijn | Integration der Themen- und Moduswechsel             | 2h 30min      |
| 6.A  | 14.06.2024 | Keanu Koelewijn | Implementierung der Tabellenbearbeitung              | 3h            |
| 7.A  | 14.06.2024 | Keanu Koelewijn | Erstellung und Durchführung von Testfällen           | 3h            |

Total: 20h 30min

## 3 Entscheiden

Ich habe mich entschieden, WebView2 für den formatierten Editor zu verwenden, um eine aktuelle und schnelle Darstellung der Markdown-Inhalte zu gewährleisten. Für die Formatierungsoptionen setze ich auf einfache Buttons, die die entsprechenden Markdown-Syntaxelemente einfügen.

## 4 Realisieren

| AP-№ | Datum      | Zuständig       | geplante Zeit | tatsächliche Zeit |
| ---- | ---------- | --------------- | ------------- | ----------------- |
| 1.A  | 24.05.2024 | Keanu Koelewijn | 1h            | 1h                |
| 1.B  | 24.05.2024 | Keanu Koelewijn | 1h            | 1h                |
| 2.A  | 24.05.2024 | Keanu Koelewijn | 2h            | 2h 15min          |
| 2.B  | 24.05.2024 | Keanu Koelewijn | 2h            | 2h                |
| 3.A  | 07.06.2024 | Keanu Koelewijn | 3h            | 3h 30min          |
| 4.A  | 07.06.2024 | Keanu Koelewijn | 2h 30min      | 2h 45min          |
| 5.A  | 14.06.2024 | Keanu Koelewijn | 2h 30min      | 2h 30min          |
| 6.A  | 14.06.2024 | Keanu Koelewijn | 3h            | 3h 15min          |
| 7.A  | 14.06.2024 | Keanu Koelewijn | 3h            | 3h                |

## 5 Kontrollieren

### 5.1 Testprotokoll

| TC-№ | Datum      | Resultat | Tester          |
| ---- | ---------- | -------- | --------------- |
| 1.1  | 14.06.2024 | OK       | Keanu Koelewijn |
| 2.1  | 14.06.2024 | OK       | Keanu Koelewijn |
| 3.1  | 14.06.2024 | OK       | Keanu Koelewijn |
| 4.1  | 14.06.2024 | OK       | Keanu Koelewijn |
| 5.1  | 14.06.2024 | NOK      | Keanu Koelewijn |
| 6.1  | 14.06.2024 | OK       | Keanu Koelewijn |
| 7.1  | 14.06.2024 | OK       | Keanu Koelewijn |

Nicht alle Tests liefen erfolgreich, die Buttons reagieren noch nicht ganz wie gewünscht.

## 6 Auswerten

Keanu Koelewijn: [Portfolioeintrag Keanu Koelewijn](https://portfolio.bbbaden.ch/view/view.php?t=a8e509f346e01f537852)

## Verwendeten Quelen

[Get started with WebView2 in WPF apps - Microsoft Edge Developer documentation | Microsoft Learn](https://learn.microsoft.com/en-us/microsoft-edge/webview2/get-started/wpf)

https://github.com/MicrosoftEdge/WebView2Samples

https://github.com/wrimle/typedown

https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.control.invoke?view=windowsdesktop-8.0

https://stackoverflow.com/questions/2486387/how-to-hide-the-scrollbar-using-javascript

https://stackoverflow.com/questions/62694172/wpf-xaml-designer-error-with-datatype-duration-build-successful-but-still-through

https://stackoverflow.com/questions/20363100/wpf-popup-location-issue

https://chatgpt.com/ --> Für das Problemen von kleine Fehler bei den Methoden ChangeMode/Theme, SynchronizeMarkdown beheben und kommentare besser zu formulieren.
