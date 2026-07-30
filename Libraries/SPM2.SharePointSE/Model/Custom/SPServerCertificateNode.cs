/* ---------------------------
 * SharePoint Manager 2010 v2
 * Created by Carsten Keutmann
 * ---------------------------
 */

using System;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SPM2.Framework; using SPM2.SharePoint; using SPM2.SharePoint.Model;

namespace SPM2.SharePointSE.Model
{
	[Title(PropertyName="DisplayName")]
	[Icon(Small = IconsSE.DefaultSmall)][View(70)]
	[ExportToNode("SPM2.SharePoint.Model.SPFarmNode")]
	[ExportToNode("SPM2.SharePoint.Model.SPWebApplicationNode")]
	[ExportToNode("SPM2.SharePointSE.Model.SPServerCertificateCollectionNode")]
	public partial class SPServerCertificateNode
	{
        public override void Setup(ISPNode parent)
        {
            base.Setup(parent);

            try
            {
                if (this.SPObject == null) return;

                var cert = this.ServerCertificate;
                this.Text = cert.DisplayName;

                // Expiry is the thing an admin is usually looking for, so surface it on
                // the node itself rather than making them open the property grid.
                var now = DateTime.UtcNow;
                if (cert.NotAfter.ToUniversalTime() < now)
                {
                    this.Text += " (EXPIRED)";
                    this.State = "Gray";
                }

                this.ToolTipText = String.Format(
                    "Subject: {0}\r\nThumbprint: {1}\r\nValid: {2} - {3}",
                    cert.Subject,
                    cert.Thumbprint,
                    cert.NotBefore,
                    cert.NotAfter);
            }
            catch (Exception ex)
            {
                this.IconUri = SharePointContext.GetImagePath("error16by16.gif");
                this.Text = "(Error: SPServerCertificate)";
                this.ToolTipText = ex.Message;
            }
        }
	}
}
