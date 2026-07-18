# Enhancements backlog

Candidate improvements to make **after** the SE uplift is running (Phases 1–5 in
`CLAUDE.md`). This is a menu to prune, not a committed plan. Value/effort are rough.

Legend: **Free** = mostly comes from regeneration; **Work** = real implementation.

## Tier 1 — high value, modest effort

### Tree search / "go to object"  — Work · high value
Resolve a site collection / web / list by **URL or GUID** and navigate the tree to it,
instead of hand-expanding. The node model already carries `Url` and `ID`, so half the
plumbing exists.

### Global read-only / safe mode  — Small · high value
The tool runs as farm admin and can write. Add a top-level toggle that disables
edit/save/delete, plus a confirmation prompt on the destructive context-menu commands
(feature deactivate, file recycle, generic delete). Turns a foot-gun into a safe
exploration tool.

### Diagnostics panel for failed nodes  — Small–medium
Load errors are currently swallowed in `catch {}` blocks throughout `Setup`. A panel
listing "nodes that failed to load and why" helps admins and makes future uplifts far
easier (API drift becomes visible instead of silent).

## Tier 2 — strong admin value

### Two-object / two-farm compare  — Medium–large · high value
Select two webs, site collections, or service apps and diff their properties side by
side. Extend to dev-vs-prod across farms for a genuine ops tool. `SPNode` is already
`[Serializable]` with XML plumbing as a foundation.

### Export / "document the farm"  — Medium
Serialize a node's properties (or a subtree) to JSON/CSV/XML for documentation, change
records, or drift baselines. Reuses the existing XML serialization infra.

### "Copy as PowerShell / PnP / CSOM"  — Medium
Right-click an object → generate the SharePoint Management Shell or PnP snippet to fetch
or reproduce it. Bridges the explorer to the scripting admins automate with.

## Tier 3 — SE object-model coverage (mostly Free)

Because the app is SSOM-reflection-driven, most SE-era objects appear automatically after
regeneration (new service apps, health/telemetry, updated auth). Optional work is adding
*rich* custom nodes (labels/icons/actions) for the ones worth first-class treatment:
- Modern auth (OIDC), App Catalog / SPFx solution catalog, sensitivity-label/IRM settings,
  Fast Site Creation, hybrid config.

Treat as opportunistic — do it for objects you actually manage, driven by what looks
unlabelled/ugly in the tree after Phase 5.

## Tier 4 — bigger bets (evaluate later)

### Single v16 build for 2016/2019/SE with runtime version detection  — Medium
They're all v16; structure the model so one binary serves all three, detecting the farm
build at startup. Useful for a mixed estate.

### Out-of-process / remote architecture  — Large
Keep an SSOM worker on .NET Framework on the server; build a modern front-end
(.NET 8 WPF/Avalonia, or web) that talks to it. The only path to a modern UI **and** to
running the console from a workstation instead of RDP. A project in its own right — park
until the straight uplift is proven.

### High-DPI / dark mode / packaging  — Small–medium
WinForms high-DPI fixes on 4.8, code-signed exe, GitHub Releases. Polish; do last.

## Suggested sequencing
1. Get the uplift green (Phases 1–5).
2. Read-only mode + diagnostics panel (cheap, protective, make everything after safer).
3. Tree search.
4. Pick between compare and export based on which you'd use weekly.
5. Defer the out-of-process rewrite unless a modern/remote UI becomes a hard requirement.
