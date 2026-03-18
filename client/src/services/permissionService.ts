import api from '../api/axios';

export interface Permission {
  id: string;
  name: string;
  description: string;
}

const permissionService = {
  getAllPermissions: async (): Promise<Permission[]> => {
    const response = await api.get('/permission');
    return response.data;
  },

  getRolePermissions: async (roleName: string): Promise<string[]> => {
    const response = await api.get(`/permission/role/${roleName}`);
    return response.data;
  },

  updateRolePermissions: async (roleName: string, permissionNames: string[]): Promise<void> => {
    await api.post(`/permission/role/${roleName}`, permissionNames);
  },
};

export default permissionService;
