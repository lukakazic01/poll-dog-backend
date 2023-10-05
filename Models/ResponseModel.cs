// <copyright file="ResponseModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WebApi.Models
{
    /// <summary>
    /// Reponse Model when using Get.
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// Gets or sets the brand.
        /// </summary>
        /// <value>
        /// The brand.
        /// </value>
        required public string Brand { get; set; }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        required public string Product { get; set; }

        /// <summary>
        /// Gets or sets the average rating.
        /// </summary>
        /// <value>
        /// The average rating.
        /// </value>
        required public double Average_rating { get; set; }
    }
}
