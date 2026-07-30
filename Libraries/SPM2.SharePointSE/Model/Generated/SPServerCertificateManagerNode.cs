/* ---------------------------
 * SharePoint Manager 2010 v2
 * Created by Carsten Keutmann
 * ---------------------------
 */

using System;

using Microsoft.SharePoint.Administration.CertificateManagement;
using SPM2.Framework; using SPM2.SharePoint.Model;

namespace SPM2.SharePointSE.Model
{
	[AdapterItemType("Microsoft.SharePoint.Administration.CertificateManagement.SPServerCertificateManager, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c")]
	public partial class SPServerCertificateManagerNode : SPNode
	{
		[System.Xml.Serialization.XmlIgnore]
        public SPServerCertificateManager ServerCertificateManager
        {
            get
            {
                return (SPServerCertificateManager)this.SPObject;
            }
            set
            {
                this.SPObject = value;
            }
        }
	}
}
