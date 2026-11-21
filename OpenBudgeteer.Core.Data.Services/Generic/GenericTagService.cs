using OpenBudgeteer.Core.Data.Contracts.Repositories;
using OpenBudgeteer.Core.Data.Contracts.Services;
using OpenBudgeteer.Core.Data.Entities.Models;
using OpenBudgeteer.Core.Data.Services.Exceptions;

namespace OpenBudgeteer.Core.Data.Services.Generic;

/// <summary>
/// A Tag is identified by its name, compared case-insensitively, so that "Italy" and "italy" never become two Tags.
/// </summary>
public class GenericTagService : GenericBaseService<Tag>, ITagService
{
    private readonly ITagRepository _tagRepository;

    public GenericTagService(ITagRepository tagRepository) : base(tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public override IEnumerable<Tag> GetAll()
    {
        return _tagRepository
            .All()
            .OrderBy(tag => tag.Name)
            .ToList();
    }

    public IEnumerable<Tag> GetAllWithEntities()
    {
        return _tagRepository
            .AllWithIncludedEntities()
            .OrderBy(tag => tag.Name)
            .ToList();
    }

    public override Tag Create(Tag entity)
    {
        entity.Name = ValidateName(entity.Name, entity.Id);
        return base.Create(entity);
    }

    public override Tag Update(Tag entity)
    {
        entity.Name = ValidateName(entity.Name, entity.Id);
        return base.Update(entity);
    }

    /// <summary>
    /// Returns the Tags with the passed names, creating the ones which don't exist yet
    /// </summary>
    /// <param name="names">Tag names, duplicates are returned once</param>
    /// <returns>One Tag per distinct name</returns>
    public IEnumerable<Tag> GetOrCreate(IEnumerable<string?> names)
    {
        // Check every name before creating anything, so that an invalid name leaves no Tag behind
        var cleanNames = names.Select(CleanName).ToList();
        return cleanNames
            .Select(name => FindByName(name) ?? base.Create(new Tag { Name = name }))
            .DistinctBy(tag => tag.Id)
            .ToList();
    }

    private string ValidateName(string? name, Guid tagId)
    {
        var cleanName = CleanName(name);
        var existingTag = FindByName(cleanName);
        if (existingTag is not null && existingTag.Id != tagId)
            throw new EntityUpdateException($"A Tag named \"{existingTag.Name}\" already exists.");
        return cleanName;
    }

    private static string CleanName(string? name)
    {
        var cleanName = name?.Trim() ?? string.Empty;
        if (cleanName == string.Empty) throw new EntityUpdateException("Tag name cannot be empty.");
        if (cleanName.Length > Tag.NameMaxLength)
            throw new EntityUpdateException($"Tag name cannot be longer than {Tag.NameMaxLength} characters.");
        return cleanName;
    }

    private Tag? FindByName(string name)
    {
        // Invariant like the LOWER() of the database: with a Turkish culture, ToLower() turns "I" into a dotless "ı"
        var lowerName = name.ToLowerInvariant();
        return _tagRepository
            .All()
            .FirstOrDefault(tag => tag.Name.ToLower() == lowerName);
    }
}
