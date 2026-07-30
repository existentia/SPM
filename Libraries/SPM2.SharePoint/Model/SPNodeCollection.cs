using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using SPM2.Framework;

namespace SPM2.SharePoint.Model
{

    public class SPNodeCollection : SPNode, ISPNodeCollection
    {
        [XmlIgnore]
        public IEnumerator Pointer { get; set; }

        [XmlIgnore]
        public bool LoadingChildren { get; set; }

        public int TotalCount { get; set; }


        public SPNodeCollection()
        {
        }


        public override void Setup(ISPNode parent)
        {
            base.Setup(parent);
        }

        public override void LoadChildren()
        {
            Children.AddRange(NodeProvider.LoadCollectionChildren(this, int.MaxValue));
        }


        public override void ClearChildren()
        {
            Pointer = null;
            TotalCount = 0;
            LoadingChildren = false;
            Children.Clear();
        }

        public override bool HasChildren()
        {
            if (SPObject == null) return true;

            var collection = SPObject as SPBaseCollection;
            if (collection != null)
            {
                try
                {
                    return collection.Count > 0;
                }
                catch (Exception ex)
                {
                    // Reading Count goes to the content database. Farm administrator
                    // rights do not imply read access to every site collection, so an
                    // UnauthorizedAccessException here is ordinary rather than
                    // exceptional - SPWebCollection.EnsureWebsData throws it for a site
                    // the current account cannot read.
                    //
                    // This is called from the SPTreeNode constructor during expansion, so
                    // an escaping exception reaches the TreeView's WndProc and takes the
                    // process down. Mark the node and report no children instead.
                    Trace.WriteLine("HasChildren failed on " + GetType().Name + ": " + ex.Message);
                    State = "Gray";
                    ToolTipText = ex.Message;
                    return false;
                }
            }
            return true;
        }


    }
}