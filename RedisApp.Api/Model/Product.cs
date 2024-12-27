using System;
using System.ComponentModel.DataAnnotations;

namespace RedisApp.Api.Model;

public class Product
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }

}
