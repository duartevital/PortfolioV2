<script setup lang="ts">
import type { PhotoCategory } from '~/types/photo'

type FilterValue = 'all' | PhotoCategory

const model = defineModel<FilterValue>({ required: true })

const filters: { label: string; value: FilterValue }[] = [
  { label: 'All',               value: 'all' },
  { label: 'Landscape / Nature', value: 'landscape-nature' },
  { label: 'Street / Urban',    value: 'street-urban' },
]
</script>

<template>
  <nav class="flex gap-2 px-4 md:px-8 py-6 flex-wrap" aria-label="Filter photos">
    <button
      v-for="f in filters"
      :key="f.value"
      class="px-4 py-1.5 rounded-full text-sm font-sans font-light tracking-wide transition-colors duration-200 border"
      :class="model === f.value
        ? 'bg-accent border-accent text-bg'
        : 'bg-transparent border-border text-muted hover:border-accent hover:text-text'"
      @click="model = f.value"
    >
      {{ f.label }}
    </button>
  </nav>
</template>
