using Parcel.CoreEngine.Runtime;
using Parcel.Neo.Base.DataTypes;
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
        public RuntimeNodeType NodeType { get; }
        #endregion

        #region Payload Type
        private NodeImplementationType ImplementationType { get; }
        private MethodInfo Method { get; }
        private AutomaticNodeDescriptor Descriptor { get; }
        private Type ProcessorNodeType { get; }
        #endregion

        #region Constructor
        public ToolboxNodeExport(string name, RuntimeNodeType nodeType, MethodInfo method)
        {
            Name = name;
            NodeType = nodeType;
            Method = method;
            ImplementationType = NodeImplementationType.MethodInfo;
        }
        public ToolboxNodeExport(string name, RuntimeNodeType nodeType, AutomaticNodeDescriptor descriptor)
        {
            Name = name;
            NodeType = nodeType;
            Descriptor = descriptor;
            ImplementationType = NodeImplementationType.AutomaticLambda;
        }
        public ToolboxNodeExport(string name, RuntimeNodeType nodeType, Type type)
        {
            Name = name;
            NodeType = nodeType;
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
                    return (BaseNode)Activator.CreateInstance(ProcessorNodeType);
                case NodeImplementationType.MethodInfo:
                    Type[] parameterTypes = Method.GetParameters().Select(p => p.ParameterType).ToArray();
                    Type returnType = Method.ReturnType;
                    // TODO: Replace with some more suitable implementation (e.g. a custom class specialized in handling those)
                    if (Method.IsStatic)
                        return new AutomaticProcessorNode(new AutomaticNodeDescriptor(Name,
                            parameterTypes.Select(CacheTypeHelper.ConvertToCacheDataType).ToArray(),
                            CacheTypeHelper.ConvertToCacheDataType(returnType),
                            objects => Method.Invoke(null, objects))
                        {
                            InputNames = Method.GetParameters().Select(p => p.Name).ToArray()
                        });
                    else
                        return new AutomaticProcessorNode(new AutomaticNodeDescriptor(Name,
                            [CacheTypeHelper.ConvertToCacheDataType(Method.DeclaringType), .. parameterTypes.Select(CacheTypeHelper.ConvertToCacheDataType)],
                            returnType == typeof(void) ? CacheTypeHelper.ConvertToCacheDataType(Method.DeclaringType) : CacheTypeHelper.ConvertToCacheDataType(returnType),
                            objects => Method.Invoke(objects[0], objects.Skip(1).ToArray()))
                            {
                                InputNames = [Method.DeclaringType.Name, .. Method.GetParameters().Select(p => p.Name)]
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