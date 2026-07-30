/* ---------------------------
 * SharePoint Manager 2010 v2
 * Created by Carsten Keutmann
 * ---------------------------
 */

using System;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SPM2.Framework; using SPM2.SharePoint.Model;

namespace SPM2.SharePointSE.Model
{
	[Title(PropertyName="DisplayName")]
	// View(100) - Full view only. This node comes from SPServerCertificate.Store, a back
	// reference that loops Certificate -> Store -> Certificates -> Certificate. The four
	// concrete stores are reached from the manager instead, so this is noise at the
	// default view level.
	[Icon(Small = IconsSE.DefaultSmall)][View(100)]
	[ExportToNode("SPM2.SharePointSE.Model.SPServerCertificateNode")]
	public partial class SPServerCertificateStoreNode
	{
	}
}
