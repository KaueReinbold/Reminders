﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Reminders.App.Resource {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///    A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal Resource() {
        }
        
        /// <summary>
        ///    Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Reminders.App.Resource.Resource", typeof(Resource).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///    Overrides the current thread's CurrentUICulture property for all
        ///    resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Ocorreu um erro ao executar a ação..
        /// </summary>
        public static string ErrorGenericMessage {
            get {
                return ResourceManager.GetString("ErrorGenericMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Lembrete cadastrado com sucesso..
        /// </summary>
        public static string SuccessCreateMessage {
            get {
                return ResourceManager.GetString("SuccessCreateMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Lembrete excluido com sucesso..
        /// </summary>
        public static string SuccessDeleteMessage {
            get {
                return ResourceManager.GetString("SuccessDeleteMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Lembre concluído com sucesso..
        /// </summary>
        public static string SuccessDoneMessage {
            get {
                return ResourceManager.GetString("SuccessDoneMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Lembrete editado com sucesso..
        /// </summary>
        public static string SuccessEditMessage {
            get {
                return ResourceManager.GetString("SuccessEditMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Lembre desmarcado como concluído com sucesso..
        /// </summary>
        public static string SuccessEnableMessage {
            get {
                return ResourceManager.GetString("SuccessEnableMessage", resourceCulture);
            }
        }
    }
}
