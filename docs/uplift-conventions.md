# Node conventions — follow these when fixing drift or adding nodes (Phase 3+)

This documents the original author's patterns so the SE model stays consistent with
the existing ~190 nodes. Read alongside `CLAUDE.md`.

## The two-file partial-class pattern
Every SSOM type is represented by **two** partial classes with the same name:

- `Model/Generated/XNode.cs` — auto-generated; declares the class, base type, and the
  `[AdapterItemType("<SSOM type name>")]` attribute plus wrapped properties. **Do not
  hand-edit** — it's overwritten on regeneration.
- `Model/Custom/XNode.cs` — hand-written partner partial; carries the display/behaviour
  attributes and any overrides. This is where all customization goes and it is **never**
  overwritten by the generator (it only creates a stub if none exists).

Generated nodes inherit `SPNode<T>` (single object) or `SPNodeCollection<TCollection,TItem>`
(collections). Base class: `Libraries/SPM2.SharePoint/Model/SPNode.cs`.

## Attributes on the Custom partial
From real examples (`SPFeatureNode`, `SPFileNode`, `SPDistributedCacheServiceNode`):

```csharp
[Title("SPFeature")]                    // static label, OR:
[Title(PropertyName="Name")]            // read the label from this SSOM property
[Icon(Small="BULLET.GIF")]              // SharePoint image filename (see Icons below)
[View(100)]                             // visibility threshold, NOT a sort order
[ExportToNode("SPM2.SharePoint.Model.SPFeatureCollectionNode")]  // attach under this parent
public partial class SPFeatureNode { ... }
```

- **`[ExportToNode("<fully-qualified node type name>")]`** (the `[#AttachTo#]` token in the
  generator template) is **vestigial — nothing reads it.** It survives from an abandoned
  MEF composition design: `ExportToNodeAttribute` still has its `: ExportAttribute` base
  commented out, and the `SPNodeProvider.GetChildrenTypes` method that once consumed it is
  commented out too. Treat these attributes as documentation of intent only.

  **The real hierarchy mechanism is `[AdapterItemType]` plus reflection.**
  `SPNodeProvider.LoadUnorderedChildren` walks `TypeDescriptor.GetProperties(...)` on the
  parent's SSOM type and, for each property, resolves a node by looking up the *property
  type's* full name against the IoC registrations that `[AdapterItemType]` creates.
  `LoadCollectionChildren` does the same using each item's runtime type. So a node appears
  wherever the SSOM exposes a property (or collection item) of its type — the tree mirrors
  the object model, and is not declared anywhere.

  Practical consequence: a wrong or dangling `ExportToNode` string is inert, and adding one
  will *not* make a node appear. To change where something shows up you have to change the
  node's `[AdapterItemType]`, or override `LoadChildren` on the parent.
- **`[Title]`**: `[Title("X")]` = literal; `[Title(PropertyName="Y")]` = read property Y.
  If property Y is renamed in v16, update the string.
- **`[Icon(Small=...)]`**: `"BULLET.GIF"` is treated as "no custom icon" by the base class.
  Default SharePoint layouts images resolve via `SharePointContext.GetImagePath(...)`.
- **`[View(n)]`** is a **visibility threshold, not a sort order** — siblings are sorted by
  `Text`. `ViewRule` (the only thing that reads it) shows a node when
  `NodeProvider.ViewLevel >= n`. The View menu sets that level: Minimal = 10,
  **Medium = 50 (the default)**, Full = 100.

  So `View(100)` means "Full view only" — the generator's default, and what 401 of the
  existing nodes use. `View(50)` shows at Medium; `View(1)` is reserved for the handful of
  core structural nodes (farm, web applications, service collection) that must always be
  present. **Pick this deliberately**: giving a branch's root node a level higher than the
  user's view level hides the entire subtree beneath it, with no error.

## Namespaces
- Base lib nodes: `namespace SPM2.SharePoint.Model`
- 2013 lib nodes: `namespace SPM2.SharePoint2013.Model`
- **SE lib**: pick one namespace (e.g. `SPM2.SharePointSE.Model`) and use it consistently.
  Remember `ExportToNode` targets are strings — when a SE node attaches under a base-lib
  parent, the string must be the base-lib fully-qualified name (`SPM2.SharePoint.Model.…`),
  not the SE namespace.

## Icons
`Model/Icons.cs` holds icon-name constants per version (e.g. `Icons2013.DefaultSmall = "BCCUR.GIF"`).
Add an `IconsSE` (or `Icons16`) class for the SE lib and reference its constants from
`[Icon(Small = IconsSE.DefaultSmall)]` rather than hard-coding filenames.

## Overridable behaviour (from SPNode base)
Override these in the Custom partial; **always call `base.`** first:

- `Setup(ISPNode parent)` — runs after construction. Set `Text`, `ToolTipText`, `IconUri`,
  `State` (e.g. `"Gray"` for hidden). This is where per-object labelling and **error
  handling** live.
- `LoadChildren()` — add child nodes the generator can't discover (e.g. web parts under an
  `.aspx` file in `SPFileNode`).
- `GetSPObject()`, `Update()`, `HasChildren()`, `IsDefaultSelected()` — override as needed.

Action methods (e.g. `SPFeatureNode.ActivateFeature()`/`DeactivateFeature()`) are plain
public methods invoked by context-menu command classes in the main app, not by the node
framework itself.

## Error-handling idiom (important for drift resilience)
The existing nodes wrap SSOM access in try/catch and degrade gracefully rather than
crashing the tree. Match this when adapting v16 changes:

```csharp
public override void Setup(ISPNode parent)
{
    base.Setup(parent);
    try
    {
        // ... touch SSOM members ...
    }
    catch (Exception ex)
    {
        this.IconUri = SharePointContext.GetImagePath("error16by16.gif");
        this.Text = "(Error: ...)";
        this.ToolTipText = ex.Message;
    }
}
```

## Phase 3 drift decision tree
- **Member renamed/moved in v16** → update the reference; if it can throw, guard it with the
  error idiom above rather than assuming it's present.
- **Property used in `[Title(PropertyName=...)]` renamed** → update the string.
- **Whole SSOM type removed in v16** → delete both the `Generated/` and `Custom/` files, AND
  grep for and remove every `[ExportToNode("...ThatType...")]` string elsewhere (compiler
  won't flag these).
- **New SSOM type in v16** → the generator produces the Generated file and a Custom stub; the
  stub is enough to make it appear. Only add attributes/overrides if you want custom
  labelling, icon, placement, or child loading.
