﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace EPMLiveEnterprise.EPMLivePortfolioEngine {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="PortfolioEngineAPISoap", Namespace="PortfolioEngine")]
    public partial class PortfolioEngineAPI : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback ExecuteOperationCompleted;
        
        private System.Threading.SendOrPostCallback ExecuteJSONOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public PortfolioEngineAPI() {
            this.Url = "http://epmdevps/sites/PS2013/_vti_bin/portfolioengine.asmx";
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event ExecuteCompletedEventHandler ExecuteCompleted;
        
        /// <remarks/>
        public event ExecuteJSONCompletedEventHandler ExecuteJSONCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("PortfolioEngine/Execute", RequestNamespace="PortfolioEngine", ResponseNamespace="PortfolioEngine", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Execute(string Function, string Dataxml) {
            object[] results = this.Invoke("Execute", new object[] {
                        Function,
                        Dataxml});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ExecuteAsync(string Function, string Dataxml) {
            this.ExecuteAsync(Function, Dataxml, null);
        }
        
        /// <remarks/>
        public void ExecuteAsync(string Function, string Dataxml, object userState) {
            if ((this.ExecuteOperationCompleted == null)) {
                this.ExecuteOperationCompleted = new System.Threading.SendOrPostCallback(this.OnExecuteOperationCompleted);
            }
            this.InvokeAsync("Execute", new object[] {
                        Function,
                        Dataxml}, this.ExecuteOperationCompleted, userState);
        }
        
        private void OnExecuteOperationCompleted(object arg) {
            if ((this.ExecuteCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ExecuteCompleted(this, new ExecuteCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("PortfolioEngine/ExecuteJSON", RequestNamespace="PortfolioEngine", ResponseNamespace="PortfolioEngine", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ExecuteJSON(string Function, string Dataxml) {
            object[] results = this.Invoke("ExecuteJSON", new object[] {
                        Function,
                        Dataxml});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ExecuteJSONAsync(string Function, string Dataxml) {
            this.ExecuteJSONAsync(Function, Dataxml, null);
        }
        
        /// <remarks/>
        public void ExecuteJSONAsync(string Function, string Dataxml, object userState) {
            if ((this.ExecuteJSONOperationCompleted == null)) {
                this.ExecuteJSONOperationCompleted = new System.Threading.SendOrPostCallback(this.OnExecuteJSONOperationCompleted);
            }
            this.InvokeAsync("ExecuteJSON", new object[] {
                        Function,
                        Dataxml}, this.ExecuteJSONOperationCompleted, userState);
        }
        
        private void OnExecuteJSONOperationCompleted(object arg) {
            if ((this.ExecuteJSONCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ExecuteJSONCompleted(this, new ExecuteJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void ExecuteCompletedEventHandler(object sender, ExecuteCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ExecuteCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ExecuteCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void ExecuteJSONCompletedEventHandler(object sender, ExecuteJSONCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ExecuteJSONCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ExecuteJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591