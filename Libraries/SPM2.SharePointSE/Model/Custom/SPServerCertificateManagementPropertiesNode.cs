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
	[Title("Certificate Management Settings")]
	[Icon(Small = IconsSE.DefaultSmall)][View(5)]
	[ExportToNode("SPM2.SharePointSE.Model.SPServerCertificateManagerNode")]
	public partial class SPServerCertificateManagementPropertiesNode
	{
	}
}
