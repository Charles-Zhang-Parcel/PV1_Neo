﻿using System;
using System.Net;
using System.Net.Sockets;
using Parcel.Shared;
using Parcel.Shared.DataTypes;
using Parcel.Shared.Framework;
using Parcel.Shared.Framework.ViewModels;
using Parcel.Shared.Framework.ViewModels.BaseNodes;
using Parcel.Toolbox.Finance;

namespace Parcel.FrontEnd.NodifyWPF.SpecialNodes
{
    public class HostAddress: ProcessorNode
    {
        #region Node Interface
        public HostAddress()
        {
            Title = NodeTypeName = "Host Address";

            if (WebHostRuntime.Singleton != null)
            {
                WebAccessPointUrl = WebHostRuntime.Singleton.BaseUrl;
                if (_webAccessPointUrl != WebHostRuntime.Singleton.LocalIPUrl)
                    LocalIPUrl = WebHostRuntime.Singleton.LocalIPUrl;   
            }
        }
        #endregion
        
        #region View Properties
        private string _webAccessPointUrl;
        public string WebAccessPointUrl { get => _webAccessPointUrl; set => SetField(ref _webAccessPointUrl, value); }
        private string _localIPUrl;
        public string LocalIPUrl { get => _localIPUrl; set => SetField(ref _localIPUrl, value); }
        #endregion
        
        #region Processor Interface
        public override OutputConnector MainOutput => null;
        public override NodeExecutionResult Execute()
        {
            Message.Content = string.Empty;
            Message.Type = NodeMessageType.Normal;
            
            return new NodeExecutionResult(true, null);
        }
        #endregion
    }
}