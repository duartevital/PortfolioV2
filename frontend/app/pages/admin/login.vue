<script setup lang="ts">
definePageMeta({ layout: false })

const { login, isAuthenticated } = useAuth()

if (import.meta.client && isAuthenticated.value) {
  await navigateTo('/admin')
}

const password = ref('')
const error = ref<string | null>(null)
const loading = ref(false)

async function submit() {
  error.value = null
  loading.value = true
  error.value = await login(password.value)
  loading.value = false
  if (!error.value) await navigateTo('/admin')
}
</script>

<template>
  <div class="min-h-screen bg-bg flex items-center justify-center px-4">
    <form
      class="w-full max-w-sm space-y-6"
      @submit.prevent="submit"
    >
      <div>
        <h1 class="font-serif text-2xl text-text">Admin</h1>
        <p class="text-muted text-sm font-sans mt-1">Vital Photography</p>
      </div>

      <div class="space-y-3">
        <input
          v-model="password"
          type="password"
          placeholder="Password"
          autocomplete="current-password"
          required
          class="w-full bg-surface border border-border rounded px-4 py-3 text-text font-sans text-sm placeholder:text-muted focus:outline-none focus:border-accent transition-colors"
        />

        <p v-if="error" class="text-red-400 text-sm font-sans">{{ error }}</p>

        <button
          type="submit"
          :disabled="loading"
          class="w-full bg-accent hover:bg-accent-dim text-bg font-sans font-medium text-sm py-3 rounded transition-colors disabled:opacity-50"
        >
          {{ loading ? 'Signing in…' : 'Sign in' }}
        </button>
      </div>

      <NuxtLink to="/" class="block text-center text-muted text-xs font-sans hover:text-text transition-colors">
        ← Back to gallery
      </NuxtLink>
    </form>
  </div>
</template>
