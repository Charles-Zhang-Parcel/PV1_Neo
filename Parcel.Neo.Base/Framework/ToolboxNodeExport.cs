using Humanizer; // TODO: Remove thsi dependency on Parcel.Neo.Base
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using System;
using System.Linq;
using System.Reflection;

namespace Parcel.Neo.Base.Framework
{
    public class ToolboxNodeExport
    {
        private enum NodeImplementationType
        {
            OOPNode,
            MethodInfo,
            AutomaticLambda
        }

        #region Attributes
        public string Name { get; }
        #endregion

        #region Payload Type
        private NodeImplementationType ImplementationType { get; }
        private MethodInfo Method { get; }
        private AutomaticNodeDescriptor Descriptor { get; }
        private Type ProcessorNodeType { get; }
        #endregion

        #region Constructor
        public ToolboxNodeExport(string name, MethodInfo method)
        {
            Name = name;
            Method = method;
            ImplementationType = NodeImplementationType.MethodInfo;
        }
        public ToolboxNodeExport(string name, AutomaticNodeDescriptor descriptor)
        {
            Name = name;
            Descriptor = descriptor;
            ImplementationType = NodeImplementationType.AutomaticLambda;
        }
        public ToolboxNodeExport(string name, Type type)
        {
            Name = name;
            ProcessorNodeType = type;
            ImplementationType = NodeImplementationType.OOPNode;
        }
        #endregion

        #region Method
        public BaseNode InstantiateNode()
        {
            switch (ImplementationType)
            {
                case NodeImplementationType.OOPNode:
                    // TODO: We can use automatic node to invoke constructors for types that have constructor
                    return (BaseNode)Activator.CreateInstance(ProcessorNodeType);
                case NodeImplementationType.MethodInfo:
                    Type[] parameterTypes = Method.GetParameters().Select(p => p.ParameterType).ToArray();
                    Type returnType = Method.ReturnType;
                    // TODO: Replace with some more suitable implementation (e.g. a custom class specialized in handling those)
                    if (Method.IsStatic)
                        return new AutomaticProcessorNode(new AutomaticNodeDescriptor(Name, parameterTypes, returnType, objects => Method.Invoke(null, objects))
                        {
                            InputNames = Method.GetParameters().Select(p => p.Name.Titleize()).ToArray(),
                            DefaultInputValues = Method.GetParameters().Select(p => p.DefaultValue).ToArray()
                        });
                    else
                        return new AutomaticProcessorNode(new AutomaticNodeDescriptor(Name,
                            [Method.DeclaringType, .. parameterTypes],
                            returnType == typeof(void) ? Method.DeclaringType : returnType,
                            objects => Method.Invoke(objects[0], objects.Skip(1).ToArray()))
                            {
                                InputNames = [Method.DeclaringType.Name, .. Method.GetParameters().Select(p => p.Name.Titleize())],
                                DefaultInputValues = Method.GetParameters().Select(p => p.DefaultValue).ToArray()
                            }); // TODO: Finish implementation; Likely we will require a new custom node descriptor type to handle this kind of behavior))
                case NodeImplementationType.AutomaticLambda:
                    return new AutomaticProcessorNode(Descriptor);
                default:
                    throw new ApplicationException("Invalid implementation type.");
            }
        }
        #endregion
    }
}