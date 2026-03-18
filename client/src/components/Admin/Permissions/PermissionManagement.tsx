import React, { useState, useEffect } from 'react';
import permissionService, { Permission } from '../../../services/permissionService';
import { UserRole } from '../../../models/UserRole';
import styles from './PermissionManagement.module.css';

const PermissionManagement: React.FC = () => {
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [selectedRole, setSelectedRole] = useState<UserRole>(UserRole.ADMINISTRATOR);
  const [rolePermissions, setRolePermissions] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const roles = Object.values(UserRole);

  const fetchData = React.useCallback(async () => {
    try {
      setLoading(true);
      const [allPerms, activePerms] = await Promise.all([
        permissionService.getAllPermissions(),
        permissionService.getRolePermissions(selectedRole),
      ]);
      setPermissions(allPerms);
      setRolePermissions(activePerms);
    } catch (error) {
      console.error('Error fetching permissions:', error);
    } finally {
      setLoading(false);
    }
  }, [selectedRole]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const handleTogglePermission = (name: string) => {
    if (selectedRole === UserRole.ADMINISTRATOR) return; // Admins have all perms

    setRolePermissions((prev) =>
      prev.includes(name) ? prev.filter((p) => p !== name) : [...prev, name]
    );
  };

  const handleSave = async () => {
    try {
      setSaving(true);
      await permissionService.updateRolePermissions(selectedRole, rolePermissions);
      alert('Permissions updated successfully!');
    } catch (error) {
      console.error('Error saving permissions:', error);
      alert('Failed to save permissions.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h2>Access & Permission Management</h2>
        <p>Configure granular access for each system role.</p>
      </div>

      <div className={styles.roleSelector}>
        <label>Select Role:</label>
        <select
          value={selectedRole}
          onChange={(e) => setSelectedRole(e.target.value as UserRole)}
        >
          {roles.map((role) => (
            <option key={role} value={role}>
              {role}
            </option>
          ))}
        </select>
      </div>

      {loading ? (
        <div className={styles.loading}>Loading permissions...</div>
      ) : (
        <div className={styles.permissionGrid}>
          <div className={styles.gridHeader}>
            <span>Permission Name</span>
            <span>Description</span>
            <span>Status</span>
          </div>
          {permissions.map((perm) => (
            <div
              key={perm.id}
              className={`${styles.gridRow} ${
                rolePermissions.includes(perm.name) ? styles.activeRow : ''
              }`}
              onClick={() => handleTogglePermission(perm.name)}
            >
              <span className={styles.permName}>{perm.name}</span>
              <span className={styles.permDesc}>{perm.description}</span>
              <span className={styles.permToggle}>
                <input
                  type="checkbox"
                  checked={rolePermissions.includes(perm.name)}
                  readOnly
                  disabled={selectedRole === UserRole.ADMINISTRATOR}
                />
              </span>
            </div>
          ))}
        </div>
      )}

      <div className={styles.actions}>
        <button
          className={styles.saveButton}
          onClick={handleSave}
          disabled={saving || selectedRole === UserRole.ADMINISTRATOR}
        >
          {saving ? 'Saving...' : 'Save Changes'}
        </button>
        {selectedRole === UserRole.ADMINISTRATOR && (
          <p className={styles.adminNote}>
            * Administrators have full access and cannot be restricted.
          </p>
        )}
      </div>
    </div>
  );
};

export default PermissionManagement;
