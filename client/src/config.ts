export const config = {
  // API base URL, if not provided use the same host and protocol as the frontend assuming
  // the API is served from the same origin
  apiBaseUrl: (() => {
    const rawUrl = process.env.REACT_APP_API_BASE_URL;
    if (!rawUrl) return `${window.location.protocol}//${window.location.host}/api/`;
    
    // Ensure protocol is present
    if (!rawUrl.startsWith('http://') && !rawUrl.startsWith('https://')) {
      return `https://${rawUrl.replace(/^\/+/, '')}`;
    }
    return rawUrl;
  })(),
  
  // Static pages path (served from the backend, so we use the API host but not the /api/ prefix)
  staticPagesPath: (() => {
    const rawUrl = process.env.REACT_APP_API_BASE_URL;
    if (!rawUrl) return `${window.location.protocol}//${window.location.host}/static/pages`;
    
    // Ensure protocol is present
    let baseUrl = rawUrl;
    if (!baseUrl.startsWith('http://') && !baseUrl.startsWith('https://')) {
      baseUrl = `https://${baseUrl.replace(/^\/+/, '')}`;
    }
    // Remove trailing /api/ or /api if it exists
    baseUrl = baseUrl.replace(/\/api\/?$/, '');
    return `${baseUrl}/static/pages`;
  })(),
  
  // UI Configuration
  ui: {
    // Maximum number of modules to show in the add module dropdown in group management
    maxModulesInDropdown: 20,
  },
};