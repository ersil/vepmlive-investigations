﻿using System;
using System.Globalization;

namespace PortfolioEngineCore.Infrastructure.Fields
{
    public sealed class CostField : PFEField
    {
        #region Fields (1) 

        private decimal? _value;

        #endregion Fields 

        #region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="CostField"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public CostField(int id, string name) : base(id, name)
        {
        }

        #endregion Constructors 

        #region Methods (3) 

        // Public Methods (3) 

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override object GetValue()
        {
            return _value;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetValue(object value)
        {
            try
            {
                _value = (decimal) value;
            }
            catch (Exception)
            {
                _value = null;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _value != null ? _value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }

        #endregion Methods 
    }
}