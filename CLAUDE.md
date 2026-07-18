# SharePoint Manager (SPM) — uplift to SharePoint Server Subscription Edition (SE)

## What this project is
SPM is a standalone WinForms tool that browses the SharePoint server-side object
model (SSOM). It runs directly on a SharePoint server with farm-admin rights,
starts at `SPFarm.Local`, and walks the object graph, rendering every object and
its properties in a tree + property grid. It is NOT a farm solution/WSP — it's a
plain WinExe that references the SSOM assemblies.

## Architecture (important)
- `Libraries/SPM2.Framework` — core framework, reflection helpers, base infra.
- `Libraries/SPM2.SharePoint` — project name `SPM2.SharePoint2010.csproj`; base
  node classes (SPNode<T>, SPNodeCollection<,>) that the model builds on.
- `Libraries/SPM2.SharePoint2013` — the version-specific MODEL library. Nearly
  empty of logic: it's just `Model/Generated/*.cs` (~95 auto-generated wrapper
  "node" classes, one per SSOM type) + `Model/Custom/*.cs` (~95 hand-tuned
  partial-class overrides) + `Icons.cs`. Nodes are discovered at runtime by
  attributes: generated classes carry `[AdapterItemType("...")]`, custom classes
  carry `[AttachTo]`/`[Title]`/`[Icon]`.
- `SharePoint Manager 2010 v2/SPM2.ClassGenerator` — a console app that GENERATES
  the model. It reflects over the live farm from `SPFarm.Local` and emits one
  Generated and one Custom file per type, from `GeneratedNode.template` and
  `CustomNode.template` (read from the working directory). It only writes a Custom
  stub if one does not already exist, so hand-customizations are preserved across
  regeneration.
- `SharePoint Manager 2013/Main` — the WinForms app (MainForm, tree explorer,
  property grid, context-menu commands). References the 2013 model lib.

## Uplift goal
Produce a v16 model that works against SharePoint Server Subscription Edition.
SE is the v16 assembly line (same major version as 2016/2019): assembly
`Microsoft.SharePoint, Version=16.0.0.0`, located at
`C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\16\ISAPI\`.
The 2013 project uses `Version=15.0.0.0` from the `\15\ISAPI\` path.

## Environment
- Build on this box with MSBuild from a VS2022 Developer prompt (these are legacy,
  non-SDK csproj files — use `msbuild`, not `dotnet build`).
- Target **.NET Framework 4.8**. Build **x64** (or AnyCPU with Prefer-32-bit OFF)
  — SharePoint is 64-bit only.

## Guardrails
- Do all work on a git branch; never commit to the original branch.
- Keep the original `SharePoint Manager 2013` and `SPM2.SharePoint2013` projects
  intact — they are the reference to diff and copy from.
- The class generator only READS the farm (enumerates objects/properties) — safe
  to run. The SPM app itself CAN modify the farm via edit/save; do NOT drive any
  save/edit/delete actions against the farm. Building and launching is fine;
  a human does the interactive click-testing.
