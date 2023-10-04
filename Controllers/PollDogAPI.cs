namespace WebApi.Controllers
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Text.Json;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using WebApi.Models;

    [ApiController]
    [Route("[controller]")]

    public class PollDogAPI : ControllerBase
    {
        private readonly ILogger<PollDogAPI> logger;

        public PollDogAPI(ILogger<PollDogAPI> logger)
        {
            this.logger = logger;
        }

        [HttpGet(Name = "GetData")]
        public async Task<ActionResult<IEnumerable<ResponseModel>>> Get()
        {
            try
            {
                var connectionString = "Data Source=Helios\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";
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
                                    product = reader.GetString(0),
                                    brand = reader.GetString(1),
                                    average_rating = reader.GetDouble(2),
                                };
                                brands.Add(brand);
                            }

                            return this.Ok(brands);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BodyModel rating)
        {
            var connectionString = "Data Source=Helios\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string productUniqueIdentifier = Guid.NewGuid().ToString();
                    string ratingUniqueIdentifier = Guid.NewGuid().ToString();
                    string brandUniqueIdentifier;

                    var brandCheckQuery = "SELECT id_brand FROM brands WHERE brand = @brand";
                    using (var command = new SqlCommand(brandCheckQuery, connection))
                    {
                        command.Parameters.AddWithValue("@brand", rating.Brand);
                        var existingBrandId = await command.ExecuteScalarAsync();
                        if (existingBrandId != null)
                        {
                            brandUniqueIdentifier = (string)existingBrandId;
                        }
                        else
                        {
                            brandUniqueIdentifier = Guid.NewGuid().ToString();
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

                return this.Ok(rating);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Error: {ex.Message}");
            }


        }
    }
}