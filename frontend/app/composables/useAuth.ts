const TOKEN_KEY = 'vp_admin_token'

export function useAuth() {
  const config = useRuntimeConfig()
  const baseUrl = config.public.apiBaseUrl || 'http://localhost:5000'

  const token = useState<string | null>('auth_token', () => {
    if (import.meta.client) return localStorage.getItem(TOKEN_KEY)
    return null
  })

  const isAuthenticated = computed(() => !!token.value)

  async function login(password: string): Promise<string | null> {
    try {
      const res = await $fetch<{ token: string }>(`${baseUrl}/api/v1/auth/login`, {
        method: 'POST',
        body: { password },
      })
      token.value = res.token
      if (import.meta.client) localStorage.setItem(TOKEN_KEY, res.token)
      return null
    } catch (e: unknown) {
      const err = e as { data?: { error?: string } }
      return err?.data?.error ?? 'Login failed'
    }
  }

  function logout() {
    token.value = null
    if (import.meta.client) localStorage.removeItem(TOKEN_KEY)
    navigateTo('/admin/login')
  }

  function authHeaders(): Record<string, string> {
    return token.value ? { Authorization: `Bearer ${token.value}` } : {}
  }

  return { token, isAuthenticated, login, logout, authHeaders }
}
