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
	// View(1): this is the entry point for the whole certificate branch under the farm,
	// so it has to be visible at the default Medium view level (50) - the same level as
	// SPWebApplicationNode and SPServiceCollectionNode. At View(100) it only appeared
	// under View > Full, which hid everything beneath it too.
	[Icon(Small = IconsSE.DefaultSmall)][View(1)]
	[ExportToNode("SPM2.SharePoint.Model.SPFarmNode")]
	public partial class SPServerCertificateManagerNode
	{
	}
}
