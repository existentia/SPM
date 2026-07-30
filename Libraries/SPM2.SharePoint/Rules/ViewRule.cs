using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPM2.Framework;
using SPM2.SharePoint.Model;
using SPM2.Framework.Components;
using SPM2.Framework.IoC;

namespace SPM2.SharePoint.Rules
{
    //[Export(typeof(IRule<ISPNode>))]
    //[PartCreationPolicy(CreationPolicy.Shared)]
    //[ExportMetadata("Order", int.MaxValue)]

    [IoCLifetime(Singleton = true)]
    // Must sort LAST. Accept() always returns true, and FirstAcceptRuleEngine stops at
    // the first rule that accepts, so any rule ordered after this one can never run.
    // The parameterless [IoCOrder()] used previously defaults to int.MaxValue-1000,
    // which sorted this rule ahead of RecursiveRule (int.MaxValue-1) and left the
    // recursion guard dead - SPFarm exposes a Farm property, so the farm node nested
    // inside itself indefinitely.
    [IoCOrder(int.MaxValue)]
    public class ViewRule : IRule<ISPNode>
    {
        // Always accept this rule as it should be the last one.
        public bool Accept(ISPNode node)
        {
 	        return true;
        }

        // Check the node
        public bool Check(ISPNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            if (node is IViewRule)
            {
                return ((IViewRule)node).IsVisible();
            }

            var type = node.GetType();
            var list = type.GetCustomAttributes(true).OfType<ViewAttribute>();
            if (list.Count() == 0) return true;

            return list.Any(p => node.NodeProvider.ViewLevel >= p.Level);
        }
    }
}
