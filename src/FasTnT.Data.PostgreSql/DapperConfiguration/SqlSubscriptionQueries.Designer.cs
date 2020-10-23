﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FasTnT.Data.PostgreSql.DapperConfiguration {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SqlSubscriptionQueries {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SqlSubscriptionQueries() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FasTnT.Data.PostgreSql.DapperConfiguration.SqlSubscriptionQueries", typeof(SqlSubscriptionQueries).Assembly);
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
        ///   Looks up a localized string similar to DELETE FROM subscriptions.pendingrequest WHERE subscription_id = (SELECT s.id FROM subscriptions.subscription s WHERE s.subscription_id = @SubscriptionId) AND request_id = ANY(@RequestIds);.
        /// </summary>
        internal static string AcknowledgePendingRequests {
            get {
                return ResourceManager.GetString("AcknowledgePendingRequests", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE from subscriptions.subscription WHERE id = (SELECT s.id FROM subscriptions.subscription s WHERE s.subscription_id = @SubscriptionId);.
        /// </summary>
        internal static string DeleteSubscription {
            get {
                return ResourceManager.GetString("DeleteSubscription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT s.id, s.destination, s.subscription_id, s.query_name, s.active, s.trigger, s.report_if_empty, s.schedule_minutes as minute, s.schedule_seconds as second, s.schedule_hours as hour, s.schedule_month as month, s.schedule_day_of_month as dayofmonth, s.schedule_day_of_week as dayofweek FROM subscriptions.subscription s;
        ///SELECT p.id, p.subscription_id, p.name FROM subscriptions.parameter p;
        ///SELECT pv.parameter_id, pv.subscription_id, pv.value FROM subscriptions.parameter_value pv;.
        /// </summary>
        internal static string GetAllSubscriptions {
            get {
                return ResourceManager.GetString("GetAllSubscriptions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT s.id, s.destination, s.subscription_id, s.query_name, s.active, s.trigger, s.report_if_empty, s.schedule_minutes, s.schedule_seconds, s.schedule_hours, s.schedule_month, s.schedule_day_of_month, s.schedule_day_of_week FROM subscriptions.subscription s WHERE s.subscription_id = @SubscriptionId;
        ///SELECT p.id, p.subscription_id, p.name FROM subscriptions.parameter p JOIN subscriptions.subscription s ON s.id = p.subscription_id WHERE s.subscription_id = @SubscriptionId;
        ///SELECT pv.parameter_id, pv.subscr [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetSubscriptionById {
            get {
                return ResourceManager.GetString("GetSubscriptionById", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT pr.request_id FROM subscriptions.pendingrequest pr JOIN subscriptions.subscription s ON s.id = pr.subscription_id WHERE s.id = (SELECT s.id FROM subscriptions.subscription s WHERE s.subscription_id = @SubscriptionId);.
        /// </summary>
        internal static string ListPendingRequests {
            get {
                return ResourceManager.GetString("ListPendingRequests", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO subscriptions.trigger(subscription_id, trigger_time, status, reason) VALUES((SELECT s.id FROM subscriptions.subscription s WHERE s.subscription_id = @SubscriptionId), NOW(), @Result, @Reason);.
        /// </summary>
        internal static string RegisterTrigger {
            get {
                return ResourceManager.GetString("RegisterTrigger", resourceCulture);
            }
        }
    }
}
