/* ---------------------------
 * SharePoint Manager 2010 v2
 * Created by Carsten Keutmann
 * ---------------------------
 */

using System;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SPM2.Framework;

namespace SPM2.SharePoint.Model
{
	// This node represents any System.Guid property, not one particular one. It is
	// resolved by SPNodeProvider from the property type, so its label comes from the
	// property it hangs off ("Id", "TraceSessionGuid", ...).
	//
	// It used to carry [Title("Products")] and an [ExportToNode] naming
	// "<get_Products>d__0Node". Both were artefacts of the class generator reflecting
	// over SPFarm.Products, an iterator property: it reached the compiler-generated
	// state machine behind the iterator and treated it as a real type. No such node
	// could ever be written, and "Products" is wrong for a general Guid node - it
	// would mislabel any Guid appearing under a collection node. The generator no
	// longer emits either (see Tools/SPM2.ClassGenerator/TypesCollection.cs).
	//
	// SPFarm.Products itself is still not shown: it is IEnumerable<Guid>, and no node
	// is registered for that type, so SPNodeProvider skips it. Surfacing it would be a
	// new feature, not a fix.
	[Icon(Small="BULLET.GIF")][View(100)]
	public partial class GuidNode
	{
	}
}
