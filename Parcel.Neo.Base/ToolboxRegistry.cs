using System;
using System.Collections.Generic;
using System.Reflection;

namespace Parcel.Neo.Base
{
    public class ToolboxRegistry
    {
        #region Data

        public Dictionary<string, Assembly> Toolboxes { get; set; } = new Dictionary<string, Assembly>();
        #endregion
        
        #region Interface
        public void RegisterToolbox(string name, Assembly Assembly)
        {
            if (Toolboxes.ContainsKey(name))
                throw new InvalidOperationException($"Assembly `{Assembly.FullName}` is already registered.");
            
            Toolboxes.Add(name, Assembly);
        }
        #endregion
    }
}