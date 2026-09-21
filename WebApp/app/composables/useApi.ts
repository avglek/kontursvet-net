import type { ProblemDetails } from "~/types/api";
import { useAuth } from "./useAuth";

interface ApiOptions {
  skipAuth?: boolean;
  skipErrorHandling?: boolean;
}

class ApiError extends Error {
  status: number;
  problem: ProblemDetails | null;

  constructor(message: string, status: number, problem: ProblemDetails | null) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.problem = problem;
  }
}

export const useApi = () => {
  const config = useRuntimeConfig();
  const { token, logout } = useAuth();

  const baseURL = (config.public.apiBase as string) || "";

  const request = async <T>(
    method: "GET" | "POST" | "PUT" | "DELETE",
    url: string,
    body?: any,
    opts: ApiOptions = {},
  ): Promise<T> => {
    const headers: Record<string, string> = {};

    if (!opts.skipAuth && token.value) {
      headers.Authorization = `Bearer ${token.value}`;
    }

    try {
      return await $fetch<T>(url, {
        baseURL,
        method,
        body,
        headers,
        // Для multipart/form-data не ставим Content-Type — браузер сам выставит с boundary
        ...(body instanceof FormData
          ? {}
          : { headers: { ...headers, "Content-Type": "application/json" } }),
      });
    } catch (err: any) {
      const status = err?.response?.status ?? 0;
      const problem: ProblemDetails | null = err?.response?._data ?? null;
      const message =
        problem?.detail || problem?.title || err?.message || "Ошибка запроса";

      // 401 — токен истёк или невалиден: сбрасываем и уводим на логин
      if (status === 401 && !opts.skipAuth) {
        logout();
        if (import.meta.client) {
          await navigateTo("/admin/login");
        }
      }

      if (!opts.skipErrorHandling) {
        // Здесь можно показать toast — подключим позже
        console.error(`[API ${method} ${url}]`, status, problem);
      }

      throw new ApiError(message, status, problem);
    }
  };

  return {
    get: <T>(url: string, opts?: ApiOptions) =>
      request<T>("GET", url, undefined, opts),
    post: <T>(url: string, body?: any, opts?: ApiOptions) =>
      request<T>("POST", url, body, opts),
    put: <T>(url: string, body?: any, opts?: ApiOptions) =>
      request<T>("PUT", url, body, opts),
    delete: <T>(url: string, opts?: ApiOptions) =>
      request<T>("DELETE", url, undefined, opts),
  };
};

export { ApiError };
