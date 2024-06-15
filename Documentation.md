# Projekt-Dokumentation UsingMd

Keanu Koelewijn

| Datum      | Version | Zusammenfassung                                              |
| ---------- | ------- | ------------------------------------------------------------ |
| 10.06.2024 | 0.0.1   | Initiales Setup des Projekts, Projektdokumentation erstellt  |
| 12.06.2024 | 0.1.0   | Grundlegende Funktionalitäten implementiert                  |
| 14.06.2024 | 0.2.0   | Erweiterung der Benutzeroberfläche                           |
| 16.06.2024 | 0.3.0   | Implementierung der Markdown-Vorschau abgeschlossen          |
| 18.06.2024 | 1.0.0   | Projekt abgeschlossen und final getestet                     |

## 1 Informieren

### 1.1 Ihr Projekt

Entwicklung einer WPF-Anwendung zur Bearbeitung und Vorschau von Markdown-Dateien mit integrierter WebView2-Komponente.

### 1.2 User Stories

| US-№ | Verbindlichkeit | Typ        | Beschreibung |
| ---- | --------------- | ---------- | ------------ |
| 1    | Muss            | Funktional | Als Entwickler möchte ich eine Methode implementieren, um Markdown-Dateien zu öffnen, damit ich vorhandene Dateien bearbeiten kann. |
| 2    | Muss            | Funktional | Als Entwickler möchte ich eine Methode implementieren, um Markdown-Dateien zu speichern, damit Änderungen gespeichert werden können. |
| 3    | Muss            | Funktional | Als Entwickler möchte ich eine Live-Vorschau des Markdown-Textes in HTML anzeigen, damit Benutzer ihre Änderungen sofort sehen können. |
| 4    | Muss            | Funktional | Als Benutzer möchte ich Text im Markdown-Format eingeben und formatiert anzeigen lassen, um die Lesbarkeit zu erhöhen. |
| 5    | Muss            | Funktional | Als Benutzer möchte ich verschiedene Formatierungsoptionen wie Fett und Kursiv nutzen können, um den Text zu gestalten. |
| 6    | Muss            | Funktional | Als Benutzer möchte ich zwischen verschiedenen Themen und Modi (Hell/Dunkel) wechseln können, um die Benutzeroberfläche anzupassen. |
| 7    | Kann            | Funktional | Als Benutzer möchte ich Tabellen im Markdown-Format bearbeiten und anzeigen können, um strukturierte Daten zu verwalten. |

### 1.3 Testfälle

| TC-№ | Ausgangslage                                  | Eingabe                                  | Erwartete Ausgabe                                           |
| ---- | --------------------------------------------- | ---------------------------------------- | ----------------------------------------------------------- |
| 1.1  | Anwendung ist gestartet                       | Öffnen einer Markdown-Datei              | Inhalt der Datei wird im Textfeld angezeigt                 |
| 2.1  | Markdown-Text ist eingegeben                  | Speichern der Datei                      | Datei wird erfolgreich gespeichert                          |
| 3.1  | Markdown-Text ist eingegeben                  | Aktualisieren der Vorschau               | HTML-Vorschau wird korrekt angezeigt                        |
| 4.1  | Anwendung ist gestartet                       | Eingabe von Markdown-Text                | Text wird formatiert angezeigt                              |
| 5.1  | Markdown-Text ist eingegeben                  | Anwendung von Fettschrift                | Text wird fett dargestellt                                  |
| 6.1  | Anwendung ist gestartet                       | Wechsel des Themas                       | Benutzeroberfläche wechselt das Thema                       |
| 7.1  | Markdown-Text mit Tabelle ist eingegeben      | Tabelle wird bearbeitet                  | Änderungen werden korrekt angezeigt                         |

### 1.4 Diagramme

(Mockup)

## 2 Planen

