using System;
namespace FC.Codeflix.Catalog.Domain.Entity;
public class Category
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Category(string name, string description)
	{
        Name = name;
        Description = description;
	}
}
