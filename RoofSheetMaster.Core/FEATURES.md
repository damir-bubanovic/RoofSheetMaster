# RoofSheetMaster Feature Roadmap

## Chapter 1 — Core Foundation

* Define project structure and solution layout
* Create core calculation library
* Implement basic roof face model (rectangular)
* Implement sheet coverage calculation
* Implement slope length calculation
* Add validation for inputs

## Chapter 2 — CLI Prototype

* Build console application for quick testing
* Test inputs and output formatting
* Verify calculation correctness
* Add sample scenarios for debugging

## Chapter 3 — Desktop Application Setup

* Initialize Avalonia UI project
* Connect UI project with core library
* Implement main window structure
* Add input controls for basic roof parameters
* Display calculated sheet list in UI

## Chapter 4 — Multi-Face Roof Support

* Extend model to support two roof faces (gable)
* Add combined material list
* Update UI for multi-face selection
* Handle per-face input overrides

## Chapter 5 — Hip Roof Implementation

* Add hip roof geometry model
* Implement hip slope calculations
* Compute sheet cut angles and lengths
* Render layout diagram for hip roof

## Chapter 6 — Valley Roof Implementation

* Add valley geometry model
* Calculate valley-side sheet cuts
* Handle asymmetric valley conditions
* Visualize valley layout in UI

## Chapter 7 — Cut Optimization

* Implement offcut tracking
* Suggest reuse of cut pieces
* Display optimized material list
* Provide warnings for unusable offcuts

## Chapter 8 — Layout Visualization

* Build roof-view rendering canvas
* Display panels with scale
* Draw hips, valleys, ridge, and eaves
* Add zoom and pan interactions

## Chapter 9 — Importing Blueprints

* Support image import (PNG/JPG)
* Implement scaling calibration tool
* Allow measurement by clicking points
* Auto-populate roof dimensions

## Chapter 10 — Exporting & Reports

* Export cut list to PDF
* Export layout diagram to image
* Save/load project files
* Provide print-friendly output

## Chapter 11 — Polishing & QA

* Add error messages and validation UI
* Add settings (units, defaults)
* Manual testing across Linux/Windows
* Prepare for packaging and distribution
