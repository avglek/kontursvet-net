import type { LoginRequest, LoginResult, AdminMe } from "~/types/api";
import { useApi } from "./useApi";

const TOKEN_KEY = "kontursvet_token";
const USER_KEY = "kontursvet_user";

interface AuthUser {
  id: number;
  username: string;
  role: string;
}

export const useAuth = () => {
  // Общее состояние для всех компонентов
  const token = useState<string | null>("auth.token", () => null);
  const user = useState<AuthUser | null>("auth.user", () => null);

  // Восстановление из localStorage (только в браузере)
  const hydrate = () => {
    if (import.meta.server) return;

    if (!token.value) {
      const stored = localStorage.getItem(TOKEN_KEY);
      if (stored) token.value = stored;
    }
    if (!user.value) {
      const stored = localStorage.getItem(USER_KEY);
      if (stored) {
        try {
          user.value = JSON.parse(stored);
        } catch {
          /* ignore */
        }
      }
    }
  };

  const isAuthenticated = computed(() => !!token.value);

  const login = async (credentials: LoginRequest): Promise<void> => {
    const api = useApi();
    const result = await api.post<LoginResult>("/api/auth/login", credentials, {
      skipAuth: true,
    });

    token.value = result.accessToken;

    // Забираем инфу о пользователе
    const me = await api.get<AdminMe>("/api/admin/me");
    user.value = { id: me.id, username: me.username, role: me.role };

    // Сохраняем в localStorage
    if (import.meta.client) {
      localStorage.setItem(TOKEN_KEY, token.value!);
      localStorage.setItem(USER_KEY, JSON.stringify(user.value));
    }
  };

  const logout = () => {
    token.value = null;
    user.value = null;
    if (import.meta.client) {
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(USER_KEY);
    }
  };

  return {
    token: readonly(token),
    user: readonly(user),
    isAuthenticated,
    hydrate,
    login,
    logout,
  };
};
