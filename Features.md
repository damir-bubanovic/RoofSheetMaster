# RoofSheetMaster – Project Roadmap

Desktop + CLI tool to calculate full-length metal roof sheets, panel layout, and material lists for single, gable, hip, and valley roofs.

This roadmap is split into **Completed** and **To-Do** chapters so it is easy to see what is done and what is coming next.

---

## ✅ Completed Chapters

### Chapter 1 — Project Setup & Architecture

- Solution structure:
  - `RoofSheetMaster.Core` – core calculation library
  - `RoofSheetMaster.Desktop` – Avalonia UI app
  - `RoofSheetMaster.Cli` – console test harness
- Core data models:
  - `RoofInput`, `Panel`, `MaterialList`
  - `RoofFace`, `HipRoof`, `ValleyRoof`
- Basic validation and error handling in the core layer

---

### Chapter 2 — Single Face Calculator & CLI Prototype

- Implement calculation for a single rectangular roof face:
  - Convert roof angle + horizontal width to slope length
  - Work out effective sheet coverage (width – overlap)
  - Determine number of sheets and their positions along the eave
- CLI app:
  - Read simple inputs from the command line
  - Print panel list with index, eave position, coverage, and sheet length
- Use CLI to verify the math and debug core logic quickly

---

### Chapter 3 — Multi-Face Roof Types (Gable / Hip / Valley)

- Extend core model to support multiple faces:
  - `RoofFace` (base face model)
  - `HipRoof` (front/back left/right faces)
  - `ValleyRoof` (upper and lower faces)
- Calculators:
  - `RoofCalculator.CalculateSimpleFace`
  - `RoofCalculator.CalculateGableRoof`
  - `RoofCalculator.CalculateHipRoof`
  - `RoofCalculator.CalculateValleyRoof`
- Each face produces its own `Panel` sequence with:
  - Face name
  - Panel index
  - Eave position
  - Effective width
  - Sheet length

---

### Chapter 4 — Desktop UI & Diagram Rendering

- Avalonia desktop UI:
  - Input fields for length, width, angle, sheet width, overlap, ridge gap
  - Roof type selector: Single / Gable / Hip / Valley
  - “Calculate” button with validation and error feedback
- Panel list display:
  - Shows face, index, eave position, coverage and sheet length
- Diagram rendering:
  - Draws each panel as a rectangle on a canvas
  - Groups panels by face
  - Scales to fit available canvas space
  - Labels rectangles with face + panel index

---

### Chapter 5 — Material Aggregation & Export

- `SheetSummary` model:
  - `SheetLength`
  - `Count`
- `MaterialList` enhancements:
  - Keeps full list of `Panel`s
  - Provides `SheetSummaries` grouped by rounded sheet length
- Desktop UI:
  - “Panels / material list” section
  - Full panel list (top)
  - Sheet summary list (grouped by length)
- CSV export:
  - Export sheet summary to CSV
  - Export full panel list to CSV
  - Uses Avalonia `StorageProvider` save dialogs

---


### Chapter 6 — Roof Model Expansion (Per-Face Dimensions)

- Allow **per-face overrides** for hip and valley roofs:
  - Separate length/width for each hip face (FrontLeft, FrontRight, BackLeft, BackRight)
  - Separate length/width for valley upper and lower faces
- UI changes:
  - Optional per-face input fields (leave blank to use main roof dimensions)
- Core changes:
  - Use overrides when present, fall back to global length/width otherwise
- Show clearly in panel list which panels belong to which face

---

### Chapter 7 — Units & Rounding

- Global units setting (metric / imperial)
- Consistent formatting for distances (e.g. 2 decimal places)
- Option to round sheet lengths to nearest increment (e.g. 5 mm or 1/8")

---


### Chapter 8 — Multiple Roof Sections

- Model multiple independent roof sections in one job
- Per-section inputs and panel lists
- Aggregated material list across all sections

---

### Chapter 9 — Flashings / Accessories (Future)

- Ridge cap length calculation
- Barge / verge flashings
- Valley tray lengths
- Basic screw/fastener estimates

---

## 🕒 To-Do Chapters

### Chapter 10 — Exporting & Reports

- Print-friendly “cut list” report
- Export reports to PDF or image (layout snapshot)
- Save/load project files

---

### Chapter 11 — Polishing & QA

- Better validation messages in the UI
- Remember last-used inputs (defaults)
- Manual testing on Linux and Windows
- Packaging for distribution

---

This file in the **repository root** is the single source of truth.  
Any previous `Features.md` / `FEATURES.md` copies under `RoofSheetMaster.Core` or elsewhere should be removed.
