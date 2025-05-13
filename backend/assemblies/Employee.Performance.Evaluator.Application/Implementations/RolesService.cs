using System.Data;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Roles;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class RolesService(
    IRolesRepository rolesRepository,
    IPermissionsRepository permissionsRepository) : IRolesService
{
    public async Task<List<Role>> GetAllRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await rolesRepository.GetAllWithPermissionsAsync(cancellationToken);

        return [.. roles];
    }

    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var role = await rolesRepository.GetByIdWithPermissionsAsync(id, cancellationToken);

        return role;
    }

    public async Task<Role> CreateRoleAsync(AddUpdateRoleRequest addUpdateRoleRequest, CancellationToken cancellationToken)
    {
        var roleName = addUpdateRoleRequest.RoleName;
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new InvalidOperationException("The role name is required.");
        }

        var roles = await rolesRepository.GetAllAsync(cancellationToken);
        if (roles.Any(r => r.RoleName == roleName))
        {
            throw new InvalidOperationException($"The role name '{roleName}' is already in use.");
        }

        var roleToCreate = new Role() { RoleName = roleName };
        var permissions = await permissionsRepository.GetAllWithTrackingAsync(cancellationToken);
        var permissionsToAdd = permissions.Where(p => addUpdateRoleRequest.PermissionIds.Contains(p.Id)).ToList();
        foreach (var permission in permissionsToAdd)
        {
            roleToCreate.Permissions.Add(permission);
        }

        var addedRole = await rolesRepository.AddAsync(roleToCreate, cancellationToken);
        await rolesRepository.SaveChangesAsync(cancellationToken);

        return addedRole;
    }

    public async Task<Role> UpdateRoleAsync(int id, AddUpdateRoleRequest addUpdateRoleRequest, CancellationToken cancellationToken)
    {
        var roleName = addUpdateRoleRequest.RoleName;
        var permissionIds = addUpdateRoleRequest.PermissionIds;
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new InvalidOperationException("The role name is required.");
        }

        var roleToUpdate = await rolesRepository.GetByIdWithPermissionsAsync(id, cancellationToken);
        if (roleToUpdate == null)
        {
            throw new InvalidOperationException($"No role with Id={id} found.");
        }

        roleToUpdate.RoleName = roleName;
        var permissions = await permissionsRepository.GetAllWithTrackingAsync(cancellationToken);
        roleToUpdate.Permissions.Clear();

        var newPermissions = permissions.Where(p => permissionIds.Contains(p.Id)).ToList();
        foreach (var permission in newPermissions)
        {
            roleToUpdate.Permissions.Add(permission);
        }

        rolesRepository.Update(roleToUpdate);
        await rolesRepository.SaveChangesAsync(cancellationToken);

        return roleToUpdate;
    }

    public async Task DeleteRoleAsync(int id, CancellationToken cancellationToken)
    {
        var roleToDelete = await rolesRepository.GetByIdAsync(id, cancellationToken);
        if (roleToDelete == null)
        {
            throw new InvalidOperationException($"No role with Id={id} found.");
        }

        try
        {
            rolesRepository.Delete(roleToDelete);
            await rolesRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete role with Id={id}. It might be in use by other entities.", ex);
        }
    }
}