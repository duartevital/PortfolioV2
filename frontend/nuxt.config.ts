// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },

  modules: ['@nuxtjs/tailwindcss'],

  css: ['~/assets/css/tokens.css'],

  runtimeConfig: {
    public: {
      apiBaseUrl: '',   // override with NUXT_PUBLIC_API_BASE_URL env var
      blobBaseUrl: '',  // override with NUXT_PUBLIC_BLOB_BASE_URL env var
    },
  },

  app: {
    head: {
      title: 'Vital Photography',
      meta: [
        { name: 'description', content: 'Photography portfolio — landscape, nature, street, and urban.' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      ],
      link: [
        { rel: 'preconnect', href: 'https://fonts.googleapis.com' },
        { rel: 'preconnect', href: 'https://fonts.gstatic.com', crossorigin: '' },
      ],
    },
  },
})
