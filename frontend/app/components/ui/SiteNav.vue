<script setup lang="ts">
const route = useRoute()
const links = [
  { label: 'Gallery', to: '/' },
  { label: 'About',   to: '/about' },
  { label: 'Contact', to: '/contact' },
]
</script>

<template>
  <header class="flex items-center justify-between px-4 md:px-8 py-5 border-b border-border">
    <NuxtLink to="/" class="font-serif text-xl text-text tracking-wide hover:text-accent transition-colors">
      Vital Photography
    </NuxtLink>

    <nav class="flex gap-6" aria-label="Site navigation">
      <NuxtLink
        v-for="l in links"
        :key="l.to"
        :to="l.to"
        class="font-sans text-sm font-light tracking-widest uppercase transition-colors"
        :class="route.path === l.to ? 'text-accent' : 'text-muted hover:text-text'"
      >
        {{ l.label }}
      </NuxtLink>
    </nav>
  </header>
</template>
