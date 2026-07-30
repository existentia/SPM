# SharePoint Manager

A standalone Windows desktop tool for browsing the SharePoint **server-side object model
(SSOM)**. It starts at `SPFarm.Local`, walks the object graph, and renders every object and
its properties in a tree plus property grid.

It is not a farm solution or WSP — it is a plain WinForms executable that references the
SSOM assemblies, so it must be run **on a SharePoint server**, signed in as an account with
farm administrator rights.

> **It can write, not just read.** The property grid supports editing and saving, and the
> context menus include destructive actions. Of those, only *Delete* asks for confirmation —
> **feature deactivation and file recycling act immediately, with no prompt.** Treat it as a
> live administrative tool and be careful what you click on a production farm.

## Supported versions

| SharePoint | Assembly line | Build configuration | Status |
| --- | --- | --- | --- |
| Subscription Edition | v16 (`\16\ISAPI`) | `Debug SE` / `Release SE` | Built and run against SE |
| 2019 | v16 | `Debug SE` / `Release SE` | Expected to work, untested |
| 2016 | v16 | `Debug SE` / `Release SE` | Expected to work, untested |
| 2013 | v15 (`\15\ISAPI`) | `Debug 2013` / `Release 2013` | Legacy, kept as a reference |

2016, 2019 and SE all report version `16.0`, and the SE configuration references the v16
assemblies, so the same binary should serve all three. Only SE has actually been exercised.
The application distinguishes them at runtime from the build number of
`Microsoft.SharePoint.dll` (2016 from `16.0.4327`, 2019 from `16.0.10337`, SE from
`16.0.14326`) and titles itself accordingly.

The `2013` configurations are retained so changes can be diffed against the pre-uplift
behaviour. They need the v15 ISAPI assemblies and the .NET Framework 4.0 targeting pack, so
they will not build on a machine that only has SE installed.

**SharePoint 2007 and 2010 are no longer supported**, and their projects have been removed
from the repository.

## Building

Requirements:

- A SharePoint server with the matching version installed — the SSOM assemblies are
  resolved from the local install, so the farm binaries have to be present.
- Visual Studio 2022 or later, or just MSBuild. These are legacy, non-SDK `.csproj` files:
  use **`msbuild`**, not `dotnet build`.
- .NET Framework **4.8** targeting pack.
- Build **x64** — SharePoint is 64-bit only. The SE configurations already set
  `PlatformTarget=x64` with `Prefer32Bit` off.

From a Developer Command Prompt:

```
msbuild "SharePoint Manager 2013\SharePoint Manager Solution.sln" /p:Configuration="Release SE" /p:Platform="Any CPU"
```

The executable lands in `SharePoint Manager 2013\Main\bin\SharePoint Manager SE\`.
Use `Debug SE` for a debug build, which outputs to `Main\bin\Debug SE\` instead.

## Repository layout

```
Libraries/
  SPM2.Framework          Core framework: reflection helpers, IoC, XML, base infrastructure
  SPM2.SharePoint         Base node classes AND the bulk of the model (367 + 399 files)
  SPM2.SharePoint2013     Delta model: types introduced in 2013 (88 + 88)
  SPM2.SharePointSE       Delta model: types introduced in SE, e.g. certificate management (9 + 9)
SharePoint Manager 2013/
  Main                    The WinForms application
Tools/
  SPM2.ClassGenerator     Console app that generates the model from a live farm
docs/                     Node conventions and an enhancements backlog
```

Some directory and project names are historical: `SPM2.SharePoint2010.csproj` builds the
`SPM2.SharePoint` assembly and is not 2010-specific, and the application still lives under
`SharePoint Manager 2013`. Neither reflects a version constraint.

### How the model works

Each SSOM type is represented by two partial classes with the same name:

- `Model/Generated/XNode.cs` — generated; carries `[AdapterItemType("<SSOM type>")]` and a
  typed property. Do not hand-edit; it is overwritten on regeneration.
- `Model/Custom/XNode.cs` — hand-written partner; carries `[Title]`, `[Icon]`, `[View]` and
  any behaviour overrides. Never overwritten by the generator.

Tree placement is **not** declared anywhere. `SPNodeProvider` reflects over the parent
object's properties and resolves a node for each by matching the property's type against the
IoC registrations that `[AdapterItemType]` creates, so the tree mirrors the object model.
(`[ExportToNode]` attributes are present throughout but are vestigial from an abandoned MEF
design and are never read.)

See `docs/uplift-conventions.md` before adding or changing nodes.

### Regenerating the model

`Tools/SPM2.ClassGenerator` reflects over the live farm and emits one Generated and one
Custom file per discovered type. It only reads the farm. Run it from a working directory
containing `GeneratedNode.template` and `CustomNode.template`; output is written to `cs/` and
`custom/` beneath it. Existing Custom files are never overwritten, so hand-tuning survives
regeneration.

Its output is not drop-in: everything is emitted into the `SPM2.SharePoint.Model` namespace,
so anything destined for a delta library has to be re-namespaced by hand.

## Credits

Originally created by **Carsten Keutmann**, with contributions from Anders Dissing, and
localization by Gustavo Velez and Andreas Kviby. Previously hosted on CodePlex.

## Licence

MIT — see [LICENSE](LICENSE).
