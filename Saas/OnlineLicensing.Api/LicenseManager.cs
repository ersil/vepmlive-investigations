﻿using OnlineLicensing.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLicensing.Api
{
    /// <summary>
    /// Class to manage all the license related options.
    /// </summary>
    public class LicenseManager : IDisposable
    {
        protected bool Disposed { get; private set; }

        /// <summary>
        /// Gets all the licences currently active in the account. Tere should only be one license active per product in the account.
        /// </summary>
        /// <param name="accountRef">The account reference number.</param>
        /// <returns>Return an IEnumerable<LicenseOrder> containing all the active licenses in the account.</returns>
        public static IEnumerable<LicenseOder> GetAllActiveLicenses(int accountRef)
        {
            using (var context = ConnectionHelper.CreateLicensingModel())
            {
                var orders = context.ORDERS.Where(l => l.account_ref == accountRef && l.enabled == true &&  l.activation <= DateTime.Now && l.expiration > DateTime.Now).ToList();

                foreach (var item in orders)
                {
                    var productName = item.product_id != null ? context.LICENSEPRODUCTS.SingleOrDefault(l => l.product_id == item.product_id).name ?? string.Empty : string.Empty;

                    var featureDetails = new StringBuilder();

                    foreach (var itemDetails in context.ORDERDETAILs.Where(o => o.order_id == item.order_id).ToList())
                    {
                        var featureName = context.DETAILTYPES.SingleOrDefault(d => d.detail_type_id == itemDetails.detail_type_id).detail_name;
                        featureDetails.Append(featureName);
                        featureDetails.Append(":");
                        featureDetails.Append(' ', 90);
                        featureDetails.Append(itemDetails.quantity);
                        featureDetails.Append("<br />");
                    }

                    yield return new LicenseOder
                    { 
                        ProductId = Convert.ToInt32(item.product_id),
                        Product = productName,
                        Features = featureDetails.ToString(),
                        ExpirationDate = item.expiration.ToShortDateString()
                    };
                }


            }
        }

        /// <summary>
        /// Adds a new license to an account.
        /// </summary>
        /// <param name="accountRef">The account reference number.</param>
        /// <param name="activationDate">The date of activation of the license.</param>
        /// <param name="expirationDate">The date of expiration of the license.</param>
        /// <param name="productId">The id of the purchased product.</param>
        /// <param name="FeatureList">Contains the quantity of seats for every product feature.</param>
        public void AddLicense(int accountRef, DateTime activationDate, DateTime expirationDate, int productId,List<Tuple<int,int>> FeatureList)
        {
            using (var context = ConnectionHelper.CreateLicensingModel())
            {
                var orderToAdd = new ORDER()
                {
                    order_id = Guid.NewGuid(),
                    account_ref = accountRef, 
                    activation = activationDate, 
                    expiration = expirationDate,
                    product_id = productId,
                     
                    contractid = "50000010",
                    plimusReferenceNumber = "00000",
                    dtcreated = DateTime.Now, 
                    quantity = 1,
                    version = 2,
                    enabled = true,
                    billingsystem = 3
            };

                foreach (var item in FeatureList)
                {
                    orderToAdd.ORDERDETAILs.Add(AddLicenseDetails(orderToAdd.order_id, item));
                }
                
                context.ORDERS.Add(orderToAdd);
                context.SaveChanges();

            }
        }

        /// <summary>
        /// Adds a new order detail to an order. The order details contains the information of how many seats are purchased for that license.
        /// </summary>
        /// <param name="orderId">The id of the related order</param>
        /// <param name="Feature">A tuple of the product feature and the quantity of seats purchased for that product.</param>
        /// <returns>Returns an OrderDetail item to be added to the License/Order object to be created.</returns>
        public ORDERDETAIL AddLicenseDetails(Guid orderId, Tuple<int,int> Feature )
        {
            return new ORDERDETAIL
            {
                order_detail_id = Guid.NewGuid(),
                order_id = orderId,
                detail_type_id = Feature.Item1, 
                quantity = Feature.Item2 
            };
        }

        /// <summary>
        /// Validates whether an account have an active license for the specified product.
        /// </summary>
        /// <param name="ProductID">The id of the product to check for existance.</param>
        /// <param name="accountRef">The reference to the account with active licenses.</param>
        /// <returns>Returns true if there is already an active license for that product. Returns false if there isn't an active license for that product.</returns>
        public bool ValidateSingleActiveLicenseForProduct(int ProductID, int accountRef)
        {
            return GetAllActiveLicenses(accountRef).Any(al => al.ProductId == ProductID);
        }

        /// <summary>
        /// Disposes object to the Garbage Collector.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }
    }

    /// <summary>
    /// Class to represent the purchased licenses in an account.
    /// </summary>
    public class LicenseOder
    {
        public int ProductId { get; set; }
        public string Product { get; set; }
        public string Features { get; set; }
        public string ExpirationDate { get; set; }
    }

    /// <summary>
    /// Class to represent the available features in a product.
    /// </summary>
    public class FeatureList
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
