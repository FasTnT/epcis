﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FasTnT.Data.PostgreSql.DataRetrieval {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class PgSqlEventRequests {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PgSqlEventRequests() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FasTnT.Data.PostgreSql.DataRetrieval.PgSqlEventRequests", typeof(PgSqlEventRequests).Assembly);
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
        ///   Looks up a localized string similar to SELECT request.record_time as capture_time, event.id, event_type as type, event.record_time as event_time, action, read_point, event_timezone_offset, disposition, business_location, business_step, transformation_id, event.event_id, declaration_time, reason FROM epcis.event JOIN epcis.request on request.id = event.request_id LEFT JOIN epcis.error_declaration ed ON event.id = ed.event_id /**where**/ /**orderby**/ LIMIT @limit.
        /// </summary>
        internal static string EventQuery {
            get {
                return ResourceManager.GetString("EventQuery", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT event_id, epc as id, type, is_quantity, quantity, unit_of_measure FROM epcis.epc WHERE event_id = ANY(@eventids);
        ///SELECT event_id, field_id as id, parent_id, namespace, name, type, text_value, numeric_value, date_value FROM epcis.custom_field WHERE event_id = ANY(@eventids);
        ///SELECT event_id, transaction_type as type, transaction_id as id FROM epcis.business_transaction WHERE event_id = ANY(@eventids);
        ///SELECT event_id, type, source_dest_id as id, direction FROM epcis.source_destination WHERE event_ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RelatedQuery {
            get {
                return ResourceManager.GetString("RelatedQuery", resourceCulture);
            }
        }
    }
}
