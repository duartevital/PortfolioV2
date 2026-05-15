// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },

  modules: [
    '@nuxtjs/tailwindcss',
    '@nuxtjs/sitemap',
  ],

  components: [
    { path: '~/components', pathPrefix: false },
  ],

  css: ['~/assets/css/tokens.css'],

  runtimeConfig: {
    public: {
      apiBaseUrl: '',   // NUXT_PUBLIC_API_BASE_URL
      blobBaseUrl: '',  // NUXT_PUBLIC_BLOB_BASE_URL
      siteUrl: '',      // NUXT_PUBLIC_SITE_URL — required for sitemap + OG absolute URLs
    },
  },

  // Sitemap
  sitemap: {
    // Dynamic photo pages are served via lightbox, not separate routes — static routes only
    urls: ['/', '/about', '/contact'],
  },

  app: {
    head: {
      titleTemplate: '%s · Vital Photography',
      meta: [
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        // Open Graph defaults (overridden per-page via useSeoMeta)
        { property: 'og:site_name',   content: 'Vital Photography' },
        { property: 'og:type',        content: 'website' },
        { name: 'twitter:card',       content: 'summary_large_image' },
      ],
      link: [
        { rel: 'preconnect', href: 'https://fonts.googleapis.com' },
        { rel: 'preconnect', href: 'https://fonts.gstatic.com', crossorigin: '' },
        {
          rel: 'stylesheet',
          href: 'https://fonts.googleapis.com/css2?family=DM+Serif+Display&family=DM+Sans:wght@300;400;500&display=swap',
        },
      ],
    },
  },
})
