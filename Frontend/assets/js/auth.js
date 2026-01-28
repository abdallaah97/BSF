const BASE_URL = 'https://localhost:7012/api'; // Use HTTPS to avoid redirect header stripping

class AuthService {
    static async login(username, password) {
        try {
            const response = await fetch(`${BASE_URL}/Auth/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ username, password }),
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.Message || 'Login failed');
            }

            const data = await response.json();
            console.log('Login success:', data);

            // Handle both camelCase and PascalCase from backend
            const accessToken = data.accessToken || data.AccessToken;
            const refreshToken = data.refreshToken || data.RefreshToken;

            if (accessToken && refreshToken) {
                this.setTokens(accessToken, refreshToken);
                this.setUserData(data);
                return data;
            } else {
                console.error('Tokens missing in response:', data);
                throw new Error('Login response missing tokens');
            }
        } catch (error) {
            console.error('Login Error:', error);
            throw error;
        }
    }

    static async refreshToken() {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) throw new Error('No refresh token available');

        try {
            const response = await fetch(`${BASE_URL}/Auth/RefreshToken?refreshToken=${encodeURIComponent(refreshToken)}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
                }
            });

            if (!response.ok) {
                this.logout();
                throw new Error('Refresh token expired');
            }

            const data = await response.json();
            const newAccessToken = typeof data === 'string' ? data : (data.accessToken || data.AccessToken);

            if (newAccessToken) {
                console.log('Token refreshed successfully');
                localStorage.setItem('accessToken', newAccessToken);
                return newAccessToken;
            }
            throw new Error('Invalid response from refresh token endpoint');
        } catch (error) {
            console.error('Refresh Token Error:', error);
            // Only logout if it's really a refresh failure, not a network error
            if (error.message.includes('Refresh token expired') || error.message.includes('401')) {
                this.logout();
            }
            throw error;
        }
    }

    static setTokens(accessToken, refreshToken) {
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
    }

    static setUserData(data) {
        const user = {
            id: data.id || data.Id,
            name: data.name || data.Name,
            email: data.email || data.Email,
            role: data.roleCode || data.RoleCode
        };
        localStorage.setItem('user', JSON.stringify(user));
    }

    static getAccessToken() {
        return localStorage.getItem('accessToken');
    }

    static getUser() {
        const user = localStorage.getItem('user');
        return user ? JSON.parse(user) : null;
    }

    static isLoggedIn() {
        return !!localStorage.getItem('accessToken');
    }

    static logout() {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        window.location.href = 'index.html';
    }

    // Helper for authenticated requests
    static async fetchWithAuth(url, options = {}) {
        const token = this.getAccessToken();

        if (!token) {
            console.error('FetchWithAuth: No token found, logging out.');
            this.logout();
            return;
        }

        // Ensure headers are set correctly
        if (!options.headers) {
            options.headers = {};
        }

        const authHeader = `Bearer ${token.trim()}`;

        // Handle both simple objects and Headers objects
        if (options.headers instanceof Headers) {
            options.headers.set('Authorization', authHeader);
            if (!options.headers.has('Content-Type')) {
                options.headers.set('Content-Type', 'application/json');
            }
        } else {
            options.headers['Authorization'] = authHeader;
            if (!options.headers['Content-Type']) {
                options.headers['Content-Type'] = 'application/json';
            }
        }

        console.log(`[Auth] Fetching ${url}`);
        console.log(`[Auth] Token matches: ${authHeader.substring(0, 15)}...`);

        let response = await fetch(url, options);

        if (response.status === 401) {
            console.warn('[Auth] 401 Unauthorized detected. Attempting token refresh...');
            try {
                const newToken = await this.refreshToken();
                if (newToken) {
                    const newAuthHeader = `Bearer ${newToken.trim()}`;
                    if (options.headers instanceof Headers) {
                        options.headers.set('Authorization', newAuthHeader);
                    } else {
                        options.headers['Authorization'] = newAuthHeader;
                    }
                    console.log('[Auth] Retrying request with new token...');
                    response = await fetch(url, options);
                }
            } catch (err) {
                console.error('[Auth] Refresh failed, redirecting to login:', err);
                this.logout();
                throw err;
            }
        }

        return response;
    }
}
