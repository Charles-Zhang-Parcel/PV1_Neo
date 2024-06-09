using System;
using System.Collections.Generic;

namespace Parcel.Neo.Shared
{
    public partial class ParcelRuntime
    {
        #region Constructor
        public ParcelRuntime()
        {
            if (Singleton == null)
                Singleton = this;
            else
            {
                throw new InvalidOperationException("RuntimeData is already initialized! Singleton is not null.");
            }
        }
        
        public static ParcelRuntime Singleton { get; private set; }
        #endregion
        
        #region Settings and Temporary Data
        public ApplicationConfiguration Configuration { get; set; }
        #endregion
        
        #region Opened Database
        #endregion
    }
}