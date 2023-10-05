// <copyright file="BodyModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WebApi.Models
{
    /// <summary>
    /// this is Body Model.
    /// </summary>
    public class BodyModel
    {
        /// <summary>
        /// Gets or sets from brand.
        /// </summary>
        required public string Brand { get; set; }

        /// <summary>
        /// Gets or sets from product.
        /// </summary>
        required public string Product { get; set; }

        /// <summary>
        /// Gets or sets from star rating.
        /// </summary>
        required public int Survey_Star_Rating { get; set; }

        /// <summary>
        /// Gets or sets from survey comment.
        /// </summary>
        required public string Survey_Comment { get; set; }
    }
}
