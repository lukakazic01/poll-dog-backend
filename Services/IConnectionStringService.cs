// <copyright file="IConnectionStringService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WebApi.Services
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// connection string service to database.
    /// </summary>
    public interface IConnectionStringService
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns>connection.</returns>
        string? GetConnectionString();
    }

    /// <summary>
    /// connection string service.
    /// </summary>
    /// <seealso cref="WebApi.Services.IConnectionStringService" />
    public class ConnectionStringService : IConnectionStringService
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ConnectionStringService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns>
        /// connection.
        /// </returns>
        public string? GetConnectionString()
        {
            return this.configuration.GetConnectionString("PollDogDatabase");
        }
    }
}
