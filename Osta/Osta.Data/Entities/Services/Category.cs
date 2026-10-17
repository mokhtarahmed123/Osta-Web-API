<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Services
{
    [Table("Category", Schema = "Service")]
=======
﻿namespace Osta.Data.Entities.Services
{
>>>>>>> c73a0b6 (Initial commit)

    public class Category
    {

        public int Id { get; set; }


        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}