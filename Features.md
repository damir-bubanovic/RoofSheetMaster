# RoofSheetMaster – Project Roadmap

This roadmap lists **all chapters**, grouped as **Completed** and **To-Do**, with clear features for each stage.

---

## ✅ Completed Chapters

### **Chapter 1 — Project Setup & Architecture**
- Core library (`RoofSheetMaster.Core`)
- Desktop UI project (Avalonia)
- CLI test harness (Program.cs)
- Shared data models:  
  `RoofInput`, `Panel`, `MaterialList`, `RoofFace`, `HipRoof`, `ValleyRoof`

---

### **Chapter 2 — Base Geometry Engine**
- Sheet effective width = SheetWidth – Overlap
- Panel spacing based on cumulative coverage
- Panel eave position calculation
- Sheet length calculation using Pythagorean slope  
  `length = RoofWidth / cos(angle)`
- Validation of inputs
- Unified `CalculateSimpleFace()` logic

---

### **Chapter 3 — Multi-Face Roof Support**
- **Gable roof** (two identical faces)
- **Hip roof** (4 identical faces – first implementation)
- **Valley roof** (upper & lower faces – identical)
- Group panels by face
- Produce unified `MaterialList` and panel collection

---

### **Chapter 4 — Desktop UI (Avalonia)**
- Input section: roof type, length, width, angle, sheet width, overlap, ridge gap
- Panel list with scroll support
- Simple roof layout diagram using `Canvas`
- Proper face grouping for drawing
- Responsive layout (left input column + right diagram column)
- Borders, styling, spacing
- Bug fixes:
  - Containers overflowing  
  - Dropdown alignment  
  - Panels section cutoff  
  - Diagram bottom cutoff  
  - Margins and visual structure  
  - Text wrapping  
  - Scaling logic improvements

---

## 📌 To-Do Chapters (Next Steps)

### **Chapter 5 — Material Aggregation & Export**
- Summarize panels by identical sheet lengths  
  → “16.771 ft × 28 pcs”, “12.350 ft × 8 pcs”
- Add `SheetSummary` model
- Display material list in UI (new section)
- Export sheet summary to **CSV**
- Optional: Export full panel list to CSV

---

### **Chapter 6 — Roof Model Expansion**
- Allow **different dimensions per face** for:
  - Hip roofs
  - Valley roofs
- UI redesign for per-face inputs  
  (collapsible face editors or grid/table inputs)

---

### **Chapter 7 — Full Diagram Improvements**
- Show ridge line(s), hip lines, valley lines
- Draw each face proportionally
- Better panel labels (2-line labels)
- Highlight selected panel in list + diagram sync
- Zoom/pan or scale to window size

---

### **Chapter 8 — Tapered Roof Support**
- Implement tapered panel widths  
  (eave width → ridge width)
- Special layout rules
- Update diagram to draw trapezoid sheets
- Integrate into material calculator

---

### **Chapter 9 — Save/Load Projects**
- Save project JSON file
- Load project from file
- Include all inputs + panel results

---

### **Chapter 10 — Print/PDF Export**
- Create print-ready layout:
  - Roof diagram
  - Panel list
  - Sheet summary
  - Job/project metadata
- Export to **PDF** or **PNG** snapshot

---

### **Chapter 11 — Advanced Features (Optional Futures)**
- Profile-based sheet selection (metal, polycarbonate, etc.)
- Local measurement units (ft/inch, metric, mixed)
- Price estimator (material cost)
- On-roof cut optimization (panel reuse)
- Custom eave/ridge offsets
- Truss spacing helpers
- Full 3D visualization (stretch goal)

