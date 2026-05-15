<script setup lang="ts">
const year = new Date().getFullYear()

const links = [
  { label: 'Gallery', to: '/' },
  { label: 'About',   to: '/about' },
  { label: 'Contact', to: '/contact' },
]
</script>

<template>
  <footer class="border-t border-border mt-auto px-4 md:px-8 py-8 flex flex-col sm:flex-row items-center justify-between gap-4">
    <p class="font-serif text-muted text-sm tracking-wide">Vital Photography</p>

    <nav class="flex gap-6" aria-label="Footer navigation">
      <NuxtLink
        v-for="l in links"
        :key="l.to"
        :to="l.to"
        class="text-muted hover:text-text font-sans text-xs uppercase tracking-widest transition-colors"
      >
        {{ l.label }}
      </NuxtLink>
    </nav>

    <p class="text-border font-sans text-xs">© {{ year }}</p>
  </footer>
</template>
