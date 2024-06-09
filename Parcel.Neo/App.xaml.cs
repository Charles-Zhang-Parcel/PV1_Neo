using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Parcel.Shared;
// using Parcel.WebHost;

namespace Parcel.Neo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Events
        private void ParcelApplicationInitialize(object sender, StartupEventArgs e)
        {
            // Entrance.SetupAndRunWebHost();
        }
        #endregion
    }
}