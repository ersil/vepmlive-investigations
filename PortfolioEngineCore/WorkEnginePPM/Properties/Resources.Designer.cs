﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkEnginePPM.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WorkEnginePPM.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;table id=&quot;idWorkspaceTable&quot; width=&quot;100%&quot; border=&quot;0&quot; cellpadding=&quot;0&quot; cellspacing=&quot;0&quot;&gt;
        ///	&lt;tr id=&quot;idWorkspaceArea&quot;&gt;
        ///		&lt;td&gt;
        ///			&lt;div id=&quot;EPKDisplayDiv&quot;&gt;
        ///				&lt;object classid=&quot;CLSID:7393552F-C4E6-49F0-8B01-52819BB9A0BC&quot; type=&quot;application/x-oleobject&quot;  codebase=&apos;***EPKURL***/CAB/WE_Client.CAB#version=***CABVERSION***&apos; style=&quot;display: none&quot;&gt;&lt;/object&gt;
        ///			&lt;/div&gt;
        ///		&lt;/td&gt;
        ///	&lt;/tr&gt;
        ///&lt;/table&gt;
        ///&lt;script src=&quot;/_layouts/epmlive/DHTML/dhtmlxajax.js&quot;&gt;&lt;/script&gt;
        ///&lt;script type=&quot;text/jscript&quot;&gt;
        ///
        ///	var sWebUrl = &quot;***WEBURL***&quot;; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string txtEPKWebpart {
            get {
                return ResourceManager.GetString("txtEPKWebpart", resourceCulture);
            }
        }
    }
}
