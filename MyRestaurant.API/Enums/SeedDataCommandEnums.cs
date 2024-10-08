using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.API.Enums
{
    /// <summary>
    /// Enum representing available commands for data seeding.
    /// </summary>
    public enum SeedDataCommandEnums
    {
        /// <summary>
        /// Command for show all all possible commands
        /// </summary>
        ShowCommands,
        /// <summary>
        /// Command for seeding all data into the database.
        /// </summary>
        SeedData,
        /// <summary>
        /// Command for seeding users into the database.
        /// </summary>
        SeedUsers
    }
}
