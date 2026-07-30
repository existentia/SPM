# SharePoint Manager (SPM) — uplift to SharePoint Server Subscription Edition (SE)

## What this project is
SPM is a standalone WinForms tool that browses the SharePoint server-side object
model (SSOM). It runs directly on a SharePoint server with farm-admin rights,
starts at `SPFarm.Local`, and walks the object graph, rendering every object and
its properties in a tree + property grid. It is NOT a farm solution/WSP — it's a
plain WinExe that references the SSOM assemblies.

## Architecture (important)
- `Libraries/SPM2.Framework` — core framework, reflection helpers, base infra.
- `Libraries/SPM2.SharePoint` — project name `SPM2.SharePoint2010.csproj` (a legacy
  filename; the assembly is `SPM2.SharePoint` and it is NOT 2010-specific). Holds
  the base node classes (SPNode<T>, SPNodeCollection<,>) **and the bulk of the
  model** — ~360 Generated + ~390 Custom node files.
- `Libraries/SPM2.SharePoint2013` — version-DELTA model library: only the ~88 types
  new in 2013. Namespace `SPM2.SharePoint2013.Model`.
- `Libraries/SPM2.SharePointSE` — version-DELTA model library for types new in
  Subscription Edition (currently certificate management). Namespace
  `SPM2.SharePointSE.Model`.
- Model libraries are just `Model/Generated/*.cs` (auto-generated wrapper "node"
  classes, one per SSOM type) + `Model/Custom/*.cs` (hand-tuned partial-class
  overrides) + `Icons.cs`. Generated classes carry `[AdapterItemType("...")]`; custom
  classes carry `[Title]`/`[Icon]`/`[View]`/`[ExportToNode]`. There is no `[AttachTo]`
  attribute — `AttachTo` is only the name of the generator's template token.
- **Tree placement is driven by `[AdapterItemType]`, not `[ExportToNode]`.**
  `[AdapterItemType]` registers the node in IoC under the SSOM type name (everything
  after the first comma is stripped, so the pinned `Version=14.0.0.0` in the generated
  files is decorative). `SPNodeProvider` then builds children by reflecting over the
  parent's properties and resolving each property type against those registrations.
  `[ExportToNode]` is **vestigial from an abandoned MEF design and is never read** —
  a dangling one is inert, and adding one will not make a node appear.
- `Tools/SPM2.ClassGenerator` — a console app that GENERATES the model. It reflects
  over the live farm from `SPFarm.Local` and emits one Generated and one Custom file
  per type into `cs/` and `custom/` under the working directory, from
  `GeneratedNode.template` and `CustomNode.template` (also read from the working
  directory). It never overwrites an existing Custom file. It emits everything into
  the `SPM2.SharePoint.Model` namespace, so output for a delta library must be
  re-namespaced by hand.
- `SharePoint Manager 2013/Main` — the WinForms app (MainForm, tree explorer,
  property grid, context-menu commands). The delta model assemblies are NOT
  compile-time dependencies of the app's own code; they are discovered at runtime by
  `AutoLoadAssemblies` scanning the exe directory, so they must be copied there.

## Supported versions
SharePoint 2013 / 2016 / 2019 / SE only. 2007 and 2010 support was removed, along
with the `SharePoint Manager 2007`, `SharePoint Manager 2010` and
`SharePoint Manager 2010 v2` trees and the `Debug/Release 2010` configurations.
Build configurations are now `Debug|Release 2013` and `Debug|Release SE`.

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
- Keep the `Debug|Release 2013` configurations intact — they are the reference to
  diff against. Note they cannot be built on a box without the v15 ISAPI assemblies
  and the .NET Framework 4.0 targeting pack.
- The class generator only READS the farm (enumerates objects/properties) — safe
  to run. The SPM app itself CAN modify the farm via edit/save; do NOT drive any
  save/edit/delete actions against the farm. Building and launching is fine;
  a human does the interactive click-testing.
