using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Parcel.Neo.Shared.DataTypes;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Shared
{
    public class WebHostRuntime
    {
        #region Constructor
        public WebHostRuntime()
        {
            if (Singleton == null)
                Singleton = this;
            else
            {
                throw new InvalidOperationException("WebHostRuntime is already initialized! Singleton is not null.");
            }
        }
        public static WebHostRuntime Singleton { get; set; }
        #endregion

        #region Properties
        public int Port { get; set; }
        public string Url { get; set; }
        public bool ShouldLog { get; set; }
        #endregion

        #region Accessor - Endpoints
        public string BaseUrl => Url;
        public string LocalIPUrl =>
            BaseUrl.Contains("localhost") ? BaseUrl.Replace("localhost", GetLocalIPAddress()) : BaseUrl;
        public string LocalIPAddress => GetLocalIPAddress();
        public void Open()
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(Url)
                {
                    UseShellExecute = true
                }
            }.Start();
        }
        public void Open(string adress)
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(adress)
                {
                    UseShellExecute = true
                }
            }.Start();
        }
        public void OpenTarget(string target)
        {
            new Process
            {
                StartInfo = new ProcessStartInfo($"{Url}/{target}")
                {
                    UseShellExecute = true
                }
            }.Start();
        }
        #endregion

        #region Interoperation
        public ProcessorNode LastNode { get; set; }
        public ServerConfig CurrentLayout { get; set; }
        public Dictionary<string, DataGrid> DataTableEndPoints { get; set; } = new Dictionary<string, DataGrid>();
        #endregion
        
        #region Routile
        private string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        #endregion
    }
}