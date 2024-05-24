# IPERKA Project Documentation

## Table of Contents

1. [Idea](#idea)
2. [Project Proposal](#project-proposal)
3. [Inform](#inform)
4. [Criteria Catalog](#criteria-catalog)
5. [Time Schedule](#time-schedule)
6. [Test Plan](#test-plan)
7. [User Stories](#user-stories)
8. [Test Concept](#test-concept)
9. [Requirements Specification](#requirements-specification)
10. [Task List](#task-list)
11. [Decision](#decision)
12. [Realization](#realization)
13. [Test Protocol](#test-protocol)
14. [Test Conclusion](#test-conclusion)
15. [Explorative Tests](#explorative-tests)
16. [Evaluate](#evaluate)

---

## Idea

### Description

The initial idea is to develop a Markdown Editor application with a user-friendly interface that supports both writing and previewing Markdown content. The application will include features such as syntax highlighting, real-time preview, and various editing tools.

### Goals

- Provide an intuitive interface for writing Markdown.
- Implement real-time Markdown preview.
- Support basic editing functionalities (bold, italic, headings, lists, etc.).
- Ensure cross-platform compatibility.

| Datum | Version | Summary                                              |
| ----- | ------- | ---------------------------------------------------- |
|       | 0.0.1   | ✍️ Each time you work on the project, add a new line and describe in *one* sentence what you achieved. |
|       | ...     |                                                      |
|       | 1.0.0   |                                                      |

---

## Project Proposal

### Project Title

Markdown Editor Application

### Project Overview

This project aims to create a versatile Markdown Editor that caters to both novice and experienced users. The editor will support Markdown syntax highlighting and provide a live preview of the Markdown content.

### Objectives

- Develop a robust Markdown parser.
- Design a user-friendly GUI.
- Integrate real-time preview functionality.
- Implement core Markdown editing features.

### Stakeholders

- Project Manager
- Developers
- End-users

### Benefits

- Simplifies the process of writing and previewing Markdown.
- Enhances productivity for content creators.
- Provides a learning tool for new Markdown users.

---

## Inform

### Research

Research will be conducted on existing Markdown editors, their features, and user reviews. Key areas of focus include user interface design, performance, and feature set.

### Technologies

- Programming Language: C#
- Framework: WPF
- Libraries: Markdig (for Markdown parsing), WebView2 (for rendering HTML preview)

### Constraints

- Time: Project must be completed within three months.
- Budget: Limited to open-source libraries and tools.

---

## Criteria Catalog

### Functional Requirements

- Real-time Markdown preview.
- Syntax highlighting.
- Basic text formatting (bold, italic, headings, lists).

### Non-Functional Requirements

- User-friendly interface.
- High performance and responsiveness.
- Cross-platform compatibility.

### Quality Criteria

- Usability: The application should be intuitive and easy to use.
- Performance: Real-time preview should not lag.
- Reliability: The application should handle various Markdown syntaxes correctly.

---

## Time Schedule

| Phase            | Start Date | End Date   | Duration |
|------------------|------------|------------|----------|
| Planning         | 01/06/2024 | 07/06/2024 | 1 week   |
| Design           | 08/06/2024 | 21/06/2024 | 2 weeks  |
| Development      | 22/06/2024 | 30/07/2024 | 6 weeks  |
| Testing          | 31/07/2024 | 14/08/2024 | 2 weeks  |
| Deployment       | 15/08/2024 | 21/08/2024 | 1 week   |

---

## Test Plan

### Testing Strategies

- Unit Testing: Test individual components and functionalities.
- Integration Testing: Ensure that different parts of the application work together.
- User Acceptance Testing (UAT): Validate the application with end-users.

### Test Environment

- Operating Systems: Windows, macOS, Linux
- Browsers: Chrome, Firefox, Edge (for WebView2 component)

### Test Cases

| TC-№ | Ausgangslage | Eingabe | Erwartete Ausgabe |
| ---- | ------------ | ------- | ----------------- |
| 1.1  |              |         |                   |
| ...  |              |         |                   |

---

## User Stories

| US-№ | Verbindlichkeit | Typ  | Beschreibung                       |
| ---- | --------------- | ---- | ---------------------------------- |
| 1    |                 |      | Als ein 🤷‍♂️ möchte ich 🤷‍♂️, damit 🤷‍♂️ |
| ...  |                 |      |                                    |

---

## Test Concept

### Unit Tests

Each core functionality (e.g., real-time preview, text formatting) will be tested independently to ensure correctness.

### Integration Tests

Tests will be conducted to ensure that the Markdown parser and preview components work seamlessly together.

### User Acceptance Tests

Selected users will be asked to use the application and provide feedback on usability and functionality.

---

## Requirements Specification

### Functional Requirements

1. **Real-time Preview:** The application must display a real-time preview of the Markdown content.
2. **Text Formatting:** The application must support bold, italic, headings, and lists.
3. **File Operations:** The application must allow users to open, save, and create new Markdown files.

### Non-Functional Requirements

1. **Performance:** The application should render the preview without noticeable delays.
2. **Usability:** The interface should be intuitive and easy to navigate.
3. **Compatibility:** The application should work on Windows, macOS, and Linux.

---

## Task List

### Planning

| AP-№ | Frist | Zuständig | Beschreibung | geplante Zeit |
| ---- | ----- | --------- | ------------ | ------------- |
| 1.A  |       |           |              |               |
| ...  |       |           |              |               |

### Design

- Create wireframes and mockups.
- Design the application architecture.

### Development

| AP-№ | Datum | Zuständig | geplante Zeit | tatsächliche Zeit |
| ---- | ----- | --------- | ------------- | ----------------- |
| 1.A  |       |           |               |                   |
| ...  |       |           |               |                   |

### Testing

- Write and execute unit tests.
- Conduct integration testing.
- Perform user acceptance testing.

### Deployment

- Prepare deployment packages.
- Distribute the application to users.

---

## Decision

### Key Decisions

- **Technology Stack:** Use C#, WPF, Markdig, and WebView2.
- **Feature Set:** Focus on core Markdown functionalities and user-friendly interface.
- **Testing Approach:** Implement unit, integration, and user acceptance testing.

---

## Realization

### Development Process

The development will follow an agile approach with iterative cycles. Each cycle will focus on a set of features and improvements based on feedback.

### Tools and Technologies

- **IDE:** Visual Studio
- **Version Control:** GitHub
- **Markdown Parser:** Markdig
- **UI Framework:** WPF
- **WebView Component:** WebView2

---

## Test Protocol

| TC-№ | Datum | Resultat | Tester |
| ---- | ----- | -------- | ------ |
| 1.1  |       |          |        |
| ...  |       |          |        |

---

## Test Conclusion

### Summary of Results

- **Unit Tests:** Passed all individual component tests.
- **Integration Tests:** Confirmed seamless integration of components.
- **User Acceptance Tests:** Received positive feedback from users.

### Issues and Resolutions

- **Issue:** Initial lag in real-time preview.
  - **Resolution:** Optimized the rendering pipeline.

---

## Explorative Tests

| BR-№ | Ausgangslage | Eingabe | Erwartete Ausgabe | Tatsächliche Ausgabe |
| ---- | ------------ | ------- | ----------------- | -------------------- |
| I    |              |         |                   |                      |
| ...  |              |         |                   |                      |

---

## Evaluate

### Project Review

- **Goals Met:** Successfully implemented the core functionalities and achieved a user-friendly interface.
- **User Feedback:** Positive feedback on ease of use and real-time preview feature.
- **Lessons Learned:** Importance of performance optimization and user feedback in the development process.

### Future Improvements

- **Additional Features:** Support for more advanced Markdown features (tables, footnotes).
- **Performance Enhancements:** Further optimize the rendering engine.
- **User Interface:** Refine the UI based on user feedback.

---

| Datum | Version | Zusammenfassung                                              |
| ----- | ------- | ------------------------------------------------------------ |
|       | 0.0.1   | ✍️ Jedes Mal, wenn Sie an dem Projekt arbeiten, fügen Sie hier eine neue Zeile ein und beschreiben in *einem* Satz, was Sie erreicht haben. |
|       | ...     |                                                              |
|       | 1.0.0   |                                                              |

---

By following this detailed documentation structure, you ensure clarity and comprehensiveness in your IPERKA project, facilitating seamless collaboration and progress tracking.
