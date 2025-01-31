﻿using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork;
namespace FC.Codeflix.Catalog.Domain.Entity;

public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category(string name, string description, bool isActive = true) : base()
	{
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = DateTime.Now;

        Validate();
	}

    public void Activate()
    {
        this.IsActive = true;
    }

    public void Deactivate()
    {
        this.IsActive = false;
    }

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description ?? Description;
        Validate();
    }

    private void Validate()
    {
        if (String.IsNullOrWhiteSpace(Name)) throw new EntityValidationException($"{nameof(Name)} should not be empty or null");
        if (Name.Length < 3) throw new EntityValidationException($"{nameof(Name)} should be at least 3 characters long");
        if (Name.Length > 255) throw new EntityValidationException($"{nameof(Name)} should be less or equal 255 characters long");
        if (Description is null) throw new EntityValidationException($"{nameof(Description)} should not be null");
        if (Description.Length > 10_000) throw new EntityValidationException($"{nameof(Description)} should be less or equal 10_000 characters long");
    }
}
