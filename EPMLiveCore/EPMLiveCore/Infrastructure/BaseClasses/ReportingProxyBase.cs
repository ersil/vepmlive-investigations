﻿using System;
using System.Reflection;
using Microsoft.SharePoint;

namespace EPMLiveCore.Infrastructure
{
    public abstract class ReportingProxyBase
    {
        #region Fields (2) 

        protected readonly SPWeb Web;
        protected Assembly ReportingAssembly;

        #endregion Fields 

        #region Constructors (1) 

        protected ReportingProxyBase(SPWeb spWeb)
        {
            Web = spWeb;
            ReportingAssembly =
                Assembly.Load("EPMLiveReportsAdmin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b90e532f481cf050");
        }

        #endregion Constructors 

        #region Methods (4) 

        // Protected Methods (4) 

        /// <summary>
        ///     Gets the epm data class.
        /// </summary>
        /// <returns></returns>
        protected object GetEpmDataClass()
        {
            Type type = ReportingAssembly.GetType("EPMLiveReportsAdmin.EPMData", true, true);
            ConstructorInfo constructorInfo = type.GetConstructor(new[] {typeof (Guid)});
            return constructorInfo != null ? constructorInfo.Invoke(new object[] {Web.Site.ID}) : null;
        }

        protected object GetReportDataClass()
        {
            Type type = ReportingAssembly.GetType("EPMLiveReportsAdmin.ReportData", true, true);
            ConstructorInfo constructorInfo = type.GetConstructor(new[] { typeof(Guid) });
            return constructorInfo != null ? constructorInfo.Invoke(new object[] { Web.Site.ID }) : null;
        }

        /// <summary>
        ///     Gets the method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="classInstance">The class instance.</param>
        /// <returns></returns>
        protected MethodInfo GetMethod(string methodName, Type[] parameters, object classInstance)
        {
            return classInstance.GetType().GetMethod(methodName, parameters);
        }

        /// <summary>
        ///     Gets the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="classInstance">The class instance.</param>
        /// <returns></returns>
        protected object GetProperty(string propertyName, object classInstance)
        {
            return classInstance.GetType().GetProperty(propertyName).GetValue(classInstance, null);
        }

        /// <summary>
        ///     Sets the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="classInstance">The class instance.</param>
        protected void SetProperty(string propertyName, object value, object classInstance)
        {
            classInstance.GetType().GetProperty(propertyName).SetValue(classInstance, value, null);
        }

        #endregion Methods 
    }
}