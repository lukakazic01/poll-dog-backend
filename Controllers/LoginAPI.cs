// <copyright file="LoginAPI.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WebApi.Controllers
{
    using System.Data.SqlClient;
    using Microsoft.AspNetCore.Mvc;
    using WebApi.Models;
    using WebApi.Services;

    /// <summary>
    /// Login.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    ///
    [ApiController]
    [Route("[controller]")]
    public class LoginAPI : Controller
    {
        private readonly IConnectionStringService connectionStringService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginAPI"/> class.
        /// </summary>
        /// <param name="connectionStringService">The connection string service.</param>
        public LoginAPI(IConnectionStringService connectionStringService)
        {
            this.connectionStringService = connectionStringService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <param name="data">username and pass.</param>
        /// <returns>login data.</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<LoginModel>>> Post([FromBody] LoginModel data)
        {
            try
            {
                var connectionString = this.connectionStringService.GetConnectionString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string checkUsernameSql = "SELECT COUNT(*) FROM login WHERE Username = @Username";

                    using (SqlCommand checkUsernameCommand = new SqlCommand(checkUsernameSql, connection))
                    {
                        checkUsernameCommand.Parameters.AddWithValue("@Username", data.Username);

                        object result = await checkUsernameCommand.ExecuteScalarAsync();

                        try
                        {
                            int usernameCount = Convert.ToInt32(result);
                            Console.Write(usernameCount.ToString());
                            if (usernameCount > 0)
                            {
                                return this.StatusCode(400, "Username already exists");
                            }
                            else
                            {
                                Guid userGuid = Guid.NewGuid();
                                string insertUsernameSql = "INSERT INTO login VALUES (@id_user, @username, @password)";
                                using (SqlCommand insertUsernameCommand = new SqlCommand(insertUsernameSql, connection))
                                {
                                    insertUsernameCommand.Parameters.AddWithValue("@id_user", userGuid);
                                    insertUsernameCommand.Parameters.AddWithValue("@username", data.Username);
                                    insertUsernameCommand.Parameters.AddWithValue("@password", data.Password);
                                    insertUsernameCommand.ExecuteNonQuery();
                                }

                                return this.StatusCode(201, data);
                            }
                        }
                        catch (Exception)
                        {
                            return this.StatusCode(500, "Cant convert to int null value");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
