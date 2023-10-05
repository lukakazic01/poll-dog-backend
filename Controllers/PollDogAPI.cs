// <copyright file="PollDogAPI.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WebApi.Controllers
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using WebApi.Models;
    using WebApi.Services;

    /// <summary>
    /// PollDogApi controller for managing rating for porducts.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Route("[controller]")]
    public class PollDogAPI : ControllerBase
    {
        private readonly ILogger<PollDogAPI> logger;

        private readonly IConnectionStringService connectionStringService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PollDogAPI"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="connectionStringService">The connection string service.</param>
        public PollDogAPI(ILogger<PollDogAPI> logger, IConnectionStringService connectionStringService)
        {
            this.logger = logger;
            this.connectionStringService = connectionStringService;
        }

        /// <summary>
        /// Gets data about average star rating for each product.
        /// </summary>
        /// <returns>list of prodcuts.</returns>
        [HttpGet(Name = "GetData")]
        public async Task<ActionResult<IEnumerable<ResponseModel>>> Get()
        {
            try
            {
                var connectionString = this.connectionStringService.GetConnectionString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM getAllData";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            List<ResponseModel> brands = new List<ResponseModel>();
                            while (await reader.ReadAsync())
                            {
                                ResponseModel brand = new ResponseModel
                                {
                                    Product = reader.GetString(0),
                                    Brand = reader.GetString(1),
                                    Average_rating = reader.GetDouble(2),
                                };
                                brands.Add(brand);
                            }

                            return this.StatusCode(200, brands);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Posts the specified rating.
        /// </summary>
        /// <param name="rating">The rating.</param>
        /// <returns>brand.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BodyModel rating)
        {
            var connectionString = this.connectionStringService.GetConnectionString();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    Guid productUniqueIdentifier = Guid.NewGuid();
                    Guid ratingUniqueIdentifier = Guid.NewGuid();
                    Guid brandUniqueIdentifier;

                    var brandCheckQuery = "SELECT id_brand FROM brands WHERE brand = @brand";
                    using (var command = new SqlCommand(brandCheckQuery, connection))
                    {
                        command.Parameters.AddWithValue("@brand", rating.Brand);
                        var existingBrandId = await command.ExecuteScalarAsync();
                        if (existingBrandId != null)
                        {
                            brandUniqueIdentifier = (Guid)existingBrandId;
                        }
                        else
                        {
                            brandUniqueIdentifier = Guid.NewGuid();
                            var brandInsertQuery = "INSERT INTO brands (id_brand, brand) VALUES (@brandUniqueIdentifier, @brand)";
                            using (var insertCommand = new SqlCommand(brandInsertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@brandUniqueIdentifier", brandUniqueIdentifier);
                                insertCommand.Parameters.AddWithValue("@brand", rating.Brand);
                                await insertCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    var productInsertQuery = "INSERT INTO products (id_product, product) VALUES (@productUniqueIdentifier, @product)";
                    using (var command = new SqlCommand(productInsertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@productUniqueIdentifier", productUniqueIdentifier);
                        command.Parameters.AddWithValue("@product", rating.Product);
                        await command.ExecuteNonQueryAsync();
                    }

                    var ratingInsertQuery = "INSERT INTO ratings (id_rating, star_rating, comment) VALUES (@ratingUniqueIdentifier, @star_rating, @comment)";
                    using (var command = new SqlCommand(ratingInsertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ratingUniqueIdentifier", ratingUniqueIdentifier);
                        command.Parameters.AddWithValue("@star_rating", rating.Survey_Star_Rating);
                        command.Parameters.AddWithValue("@comment", rating.Survey_Comment);
                        await command.ExecuteNonQueryAsync();
                    }

                    var productDetailsInsertQuery = "INSERT INTO productDetails (id_brand, id_product, id_rating) VALUES (@brandUniqueIdentifier, @productUniqueIdentifier, @ratingUniqueIdentifier)";
                    using (var command = new SqlCommand(productDetailsInsertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@brandUniqueIdentifier", brandUniqueIdentifier);
                        command.Parameters.AddWithValue("@productUniqueIdentifier", productUniqueIdentifier);
                        command.Parameters.AddWithValue("@ratingUniqueIdentifier", ratingUniqueIdentifier);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return this.StatusCode(201, rating);
            }
            catch (Exception ex)
            {
                if (rating.Brand == null)
                {
                    return this.StatusCode(406, "You must provide all the data required");
                }
                else
                {
                    return this.StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
    }
}