| AP-№ | Frist      | Zuständig       | Beschreibung                                              | geplante Zeit |
| ---- | ---------- | --------------- | --------------------------------------------------------- | ------------- |
| 1.A  | 11.06.2024 | Keanu Koelewijn | Projektsetup in Visual Studio                             | 45min         |
| 1.B  | 11.06.2024 | Keanu Koelewijn | GitHub-Repository erstellen                               | 15min         |
| 2.A  | 12.06.2024 | Keanu Koelewijn | Implementierung der Dateiöffnungs-Funktion                | 1h            |
| 2.B  | 12.06.2024 | Keanu Koelewijn | Implementierung der Dateispeicher-Funktion                | 1h            |
| 3.A  | 13.06.2024 | Keanu Koelewijn | Implementierung der Markdown-Vorschau                     | 2h            |
| 4.A  | 13.06.2024 | Keanu Koelewijn | Integration von Formatierungsoptionen (Fett, Kursiv)      | 1h 30min      |
| 5.A  | 14.06.2024 | Keanu Koelewijn | Integration der Themen- und Moduswechsel                  | 1h 30min      |
| 6.A  | 15.06.2024 | Keanu Koelewijn | Implementierung der Tabellenbearbeitung                   | 2h            |
| 7.A  | 16.06.2024 | Keanu Koelewijn | Erstellung und Durchführung von Testfällen                | 2h            |

Total: 12h 30min

## 3 Entscheiden

Ich habe entschieden, WebView2 für die HTML-Vorschau zu verwenden, um eine aktuelle und schnelle Darstellung der Markdown-Inhalte zu gewährleisten. Für die Formatierungsoptionen setze ich auf einfache Buttons, die die entsprechenden Markdown-Syntaxelemente einfügen.

## 4 Realisieren

| AP-№ | Datum      | Zuständig       | geplante Zeit | tatsächliche Zeit |
| ---- | ---------- | --------------- | ------------- | ----------------- |
| 1.A  | 11.06.2024 | Keanu Koelewijn | 45min         | 45min             |
| 1.B  | 11.06.2024 | Keanu Koelewijn | 15min         | 15min             |
| 2.A  | 12.06.2024 | Keanu Koelewijn | 1h            | 1h 15min          |
| 2.B  | 12.06.2024 | Keanu Koelewijn | 1h            | 1h                |
| 3.A  | 13.06.2024 | Keanu Koelewijn | 2h            | 2h 30min          |
| 4.A  | 13.06.2024 | Keanu Koelewijn | 1h 30min      | 1h 45min          |
| 5.A  | 14.06.2024 | Keanu Koelewijn | 1h 30min      | 1h 30min          |
| 6.A  | 15.06.2024 | Keanu Koelewijn | 2h            | 2h 15min          |
| 7.A  | 16.06.2024 | Keanu Koelewijn | 2h            | 2h                |

## 5 Kontrollieren

### 5.1 Testprotokoll

| TC-№ | Datum      | Resultat | Tester         |
| ---- | ---------- | -------- | -------------- |
| 1.1  | 16.06.2024 | OK       | Keanu Koelewijn|
| 2.1  | 16.06.2024 | OK       | Keanu Koelewijn|
| 3.1  | 16.06.2024 | OK       | Keanu Koelewijn|
| 4.1  | 16.06.2024 | OK       | Keanu Koelewijn|
| 5.1  | 16.06.2024 | OK       | Keanu Koelewijn|
| 6.1  | 16.06.2024 | OK       | Keanu Koelewijn|
| 7.1  | 16.06.2024 | OK       | Keanu Koelewijn|

### 5.2 Exploratives Testen

| BR-№ | Ausgangslage | Eingabe | Erwartete Ausgabe | Tatsächliche Ausgabe |
| ---- | ------------ | ------- | ----------------- | -------------------- |
| I    | Anwendung gestartet | Eingabe von Markdown-Text | Korrekte Vorschau | Vorschau wird korrekt angezeigt |
| II   | Tabelle vorhanden | Bearbeitung der Tabelle | Tabelle wird korrekt aktualisiert | Tabelle wird korrekt aktualisiert |

Alle Tests verliefen erfolgreich und ergaben keine schwerwiegenden Fehler.

## 6 Auswerten

Keanu Koelewijn: [Portfolio eintrag Keanu Koelewijn](https://portfolio.bbbaden.ch/view/view.php?t=a8e509f346e01f537852)
