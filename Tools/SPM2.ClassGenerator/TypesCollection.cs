using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPM2.Framework;
using System.Reflection;

namespace SPM2.ClassGenerator
{
    public class TypesCollection : Dictionary<string, NodeDescriptor>
    {

        public bool Add(NodeDescriptor descriptor, NodeDescriptor parent)
        {
            // Compiler-generated types cannot be represented as node classes. Reject
            // them before linking to the parent: otherwise UpdateAttachTo still emits
            // an [ExportToNode] naming a node that can never be written, leaving a
            // dangling reference the compiler cannot catch. That is how the existing
            // model ended up with "<get_Products>d__0Node".
            if (descriptor.IsCompilerGenerated)
            {
                return false;
            }

            // Generic types are named like "KeyValuePair`2". The backtick is legal in a
            // filename but not in a C# identifier, so the emitted class would not
            // compile. Closed generics that are worth showing (for example
            // Dictionary<SPUrlZone, SPIisSettings>) are hand-written nodes instead.
            if (descriptor.IsGeneric)
            {
                return false;
            }

            if (parent != null)
            {
                parent.Children.Add(descriptor);
            }

            if (!this.ContainsKey(descriptor.NodeFullName))
            {
                this.Add(descriptor.NodeFullName, descriptor);
                //Console.WriteLine("Type added:" + descriptor.NodeFullName);
                return true;
            }
            return false;
        }



        public NodeDescriptor GetOrCreate(Type itemType)
        {
            return GetOrCreate(new NodeDescriptor(itemType));
            
        }

        public NodeDescriptor GetOrCreate(PropertyInfo info, NodeDescriptor parentDescriptor)
        {
            return GetOrCreate(info, parentDescriptor.GetPropertyObject(info), parentDescriptor);
        }


        public NodeDescriptor GetOrCreate(PropertyInfo info, object instance, NodeDescriptor parentDescriptor)
        {
            return GetOrCreate(new NodeDescriptor(info, instance));
        }

        public NodeDescriptor GetOrCreate(NodeDescriptor descriptor)
        {
            NodeDescriptor result = descriptor;
            if (this.ContainsKey(result.NodeFullName))
            {
                result = this[result.NodeFullName];
            }

            return result;
        }


        //public bool Append(string title, Type propertyType, Type parentType)
        //{
            
        //    bool isNew = false;
        //    AttachToCollection attachTo = null;
        //    if (this.ContainsKey(propertyType))
        //    {
        //        attachTo = this[propertyType];
        //    }
        //    else
        //    {
        //        attachTo = new AttachToCollection(title, propertyType);
        //        this.Add(propertyType, attachTo);
        //        isNew = true;
        //        Console.WriteLine("Type added:" + propertyType.Name + "(" + title + ")");
        //    }

        //    // Only attach to the parent if the propertyType is not defined in the chain before.
        //    if (!IsRecursive(propertyType, parentType))
        //    {
        //        attachTo.Append(parentType);
        //    }

        //    return isNew;
        //}



    }
}
