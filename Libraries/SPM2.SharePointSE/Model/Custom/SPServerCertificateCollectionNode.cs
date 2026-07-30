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
	[Title("Certificates")]
	[Icon(Small = IconsSE.DefaultSmall)][View(50)]
	[ExportToNode("SPM2.SharePointSE.Model.SPServerCertificateStoreNode")]
	[ExportToNode("SPM2.SharePointSE.Model.SPServerRootCertificateStoreNode")]
	[ExportToNode("SPM2.SharePointSE.Model.SPServerIntermediateCertificateStoreNode")]
	[ExportToNode("SPM2.SharePointSE.Model.SPServerEndEntityCertificateStoreNode")]
	[ExportToNode("SPM2.SharePointSE.Model.SPServerPendingCertificateStoreNode")]
	public partial class SPServerCertificateCollectionNode
	{
	}
}
